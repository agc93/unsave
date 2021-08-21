using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnSave.Serialization;

namespace UnSave.Types
{
    [DebuggerDisplay("float:{" + nameof(Value) + "}", Name = "{Name}")]
    public class UEFloatProperty : UnrealPropertyBase<float>
    {
        public override string Type => "FloatProperty";
        public override long ValueLength { get; set; } = 4;
    }
}