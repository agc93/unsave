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
            // reader.BaseStream.Seek(-4, SeekOrigin.Current);
            // if (prop.ItemType == "StructProperty")
            if (true)
            {
                var items = serializer.ReadSet(reader, prop.ItemType, itemCount);
                /*var structName = reader.ReadUEString();
                var itemType = reader.ReadUEString();
                var structValueLength = reader.ReadInt64();
                for (int i = 0; i < itemCount; i++)
                {
                    var itemProp = serializer.ReadItem(reader, prop.ItemType, Convert.ToInt32(structValueLength), structName);
                    prop.Items.Add(itemProp);
                }*/
                prop.Items.AddRange(items);
            }
            else
            {
                for (int i = 0; i < itemCount; i++)
                {
                    var itemProp = serializer.ReadItem(reader, prop.ItemType, -1);
                    prop.Items.Add(itemProp);
                }
            }
            
            /*prop.Items = new IUnrealProperty[prop.Count];
            
            prop.Items = serializer.ReadSet(reader, prop.Count);*/

            /*switch (ItemType)
            {
                case "StructProperty":
                    Items = serializer.ReadSet(reader, Count);
                    break;
                case "ByteProperty":
                    
                    break;
                default:
                {
                    for (var i = 0; i < Count; i++)
                        Items[i] = UESerializer.Deserialize(null, ItemType, -1, reader);
                    break;
                }
            }*/
            return prop;
        }

        public override void SerializeProp(UEArrayProperty prop, BinaryWriter writer, PropertySerializer serializer)
        {
            writer.WriteUEString(prop.ItemType);
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
                    serializer.Write(propItem, writer);
                }
                /*else if (prop is UEStructProperty) { ((UEStructProperty)prop).SerializeStructProp(writer); }
                else { prop.SerializeProp(writer); }*/
            }
        }
    }
}