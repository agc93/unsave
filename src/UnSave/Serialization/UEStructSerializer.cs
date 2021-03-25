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
        private UEStructProperty _structProp;

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
            _structProp = ReadStructValue(name, itemType, reader, valueLength, serializer);
            return _structProp;
        }

        public void Serialize(IUnrealProperty prop, BinaryWriter writer, PropertySerializer serializer)
        {
            writer.WriteUEString(_structProp.StructType);
            writer.Write(Guid.Empty.ToByteArray());
            writer.Write(false);
            var typeSerializer = Serializers.FirstOrDefault(s => s.SupportsType(_structProp.StructType)) ?? new UEGenericStructProperty(serializer) {_structType = _structProp.StructType};
            typeSerializer.SerializeStruct(writer);
            
        }
        
        protected UEStructProperty ReadStructValue(string name, string type, BinaryReader reader, long valueLength,
            PropertySerializer propSerializer)
        {
            UEStructProperty result;
            var itemOffset = reader.BaseStream.Position;
            var typeSerializer = Serializers.FirstOrDefault(s => s.SupportsType(type)) ?? new UEGenericStructProperty(propSerializer) {_structType = type};
            result = typeSerializer.DeserializeStruct(reader) as UEStructProperty;
            result.ValueLength = valueLength;
            result.Address = $"0x{ itemOffset :x8}";
            result.Name = name;
            return result;
        }
    }
}