using System;

namespace UnSave.Extensions
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ValuePropertyAttribute : Attribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        public ValuePropertyAttribute() {
        }
        
        public string ValuePropertyName { get; set; }
        public bool ReadOnly { get; set; }
        public string CreateProperty { get; set; }
        public bool AllowDefault { get; set; }
    }
}