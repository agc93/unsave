using System.Collections.Generic;
using System.IO;
using UnSave.Types;

namespace UnSave.Serialization
{
    public class BytePropertySerializer : IUnrealPropertySerializer
    {
        public IEnumerable<string> Types => new[] {"ByteProperty"};

        public IUnrealProperty Deserialize(string name, string type, long valueLength, BinaryReader reader,
            PropertySerializer serializer)
        {
            var count = reader.ReadInt32();
            var bytes = reader.ReadBytes(count);
            return new UEByteProperty {Value = bytes.AsHex()};
        }

        public void Serialize(IUnrealProperty baseProp, BinaryWriter writer, PropertySerializer serializer)
        {
            var prop = baseProp as UEByteProperty;
            writer.Write(false); //terminator
            writer.WriteInt32(prop.Value.AsBytes().Length);
            writer.Write(prop.Value.AsBytes());
        }
    }
}