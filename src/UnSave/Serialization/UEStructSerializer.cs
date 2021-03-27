using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnSave.Types;

namespace UnSave.Serialization
{
    public class UEStructSerializer : IUnrealPropertySerializer, IUnrealCollectionPropertySerializer
    {
        public UEStructSerializer(IEnumerable<IUnrealStructSerializer> structTypes)
        {
            Serializers = structTypes;
        }

        private IEnumerable<IUnrealStructSerializer> Serializers { get; set; }
        // private UEStructProperty _structProp;

        public IEnumerable<string> Types => new[] {"StructProperty"};
        public IEnumerable<IUnrealProperty> Deserialize(string name, string baseType, long valueLength, int count, BinaryReader reader, PropertySerializer serializer)
        {
            var itemType = reader.ReadUEString(); //should always be StructProperty
            var id = new Guid(reader.ReadBytes(16));
            if (id != Guid.Empty)
                throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 16:x8}. Expected struct ID {Guid.Empty}, but was {id}");

            var terminator = reader.ReadByte();
            if (terminator != 0)
                throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected terminator (0x00), but was (0x{terminator:x2})");
            
            
            if (count == 0)
            {
                yield return new UEGenericStructProperty(serializer) {_structType = itemType};
            }
            else
            {
                for (var i = 0; i < count; i++)
                {
                    yield return ReadStructValue(name, itemType, reader, valueLength, serializer);
                    
                }
            }
        }
        
        public IUnrealProperty Deserialize(string name, string baseType, long valueLength, BinaryReader reader, PropertySerializer serializer)
        {
            
            var itemType = reader.ReadUEString(); //should be the actual struct property type
            if (itemType == "None")
            {
                return new UENoneProperty() {Name = name};
            }
            var id = new Guid(reader.ReadBytes(16));
            if (id != Guid.Empty)
                throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 16:x8}. Expected struct ID {Guid.Empty}, but was {id}");

            var terminator = reader.ReadByte();
            if (terminator != 0)
                throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected terminator (0x00), but was (0x{terminator:x2})");
            try
            {
                var result = serializer.ReadItem(reader, itemType, (int) valueLength, name);
                return result;
            }
            catch (FormatException)
            {
                //ignored
            }
            var structProp = ReadStructValue(name, itemType, reader, valueLength, serializer);
            return structProp;
        }

        public void Serialize(IEnumerable<IUnrealProperty> props, string itemType, BinaryWriter writer, PropertySerializer serializer)
        {
            writer.WriteUEString(props.First().Name);
            writer.WriteUEString(itemType);
            writer.Write(props.First().ValueLength);
            writer.WriteUEString((props.First() as UEStructProperty).StructType);
            writer.Write(Guid.Empty.ToByteArray());
            writer.Write(false); //terminator
            foreach (var structProp in props.Cast<UEStructProperty>())
            {
                /*try
                {
                    serializer.WriteItem(structProp, itemType, writer);
                    continue;
                }
                catch (FormatException)
                {
                    //ignored
                }*/
                WriteStructValue(structProp, writer, serializer);
            }
        }

        public void Serialize(IUnrealProperty prop, BinaryWriter writer, PropertySerializer serializer)
        {
            var structProp = prop as UEStructProperty;
            writer.WriteUEString(structProp.StructType);
            writer.Write(Guid.Empty.ToByteArray());
            writer.Write(false);
            try
            {
                serializer.WriteItem(structProp, structProp.StructType, writer);
                return;
            }
            catch (FormatException)
            {
                //ignored
            }
            WriteStructValue(structProp, writer, serializer);
            
        }

        private void WriteStructValue(UEStructProperty baseProp, BinaryWriter writer, PropertySerializer propSerializer)
        {
            // var typeSerializer = Serializers.FirstOrDefault(s => s.SupportsType(prop.StructType)) ?? new UEGenericStructProperty(propSerializer) {_structType = prop.StructType};
            
            var structProperty = baseProp as UEGenericStructProperty;
            foreach (var prop in structProperty.Properties)
            {
                propSerializer.Write(prop, writer);
            }

        }
        
        protected UEStructProperty ReadStructValue(string name, string type, BinaryReader reader, long valueLength,
            PropertySerializer propSerializer)
        {
            var result = new UEGenericStructProperty() {_structType =  type};
            var itemOffset = reader.BaseStream.Position;
            while (propSerializer.Read(reader) is IUnrealProperty prop)
            {
                result.Properties.Add(prop);
                if (prop is UENoneProperty)
                {
                    break;
                }
            }
            result.ValueLength = valueLength;
            result.Address = $"0x{ itemOffset :x8}";
            result.Name = name;
            return result;
        }
    }
}