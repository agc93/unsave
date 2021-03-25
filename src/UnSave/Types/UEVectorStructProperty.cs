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
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Z = reader.ReadSingle();
            return this;
        }

        public void SerializeStruct(BinaryWriter writer)
        {
            writer.WriteSingle(X);
            writer.WriteSingle(Y);
            writer.WriteSingle(Z);
        }
        public bool SupportsType(string type) => type == StructType;
    }
}