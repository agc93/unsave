using System;

namespace UnSave.Extensions
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class SavePropertyAttribute : Attribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        public SavePropertyAttribute(string savePropertyName, Type propertyType) {
            // // TODO: Implement code here
            // throw new NotImplementedException();
            
        }

        public string PropertyName { get; set; }
        public bool IncludeValueProperty { get; set; }
    }
}