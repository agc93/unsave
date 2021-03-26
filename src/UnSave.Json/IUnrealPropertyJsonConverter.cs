using System.Collections.Generic;
using System.Text.Json;
using UnSave.Types;

namespace UnSave.Json
{
    public interface IUnrealPropertyJsonConverter
    {
        public IEnumerable<string> Types { get; }
        public void ReadProperty(ref IUnrealProperty prop, JsonDocument json);
        public void WriteProperty(ref IUnrealProperty prop, Utf8JsonWriter writer);
        public IUnrealProperty Initialize();
    }
}