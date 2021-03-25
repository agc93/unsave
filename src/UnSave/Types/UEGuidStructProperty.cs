using System;
using System.IO;
using UnSave.Serialization;

namespace UnSave.Types
{
    public class UEGuidStructProperty : UEStructProperty, IUnrealStructSerializer
    {
        public Guid Value;
        public override string StructType => "Guid";
        public IUnrealProperty DeserializeStruct(BinaryReader reader)
        {
            Value = new Guid(reader.ReadBytes(16));
            return this;
        }

        public void SerializeStruct(BinaryWriter writer)
        {
            writer.Write(Value.ToByteArray());
        }
        public bool SupportsType(string type) => type == StructType;
    }
}