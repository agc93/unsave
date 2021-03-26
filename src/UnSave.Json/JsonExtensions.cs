using System;
using System.Text.Json;

namespace UnSave.Json
{
    public static class JsonExtensions
    {
        public static T GetValue<T>(this JsonDocument doc, Func<JsonElement, T> parseFunc, string name = "Value")
        {
            var prop = doc.RootElement.GetProperty(name);
            return parseFunc(prop);
        }
    }
}