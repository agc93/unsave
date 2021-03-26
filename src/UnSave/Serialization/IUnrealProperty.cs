using System.Collections.Generic;
using System.IO;
using UnSave.Types;

namespace UnSave.Serialization
{
    /// <summary>
    /// Among other things, separating the serializer into a different class allows for tweaking the behaviour of built-in types by overriding the serializer.
    /// </summary>
    public interface IUnrealPropertySerializer
    {
        IEnumerable<string> Types { get; }
        IUnrealProperty Deserialize(string name, string type, long valueLength, BinaryReader reader, PropertySerializer serializer);
        void Serialize(IUnrealProperty prop, BinaryWriter writer, PropertySerializer serializer);
    }
}