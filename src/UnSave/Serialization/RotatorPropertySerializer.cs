using System.IO;
using UnSave.Types;

namespace UnSave.Serialization
{
    public class RotatorPropertySerializer : UnrealPropertySerializer<UERotatorStructProperty>
    {
        public override UERotatorStructProperty DeserializeProp(string name, string type, long valueLength, BinaryReader reader,
            PropertySerializer serializer)
        {
            var prop = new UERotatorStructProperty();
            prop.X = reader.ReadSingle();
            prop.Y = reader.ReadSingle();
            prop.Z = reader.ReadSingle();
            return prop;
        }

        public override void SerializeProp(UERotatorStructProperty prop, BinaryWriter writer, PropertySerializer serializer)
        {
            writer.WriteSingle(prop.X);
            writer.WriteSingle(prop.Y);
            writer.WriteSingle(prop.Z);
        }
    }
}