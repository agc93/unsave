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
            if (valueLength > -1)
            {
                var terminator = reader.ReadByte();
                if (terminator != 0)
                    throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected terminator (0x00), but was (0x{terminator:x2})");
            }
            var value = reader.ReadUEString();

            return new UEStringProperty(type) {Value = value};

            /*return type switch
            {
                "StrProperty" => new UEStringProperty() {Value = value},
                "NameProperty" => new UEStringProperty("NameProperty") {Value = value, ValueType = "NameProperty"},
                "ObjectProperty" => new UEStringProperty() {Value = value, ValueType = "ObjectProperty"},
                "SoftObjectProperty" => new UEStringProperty() {Value = value, ValueType = "SoftObjectProperty"},
                _ => throw new FormatException("Unrecognised string format!")
            };*/
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