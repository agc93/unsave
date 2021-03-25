using System.Collections.Generic;
using System.IO;
using UnSave.Types;

namespace UnSave.Serialization
{
    public interface IUnrealCollectionPropertySerializer
    {
        IEnumerable<string> Types { get; }
        IEnumerable<IUnrealProperty> Deserialize(string name, string type, long valueLength, int count, BinaryReader reader, PropertySerializer serializer);
        // void Serialize(IEnumerable<IUnrealProperty> prop, BinaryWriter writer, PropertySerializer serializer);
    }
}