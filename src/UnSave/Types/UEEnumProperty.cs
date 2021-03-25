using System;
using System.Collections.Generic;
using System.IO;
using UnSave.Serialization;

namespace UnSave.Types
{
    public class UEEnumProperty : UnrealPropertyBase<string>
    {
        public string EnumType { get; set; }
        public override string Type => "EnumProperty";
    }
}