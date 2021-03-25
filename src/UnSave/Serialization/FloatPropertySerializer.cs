using System;
using System.Collections.Generic;
using System.IO;
using UnSave.Types;

namespace UnSave.Serialization
{
    public class FloatPropertySerializer : IUnrealPropertySerializer
    {
        public IEnumerable<string> Types => new[] {"FloatProperty"};

        public IUnrealProperty Deserialize(string name, string baseType, long valueLength, BinaryReader reader,
            PropertySerializer serializer)
        {
            var prop = new UEFloatProperty();
            var terminator = reader.ReadByte();
            if (terminator != 0)
                throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected terminator (0x00), but was (0x{terminator:x2})");

            if (valueLength != sizeof(float))
                throw new FormatException($"Expected float value of length {sizeof(float)}, but was {valueLength}");

            prop.Value = reader.ReadSingle();
            return prop;
        }

        public void Serialize(IUnrealProperty baseProp, BinaryWriter writer, PropertySerializer serializer)
        {
            var prop = baseProp as UEFloatProperty;
            writer.Write(false); //terminator
            writer.WriteSingle(prop.Value);
        }
    }
}