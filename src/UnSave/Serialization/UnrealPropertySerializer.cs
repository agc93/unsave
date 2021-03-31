using System;
using System.Collections.Generic;
using System.IO;
using UnSave.Types;

namespace UnSave.Serialization
{
    public abstract class UnrealPropertySerializer<T> : IUnrealPropertySerializer where T : IUnrealProperty, new()
    {
        // public virtual IEnumerable<string> Types => new[] { typeof(T).IsAssignableTo(typeof(UEStructProperty)) ? (new T() as UEStructProperty).StructType : new T().Type};
        // public virtual IEnumerable<string> Types => new[] { new T().Type};
        public virtual IEnumerable<string> Types
        {
            get
            {
                
                return new[]
                {
                    typeof(UEStructProperty).IsAssignableFrom(typeof(T))
                        ? (new T() as UEStructProperty).StructType
                        : new T().Type
                };
            }
        }

        public virtual IUnrealProperty Deserialize(string name, string type, long valueLength,
            BinaryReader reader,
            PropertySerializer serializer)
        {
            return DeserializeProp(name, type, valueLength, reader, serializer);
        }

        public virtual void Serialize(IUnrealProperty baseProp, BinaryWriter writer, PropertySerializer serializer)
        {
            if (baseProp is T prop)
            {
                SerializeProp(prop, writer, serializer);
                return;
            }
            throw new FormatException("Incorrect property type!");
        }
        public abstract T DeserializeProp(string name, string type, long valueLength, BinaryReader reader, PropertySerializer serializer);
        public abstract void SerializeProp(T prop, BinaryWriter writer, PropertySerializer serializer);
    }
}