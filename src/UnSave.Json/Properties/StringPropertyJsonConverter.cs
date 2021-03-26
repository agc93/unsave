using System.Text.Json;
using UnSave.Types;

namespace UnSave.Json.Properties
{
    public class StringPropertyJsonConverter : UnrealPropertyJsonConverter<UEStringProperty>
    {
        public override void ReadPropertyValue(ref UEStringProperty prop, JsonDocument json)
        {
            prop.Value = json.RootElement.GetProperty("Value").GetString();
        }

        public override void WritePropertyValue(ref UEStringProperty prop, Utf8JsonWriter writer)
        {
            writer.WriteString(nameof(prop.Value), prop.Value);
        }
    }
}