using System;
using System.Collections.Generic;
using System.IO;
using UnSave.Types;

namespace UnSave.Serialization
{
    public class EnumPropertySerializer : IUnrealPropertySerializer
    {
        public IEnumerable<string> Types => new[] {"EnumProperty"};
        public IUnrealProperty Deserialize(string name, string baseType, long valueLength, BinaryReader reader,
            PropertySerializer serializer)
        {
            var prop = new UEEnumProperty();
            prop.EnumType = reader.ReadUEString();

            var terminator = reader.ReadByte();
            if (terminator != 0)
                throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected terminator (0x00), but was (0x{terminator:x2})");

            // valueLength starts here

            prop.Value = reader.ReadUEString();
            return prop;
        }

        public void Serialize(IUnrealProperty prop, BinaryWriter writer, PropertySerializer serializer)
        {
            var enumProp = prop as UEEnumProperty;
            writer.WriteUEString(enumProp.EnumType);
            writer.Write(false); //terminator
            writer.WriteUEString(enumProp.Value);
        }
    }
}