using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnSave.Serialization;

namespace UnSave.Types
{
    [DebuggerDisplay("{" + nameof(Value) + "}", Name = "{Name}")]
    public sealed class UEIntProperty : UnrealPropertyBase<int>
    {
        public override string Type => TypeName;
        public static string TypeName => "IntProperty";
        // public string Name { get; set; }

    }
}