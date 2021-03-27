using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnSave.Serialization;

namespace UnSave.Types
{
    [DebuggerDisplay("{" + nameof(StructType) + "}", Name = "{Name}")]
    public class UEGenericStructProperty : UEStructProperty
    {
        // public override string Type => "StructProperty";

        public UEGenericStructProperty(PropertySerializer serializer)
        {
            _serializer = serializer;
        }
        internal string _structType;
        private readonly PropertySerializer _serializer;


        public override string StructType => _structType;

        
        
        public List<IUnrealProperty> Properties = new List<IUnrealProperty>();

        public UEGenericStructProperty()
        {
            
        }

        public bool SupportsType(string type) => false;
    }
}