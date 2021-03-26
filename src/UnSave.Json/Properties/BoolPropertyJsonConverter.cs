using System.Text.Json;
using UnSave.Types;

namespace UnSave.Json.Properties
{
    public class BoolPropertyJsonConverter : UnrealPropertyJsonConverter<UEBoolProperty>
    {
        public override void ReadPropertyValue(ref UEBoolProperty prop, JsonDocument json)
        {
            prop.Value = GetValue(json, e => e.GetBoolean());
        }

        public override void WritePropertyValue(ref UEBoolProperty prop, Utf8JsonWriter writer)
        {
            writer.WriteBoolean("Value", prop.Value);
        }
    }
}