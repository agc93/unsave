using System;
using System.Collections.Generic;
using System.IO;
using UnSave.Types;

namespace UnSave.Serialization
{
    public class BoolPropertySerializer : IUnrealPropertySerializer
    {
        public IEnumerable<string> Types => new[] {"BoolProperty"};

        public IUnrealProperty Deserialize(string name, string type, long valueLength, BinaryReader reader, PropertySerializer serializer)
        {
            var boolProp = new UEBoolProperty();
            if (valueLength != 0)
                throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected bool value length 0, but was {valueLength}");

            var val = reader.ReadInt16();
            boolProp.Value = val switch
            {
                0 => false,
                1 => true,
                _ => throw new InvalidOperationException(
                    $"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected bool value, but was {val}")
            };
            return boolProp;
        }

        public void Serialize(IUnrealProperty prop, BinaryWriter writer, PropertySerializer serializer)
        {
            writer.WriteInt16((short)(((UEBoolProperty)prop).Value ? 1 : 0));
        }
    }
    
    public class BytePropertySerializer : IUnrealPropertySerializer
    {
        public IEnumerable<string> Types => new[] {"ByteProperty"};

        public IUnrealProperty Deserialize(string name, string type, long valueLength, BinaryReader reader,
            PropertySerializer serializer)
        {
            var count = reader.ReadInt32();
            var bytes = reader.ReadBytes(count);
            return new UEByteProperty {Value = bytes.AsHex()};
        }

        public void Serialize(IUnrealProperty baseProp, BinaryWriter writer, PropertySerializer serializer)
        {
            var prop = baseProp as UEByteProperty;
            writer.Write(false); //terminator
            writer.WriteInt32(prop.Value.AsBytes().Length);
            writer.Write(prop.Value.AsBytes());
        }
    }
}