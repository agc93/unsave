using System.IO;
using UnSave.Types;

namespace UnSave.Serialization
{
    public interface IUnrealStructSerializer
    {
        IUnrealProperty DeserializeStruct(BinaryReader reader);
        void SerializeStruct(BinaryWriter writer);
        bool SupportsType(string type);
    }
}