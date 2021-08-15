using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnSave.Serialization;

namespace UnSave.Types
{
    [DebuggerDisplay("{" + nameof(Value) + "}", Name = "{Name}")]
    public class UEStringProperty : UnrealPropertyBase<string>
    {
        public UEStringProperty()
        {
            
        }

        internal UEStringProperty(string valueType)
        {
            ValueType = valueType;
        }
        public override string Type => ValueType;
        public string ValueType { get; } = "StrProperty";
        private string _value;

        public override string Value
        {
            get => _value;
            set
            {
                _value = value;
                ValueLength = _value.Length + 5;
            } 
        }
    }
}