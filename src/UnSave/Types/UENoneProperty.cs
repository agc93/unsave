using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnSave.Serialization;

namespace UnSave.Types
{
    [DebuggerDisplay("", Name = "{Name}")]
    public sealed class UENoneProperty : UnrealPropertyBase, IUnrealPropertySerializer
    {

        public UENoneProperty() { Name = "None"; }
        public override string Type => null; //this one gets special handling

        public IEnumerable<string> Types => new List<string>();

        public IUnrealProperty Deserialize(string name, string type, long valueLength, BinaryReader reader,
            PropertySerializer serializer)
        {
            throw new System.NotImplementedException();
        }

        public void Serialize(IUnrealProperty prop, BinaryWriter writer, PropertySerializer serializer)
        {
            writer.WriteUEString("None");
        }
    }
}