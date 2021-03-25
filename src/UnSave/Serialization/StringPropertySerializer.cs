using System;
using System.Collections.Generic;
using System.IO;
using UnSave.Types;

namespace UnSave.Serialization
{
    public class StringPropertySerializer : IUnrealPropertySerializer
    {
        public IEnumerable<string> Types =>
            new[] {"NameProperty", "StrProperty", "SoftObjectProperty", "ObjectProperty"};

        public IUnrealProperty Deserialize(string name, string type, long valueLength, BinaryReader reader,
            PropertySerializer serializer)
        {
            var prop = new UEStringProperty();
            
            if (valueLength > -1)
            {
                var terminator = reader.ReadByte();
                if (terminator != 0)
                    throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected terminator (0x00), but was (0x{terminator:x2})");
            }
            // ValueLength = valueLength;

            prop.Value = reader.ReadUEString();
            return prop;
        }
        
        public void Serialize(IUnrealProperty baseProp, BinaryWriter writer, PropertySerializer serializer)
        {
            var prop = baseProp as UEStringProperty;
            if (prop.ValueLength > -1) {
                writer.Write(false); //terminator
            }
            writer.WriteUEString(prop.Value, prop.ValueLength);
        }
    }
}