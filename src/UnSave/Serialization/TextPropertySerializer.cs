using System;
using System.IO;
using System.Linq;
using UnSave.Types;

namespace UnSave.Serialization
{
    public class TextPropertySerializer : UnrealPropertySerializer<UETextProperty>
    {
        public override UETextProperty DeserializeProp(string name, string type, long valueLength, BinaryReader reader,
            PropertySerializer serializer)
        {
            var prop = new UETextProperty();
            var startPos = reader.BaseStream.Position;
            /*if (valueLength == -1) {//IntProperty in MapProperty
                prop.Value = reader.ReadInt32();
                return prop;
            }*/
            var terminator = reader.ReadByte();

            if (terminator != 0)
                throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected terminator (0x00), but was (0x{terminator:x2})");

            var hasFlags = reader.PeekChar() > 0;
            if (hasFlags)
            {
                var flags = reader.ReadBytes(8);
                prop.Flags = flags;
            }
            else
            {
                prop.Flags = reader.ReadBytes(4);
            }
            prop.Id = reader.ReadByte().ToString(); //I think this is actually just a terminator
            var interval = reader.BaseStream.Position - startPos;
            do
            {
                var currData = reader.ReadUEString();
                prop.Data.Add(currData);
                interval = reader.BaseStream.Position - startPos;
            } while (interval < valueLength);
            prop.Value = prop.Data.Last();
            
            // prop.Data = reader.ReadUEString();
            // prop.Value = reader.ReadUEString();
            return prop;
        }

        public override void SerializeProp(UETextProperty prop, BinaryWriter writer, PropertySerializer serializer)
        {
            writer.Write(false); //terminator
            writer.Write(prop.Flags);
            writer.Write(byte.Parse(prop.Id));
            foreach (var dataEntry in prop.Data)
            {
                writer.WriteUEString(dataEntry);
            }
        }
    }
}