using System.Text.Json;
using UnSave.Types;

namespace UnSave.Json.Properties
{
    public class FloatPropertyJsonConverter : UnrealPropertyJsonConverter<UEFloatProperty>
    {
        public override void ReadPropertyValue(ref UEFloatProperty prop, JsonDocument json)
        {
            prop.Value = json.GetValue(e => e.GetSingle());
        }

        public override void WritePropertyValue(ref UEFloatProperty prop, Utf8JsonWriter writer)
        {
            writer.WriteNumber("Value", prop.Value);
        }
    }
}