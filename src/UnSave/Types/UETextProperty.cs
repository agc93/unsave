using System;
using System.Collections.Generic;
using System.IO;
using UnSave.Serialization;

namespace UnSave.Types
{
    public class UETextProperty : UnrealPropertyBase<string>
    {
        public override string Type => "TextProperty";

        public byte[] Flags { get; set; }
        public string Id { get; set; }
        public List<string> Data { get; set; } = new List<string>();
    }
}