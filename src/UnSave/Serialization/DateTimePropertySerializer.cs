using System;
using System.IO;
using UnSave.Types;

namespace UnSave.Serialization
{
    public class DateTimePropertySerializer : UnrealPropertySerializer<UEDateTimeStructProperty>
    {
        public override UEDateTimeStructProperty DeserializeProp(string name, string type, long valueLength, BinaryReader reader,
            PropertySerializer serializer)
        {
            var prop = new UEDateTimeStructProperty();
            prop.Value = DateTime.FromBinary(reader.ReadInt64());
            return prop;
        }

        public override void SerializeProp(UEDateTimeStructProperty prop, BinaryWriter writer, PropertySerializer serializer)
        {
            writer.WriteInt64(prop.Value.ToBinary());
        }
    }
}