using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private IUnrealPropertySerializer GetSerializer(string itemType)
        {
            var propertySerializer = Properties.FirstOrDefault(p => p.Types.Contains(itemType));
            if (propertySerializer == null)
            {
                //didn't find anything on first pass, check for struct matches
                Properties.Where(t => t.Types.Contains("StructProperty"));
            }

            return null;
        }
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

        public IUnrealProperty ReadItem(BinaryReader reader, string type, long valueLength, string name = null)
        {
            if (reader.PeekChar() < 0)
                return null;
            return Deserialize(name, type, valueLength, reader);
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

        private void Serialize(IUnrealProperty prop, string itemType, BinaryWriter writer)
        {
            var propSerializer = Properties.FirstOrDefault(p => p.Types.Contains(itemType));
            if (propSerializer == null)
            {
                throw new FormatException($"Offset: 0x{prop.Address:x8}. Unknown value type '{prop.Type}' of item '{prop.Name}'");
            }
            propSerializer.Serialize(prop, writer, this);
        }

        public virtual void Write(IUnrealProperty prop, BinaryWriter writer)
        {
            if ((prop.Name == "None" || prop.Name == null) && prop is UENoneProperty noneProperty)
            {
                noneProperty.Serialize(noneProperty, writer, this);
            }
            else
            {
                writer.WriteUEString(prop.Name);
                writer.WriteUEString(prop.Type);
                writer.WriteInt64(prop.ValueLength);
                Serialize(prop, prop.Type, writer);
            }
        }

        public virtual void WriteItem(IUnrealProperty prop, string itemType, BinaryWriter writer)
        {
            if ((prop.Name == "None" || prop.Name == null) && prop is UENoneProperty noneProperty)
            {
                noneProperty.Serialize(noneProperty, writer, this);
                return;
            }
            Serialize(prop, itemType, writer);
            // Write(prop, writer, true);
        }

        public virtual void WriteSet(IEnumerable<IUnrealProperty> props, string itemType, BinaryWriter writer)
        {
            var setSerializer = CollectionProperties.FirstOrDefault(p => p.Types.Contains(itemType));
            IEnumerable<IUnrealProperty> result;
            if (setSerializer != null)
            {
                //we have a known set type (like Struct)
                setSerializer.Serialize(props, itemType, writer, this);
            } else {
                //not a known set type (so probably a bunch of strings or whatever)
                //no collection available, try just doing it prop-by-prop
                /*writer.WriteUEString(itemType);
                writer.Write(false); //terminator
                writer.WriteInt32(props.Count());*/

                var allProps = props.ToList();
                for (int i=0; i<allProps.Count; i++)
                {
                    var propItem = allProps[i];
                    WriteItem(propItem, itemType, writer);
                    /*if (i == 0)
                    {
                        Write(propItem, writer);
                    }
                    else
                    {
                        WriteItem(propItem, itemType, writer);
                    }*/
                }
                var bareProps = new List<IUnrealProperty>();
                // throw new FormatException($"Offset: 0x{itemOffset:x8}. Unknown value type '{type}' of item '{name}'");
            }
        }
    }
}