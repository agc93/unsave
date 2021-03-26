using System.Text.Json;
using UnSave.Types;

namespace UnSave.Json.Properties
{
    public class IntPropertyJsonConverter : UnrealPropertyJsonConverter<UEIntProperty>
    {
        public override void ReadPropertyValue(ref UEIntProperty prop, JsonDocument json)
        {
            prop.Value = json.GetValue(e => e.GetInt32());
        }

        public override void WritePropertyValue(ref UEIntProperty prop, Utf8JsonWriter writer)
        {
            writer.WriteNumber("Value", prop.Value);
        }
    }
}