using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnSave.Serialization;
using UnSave.Types;

namespace UnSave
{
    public class PropertySerializer
    {
        public PropertySerializer(IEnumerable<IUnrealPropertySerializer> serializers, IEnumerable<IUnrealCollectionPropertySerializer> collections)
        {
            Properties = serializers.ToList();
            CollectionProperties = collections.ToList();
        }
        private List<IUnrealPropertySerializer> Properties { get; set; }
        private List<IUnrealCollectionPropertySerializer> CollectionProperties { get; set; }
        public IUnrealProperty Read(BinaryReader reader)
        {
            if (reader.PeekChar() < 0)
                return null;

            var name = reader.ReadUEString();
            if (name == null)
                return null;

            if (name == "None")
                return new UENoneProperty { Name = name };

            var type = reader.ReadUEString();
            var valueLength = reader.ReadInt64();
            return Deserialize(name, type, valueLength, reader);
        }

        [Obsolete("This is just awful", false)]
        public IUnrealPropertySerializer GetSerializer(string type)
        {
            var prop = Properties.FirstOrDefault(p => p.Types.Contains(type));
            return prop;
        }

        public IUnrealProperty ReadItem(BinaryReader reader, string type, int valueLength, string name = null)
        {
            if (reader.PeekChar() < 0)
                return null;

            // var name = reader.ReadUEString();
            // var length = reader.ReadInt64();
            return Deserialize(name, type, valueLength, reader);
        }

        [Obsolete("This is a fuckin hack", false)]
        public IUnrealProperty DeserializeType(IUnrealPropertySerializer propSerializer, string name, string type, long valueLength, BinaryReader reader)
        {
            IUnrealProperty result;
            var itemOffset = reader.BaseStream.Position;
            if (propSerializer != null)
            {
                result = propSerializer.Deserialize(name, type, valueLength, reader, this);
            }
            else
            {
                throw new FormatException($"Offset: 0x{itemOffset:x8}. Unknown value type '{type}' of item '{name}'");
            }
            result.Name = name;
            // result.Type = type;
            result.ValueLength = valueLength;
            result.Address ??= $"0x{ itemOffset :x8}";
            return result;
        }

        private IUnrealProperty Deserialize(string name, string type, long valueLength, BinaryReader reader)
        {
            IUnrealProperty result;
            var itemOffset = reader.BaseStream.Position;
            var prop = Properties.FirstOrDefault(p => p.Types.Contains(type));
            if (prop != null)
            {
                result = prop.Deserialize(name, type, valueLength, reader, this);
            }
            else
            {
                throw new FormatException($"Offset: 0x{itemOffset:x8}. Unknown value type '{type}' of item '{name}'");
            }
            result.Name = name;
            // result.Type = type;
            result.ValueLength = valueLength;
            result.Address ??= $"0x{ itemOffset :x8}";
            return result;
        }

        public List<IUnrealProperty> ReadSet(BinaryReader reader, string itemType, int count)
        {
            if (reader.PeekChar() < 0)
                return null;
            var setSerializer = CollectionProperties.FirstOrDefault(p => p.Types.Contains(itemType));
            IEnumerable<IUnrealProperty> result;
            if (setSerializer != null)
            {
                //we have a known set type (like Struct)
                var itemOffset = reader.BaseStream.Position;
                var name = reader.ReadUEString();
                var type = reader.ReadUEString();
                var valueLength = reader.ReadInt64();
                result = setSerializer.Deserialize(name, type, valueLength, count, reader, this).ToList();
                foreach (var item in result)
                {
                    item.Name ??= name;
                    // item.Type = type;
                    item.Address ??= $"0x{ itemOffset :x8}";
                    // item.ValueLength = valueLength;
                }
            } else {
                //not a known set type (so probably a bunch of strings or whatever)
                var bareProps = new List<IUnrealProperty>();
                //no collection available, try just doing it prop-by-prop
                for (int i = 0; i < count; i++)
                {
                    var itemProp = ReadItem(reader, itemType, -1);
                    bareProps.Add(itemProp);
                }
                result = bareProps;
                // throw new FormatException($"Offset: 0x{itemOffset:x8}. Unknown value type '{type}' of item '{name}'");
            }
            
            return result.ToList();
        }

