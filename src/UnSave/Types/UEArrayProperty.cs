using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnSave.Serialization;

namespace UnSave.Types
{
    [DebuggerDisplay("{" + nameof(Count) + "}", Name = "{Name}")]
    public class UEArrayProperty : UnrealPropertyBase
    {
        public string ItemType;
        public List<IUnrealProperty> Items { get; set; } = new List<IUnrealProperty>();
        public int Count => Items.Count;
        public override string Type => "ArrayProperty";
    }
}