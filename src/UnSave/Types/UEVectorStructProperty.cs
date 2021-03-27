using System.Diagnostics;
using System.IO;
using UnSave.Serialization;

namespace UnSave.Types
{
    [DebuggerDisplay("X = {X}, Y = {Y}, Z = {Z}", Name = "{Name}")]
    public class UEVectorStructProperty : UEStructProperty, IUnrealStructSerializer
    {
        public override string StructType => "Vector";
        public float X, Y, Z;
        public IUnrealProperty DeserializeStruct(BinaryReader reader)
        {
            var prop = new UEVectorStructProperty();
            prop.X = reader.ReadSingle();
            prop.Y = reader.ReadSingle();
            prop.Z = reader.ReadSingle();
            return prop;
        }

        public void SerializeStruct(IUnrealProperty baseProp, BinaryWriter writer)
        {
            var prop = baseProp as UEVectorStructProperty;
            writer.WriteSingle(prop.X);
            writer.WriteSingle(prop.Y);
            writer.WriteSingle(prop.Z);
        }
        public bool SupportsType(string type) => type == StructType;
    }
}