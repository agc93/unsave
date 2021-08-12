using System;

namespace UnSave.Extensions
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ViewPropertyAttribute : Attribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        public ViewPropertyAttribute() {
        }
        
        public string ViewPropertyName { get; set; }
        public bool ReadOnly { get; set; }
    }
}