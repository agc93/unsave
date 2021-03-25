using System;
using System.IO;
using UnSave.Serialization;

namespace UnSave.Types
{
    public class UEDateTimeStructProperty : UEStructProperty
    {
        public DateTime Value { get; set; }
        public override string StructType => "DateTime";
    }
}