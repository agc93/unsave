using System.IO;
using UnSave.Types;

namespace UnSave.Serialization
{
    public class ColorPropertySerializer : UnrealPropertySerializer<UEColorStructProperty>
    {
        public override UEColorStructProperty DeserializeProp(string name, string type, long valueLength, BinaryReader reader,
            PropertySerializer serializer)
        {
            var prop = new UEColorStructProperty();
            prop.R = reader.ReadSingle();
            prop.G = reader.ReadSingle();
            prop.B = reader.ReadSingle();
            prop.A = reader.ReadSingle();
            return prop;
        }

        public override void SerializeProp(UEColorStructProperty prop, BinaryWriter writer, PropertySerializer serializer)
        {
            if (prop.ValueLength == 0) return;
            writer.WriteSingle(prop.R);
            writer.WriteSingle(prop.G);
            writer.WriteSingle(prop.B);
            writer.WriteSingle(prop.A);
        }
    }
}