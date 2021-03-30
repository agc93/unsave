using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnSave.Serialization;

namespace UnSave.Types
{
    [DebuggerDisplay("{" + nameof(Value) + "}", Name = "{Name}")]
    public class UETextProperty : UnrealPropertyBase<string>
    {
        public override string Type => "TextProperty";

        public byte[] Flags { get; set; }
        public string Id { get; set; }
        public List<string> Data { get; set; } = new List<string>();
    }
}