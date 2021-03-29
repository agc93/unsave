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
            var prop = new UEGuidStructProperty();
            prop.Value = new Guid(reader.ReadBytes(16));
            return prop;
        }

        public void SerializeStruct(IUnrealProperty baseProp, BinaryWriter writer)
        {
            var prop = baseProp as UEGuidStructProperty;
            writer.Write(prop.Value.ToByteArray());
        }
        public bool SupportsType(string type) => type == StructType;
    }
}