        public virtual void Write(IUnrealProperty prop, BinaryWriter writer, bool skipHeader = false)
        {
            if ((prop.Name == "None" || prop.Name == null) && prop is UENoneProperty noneProperty)
            {
                noneProperty.Serialize(noneProperty, writer, this);
            }
            else
            {
                var propSerializer = Properties.FirstOrDefault(p => p.Types.Contains(prop.Type));
                if (propSerializer == null)
                {
                    throw new FormatException($"Offset: 0x{prop.Address:x8}. Unknown value type '{prop.Type}' of item '{prop.Name}'");
                }

                if (!skipHeader)
                {
                    writer.WriteUEString(prop.Name);
                    writer.WriteUEString(prop.Type);
                    writer.WriteInt64(prop.ValueLength);
                }
                propSerializer.Serialize(prop, writer, this);
            }
        }
    }
    public class SaveSerializer
    {
        public SaveSerializer(PropertySerializer propSerializer)
        {
            this.propSerializer = propSerializer;
        }
        private PropertySerializer propSerializer { get; set; }
        public GvasSaveData Read(Stream stream)
        {
            using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
            {
                var header = reader.ReadBytes(GvasSaveData.Header.Length);
                if (!GvasSaveData.Header.SequenceEqual(header))
                    throw new FormatException($"Invalid header, expected {GvasSaveData.Header.AsHex()}");

                // ReSharper disable once UseObjectOrCollectionInitializer
                var result = new GvasSaveData();
                result.SaveGameVersion = reader.ReadInt32();
                result.PackageVersion = reader.ReadInt32();
                result.EngineVersion.Major = reader.ReadInt16();
                result.EngineVersion.Minor = reader.ReadInt16();
                result.EngineVersion.Patch = reader.ReadInt16();
                result.EngineVersion.Build = reader.ReadInt32();
                result.EngineVersion.BuildId = reader.ReadUEString();
                result.CustomFormatVersion = reader.ReadInt32();
                result.CustomFormatData.Count = reader.ReadInt32();
                result.CustomFormatData.Entries = new CustomFormatDataEntry[result.CustomFormatData.Count];
                for (var i = 0; i < result.CustomFormatData.Count; i++)
                {
                    var entry = new CustomFormatDataEntry();
                    entry.Id = new Guid(reader.ReadBytes(16));
                    entry.Value = reader.ReadInt32();
                    result.CustomFormatData.Entries[i] = entry;
                }
                result.SaveGameType = reader.ReadUEString();

                while (propSerializer.Read(reader) is IUnrealProperty prop)
                    result.Properties.Add(prop);

                return result;
            }
        }
        
        public void Write(FileStream stream, GvasSaveData data)
        {
            using (var writer = new BinaryWriter(stream, Encoding.ASCII, true))
            {
                writer.Write(GvasSaveData.Header);

                writer.WriteInt32(data.SaveGameVersion);
                writer.WriteInt32(data.PackageVersion);
                writer.WriteInt16(data.EngineVersion.Major);
                writer.WriteInt16(data.EngineVersion.Minor);
                writer.WriteInt16(data.EngineVersion.Patch);
                writer.WriteInt32(data.EngineVersion.Build);
                writer.WriteUEString(data.EngineVersion.BuildId);
                writer.WriteInt32(data.CustomFormatVersion);
                writer.WriteInt32(data.CustomFormatData.Count);
                for (var i = 0; i < data.CustomFormatData.Count; i++)
                {
                    var entry = data.CustomFormatData.Entries[i];
                    writer.Write(entry.Id.ToByteArray());
                    writer.WriteInt32(entry.Value);
                }
                writer.WriteUEString(data.SaveGameType);

                foreach (var prop in data.Properties)
                {
                    propSerializer.Write(prop, writer);
                }

                writer.WriteInt32(0);
            }
        }
    }
}
