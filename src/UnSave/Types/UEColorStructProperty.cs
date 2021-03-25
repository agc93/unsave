namespace UnSave.Types
{
    public class UEColorStructProperty : UEStructProperty
    {
        public override string StructType => "LinearColor";
        public float R, G, B, A;
    }
}