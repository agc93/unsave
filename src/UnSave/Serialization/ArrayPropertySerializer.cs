using System;
using System.IO;
using UnSave.Types;

namespace UnSave.Serialization
{
    public class ArrayPropertySerializer : UnrealPropertySerializer<UEArrayProperty>
    {
        public override UEArrayProperty DeserializeProp(string name, string type, long valueLength, BinaryReader reader,
            PropertySerializer serializer)
        {
            var prop = new UEArrayProperty();
            prop.ItemType = reader.ReadUEString();
            var terminator = reader.ReadByte();
            if (terminator != 0)
                throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected terminator (0x00), but was (0x{terminator:x2})");

            // valueLength starts here
            var itemCount = reader.ReadInt32();
            var items = serializer.ReadSet(reader, prop.ItemType, itemCount);
            prop.Items.AddRange(items);
            return prop;
        }

        public override void SerializeProp(UEArrayProperty prop, BinaryWriter writer, PropertySerializer serializer)
        {
            /*writer.WriteUEString(prop.ItemType);
            writer.Write(false); //terminator
            writer.WriteInt32(prop.Count);

            for (int i=0; i<prop.Items.Count; i++)
            {
                var propItem = prop.Items[i];
                if (i == 0)
                {
                    serializer.Write(propItem, writer);
                }
                else
                {
                    serializer.WriteItem(propItem, writer);
                }
            }*/
            
        }
    }
}