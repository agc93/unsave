using System;
using System.IO;

namespace UnSave.Types
{
    public interface IUnrealProperty
    {
        public string Name { get; set; }
        public string Type { get; }
        public long ValueLength { get; set; }
        public string Address { get; set; }

        /*public void Read(string name, string baseType, long valueLength, BinaryReader reader,
            PropertySerializer serializer) => throw new NotImplementedException();

        public void Write(BinaryWriter writer, PropertySerializer serializer) => throw new NotImplementedException();*/
    }
}