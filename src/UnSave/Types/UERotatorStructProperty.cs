namespace UnSave.Types
{
    public class UERotatorStructProperty : UEStructProperty
    {
        public float X, Y, Z;
        public override string StructType => "Rotator";
    }
}