namespace UnSave.Types
{
    public abstract class UnrealPropertyBase<T> : UnrealPropertyBase
    {
        public T Value { get; set; }
    }
    public abstract class UnrealPropertyBase : IUnrealProperty
    {
        public string Name { get; set; }
        public abstract string Type { get; }
        public long ValueLength { get; set; }
        public string Address { get; set; }
    }
}