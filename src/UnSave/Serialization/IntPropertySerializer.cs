using System;
using System.IO;
using UnSave.Types;

namespace UnSave.Serialization
{
    public class IntPropertySerializer : UnrealPropertySerializer<UEIntProperty>
    {
        public override UEIntProperty DeserializeProp(string name, string type, long valueLength, BinaryReader reader,
            PropertySerializer serializer)
        {
            var prop = new UEIntProperty();
            if (valueLength == -1) {//IntProperty in MapProperty
                prop.Value = reader.ReadInt32();
                return prop;
            }
            var terminator = reader.ReadByte();

            if (terminator != 0)
                throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected terminator (0x00), but was (0x{terminator:x2})");

            if (valueLength != sizeof(int))
                throw new FormatException($"Expected int value of length {sizeof(int)}, but was {valueLength}");

            prop.Value = reader.ReadInt32();
            prop.Name = name;
            return prop;
        }

        public override void SerializeProp(UEIntProperty prop, BinaryWriter writer, PropertySerializer serializer)
        {
            if (prop.ValueLength != -1)
                writer.Write(false); //terminator
            writer.WriteInt32(prop.Value);
        }
    }
}