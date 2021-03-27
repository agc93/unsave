using System.Diagnostics;

namespace UnSave.Types
{
    public abstract class UEStructProperty : UnrealPropertyBase
    {
        public override string Type => "StructProperty";
        public abstract string StructType { get; }
    }
}