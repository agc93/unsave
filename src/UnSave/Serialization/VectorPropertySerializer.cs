using System;
using System.IO;
using UnSave.Types;

namespace UnSave.Serialization
{
    public class VectorPropertySerializer : UnrealPropertySerializer<UEVectorStructProperty>
    {
        public IUnrealProperty DeserializeStruct(BinaryReader reader)
        {
            var prop = new UEVectorStructProperty();
            prop.X = reader.ReadSingle();
            prop.Y = reader.ReadSingle();
            prop.Z = reader.ReadSingle();
            return prop;
        }

        public void SerializeStruct(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public bool SupportsType(string type)
        {
            return type == "Vector";
        }

        public override UEVectorStructProperty DeserializeProp(string name, string type, long valueLength, BinaryReader reader,
            PropertySerializer serializer)
        {
            var prop = new UEVectorStructProperty();
            prop.X = reader.ReadSingle();
            prop.Y = reader.ReadSingle();
            prop.Z = reader.ReadSingle();
            return prop;
        }

        public override void SerializeProp(UEVectorStructProperty prop, BinaryWriter writer, PropertySerializer serializer)
        {
            writer.WriteSingle(prop.X);
            writer.WriteSingle(prop.Y);
            writer.WriteSingle(prop.Z);
        }
    }
}