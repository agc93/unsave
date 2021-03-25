using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnSave.Serialization;

namespace UnSave.Types
{
    [DebuggerDisplay("{" + nameof(StructType) + "}", Name = "{Name}")]
    public class UEGenericStructProperty : UEStructProperty, IUnrealStructSerializer
    {
        public UEGenericStructProperty(PropertySerializer serializer)
        {
            _serializer = serializer;
        }
        internal string _structType;
        private readonly PropertySerializer _serializer;


        public override string StructType => _structType;

        public IUnrealProperty DeserializeStruct(BinaryReader reader)
        {
            while (_serializer.Read(reader) is IUnrealProperty prop)
            {
                Properties.Add(prop);
                if (prop is UENoneProperty)
                {
                    break;
                }
            }

            return this;
        }
        
        public List<IUnrealProperty> Properties = new List<IUnrealProperty>();

        public void SerializeStruct(BinaryWriter writer)
        {
            foreach (var prop in Properties)
            {
                _serializer.Write(prop, writer);
            }
        }

        public bool SupportsType(string type) => false;
    }
}