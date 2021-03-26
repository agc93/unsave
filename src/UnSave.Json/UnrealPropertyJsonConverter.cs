using System;
using System.Collections.Generic;
using System.Text.Json;
using UnSave.Types;

namespace UnSave.Json
{
    public abstract class UnrealPropertyJsonConverter<T> : IUnrealPropertyJsonConverter where T : IUnrealProperty, new()
    {
        public IEnumerable<string> Types => new[] {new T().Type};
        public abstract void ReadPropertyValue(ref T prop, JsonDocument json);
        public abstract void WritePropertyValue(ref T prop, Utf8JsonWriter writer);
        public void ReadProperty(ref IUnrealProperty baseProp, JsonDocument json)
        {
            if (baseProp is T prop)
            {
                ReadPropertyValue(ref prop, json);
                return;
            }

            throw new FormatException("Incorrect property type");
        }

        public void WriteProperty(ref IUnrealProperty baseProp, Utf8JsonWriter writer)
        {
            throw new NotImplementedException();
        }

        public IUnrealProperty Initialize()
        {
            return new T();
        }

        protected T GetValue<T>(JsonDocument doc, Func<JsonElement, T> parseFunc)
        {
            var prop = doc.RootElement.GetProperty("Value");
            return parseFunc(prop);
        }
    }
}