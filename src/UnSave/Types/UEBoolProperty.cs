using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnSave.Serialization;

namespace UnSave.Types
{
    [DebuggerDisplay("{" + nameof(Value) + "}", Name = "{Name}")]
    public class UEBoolProperty : UnrealPropertyBase<bool>
    {
        public override string Type => "BoolProperty";
    }
}