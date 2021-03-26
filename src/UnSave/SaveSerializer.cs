using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using UnSave.Types;

namespace UnSave
{
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
