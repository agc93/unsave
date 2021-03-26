using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnSave.Serialization;
using UnSave.Types;

namespace UnSave.Json
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class UnrealPropertyNameAttribute : Attribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        public UnrealPropertyNameAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }

        public string PropertyName { get; set; }
    }

    //for now, this is the real one (famous last words)
    public class UnrealPropertyConverter : JsonConverter<IUnrealProperty>
    {
        private List<IUnrealPropertyJsonConverter> _converters;

        public UnrealPropertyConverter(IEnumerable<IUnrealPropertyJsonConverter> converters)
        {
            _converters = converters.ToList();
        }
        private JsonConverter GetValueConverter(string propName, JsonSerializerOptions options)
        {
            foreach (var jsonConverter in options.Converters)
            {
                var attr = (UnrealPropertyNameAttribute) Attribute.GetCustomAttribute(jsonConverter.GetType(),
                    typeof(UnrealPropertyNameAttribute));
                if (attr != null && attr.PropertyName == propName)
                {
                    return jsonConverter;
                }
            }
            return null;
        }
        public override IUnrealProperty? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Expected StartObject token");
            
            var value = JsonDocument.ParseValue(ref reader);
            if (!value.RootElement.TryGetProperty("Type", out var typeProp))
            {
                throw new FormatException("Couldn't find Type!");
            }

            var propConverter = _converters.FirstOrDefault(c => c.Types.Contains(typeProp.GetString()));
            var prop = propConverter.Initialize();
            prop.Address = value.RootElement.GetProperty("Address").GetString();
            prop.Name = value.RootElement.GetProperty("Name").GetString();
            prop.ValueLength = value.RootElement.GetProperty("ValueLength").GetInt64();
            propConverter.ReadProperty(ref prop, value);
            return prop;
        }

        public override void Write(Utf8JsonWriter writer, IUnrealProperty value, JsonSerializerOptions options)
        {
            var propConverter = _converters.FirstOrDefault(c => c.Types.Contains(value.GetType().ToString()));
            writer.WriteString("Type", value.Type);
            propConverter.WriteProperty(ref value, writer);
            writer.WriteString("Name", value.Name);
            writer.WriteNumber(nameof(value.ValueLength), value.ValueLength);
            writer.WriteString(nameof(value.Address), value.Address);
        }
    }

    public abstract class GenericUnrealPropertyJsonConverter<T> : JsonConverter<T> where T : IUnrealProperty, new()
    {
        public abstract string Type { get; }
        protected abstract void ReadProperty(ref T prop, string propName, Utf8JsonReader reader);
        // public abstract Action<T> ReadProperty(string propName, Utf8JsonReader reader);
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Expected StartObject token");

            var message = new T();
	
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return message;
		
                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException("Expected PropertyName token");

                var propName = reader.GetString();
                reader.Read();

                ReadProperty(ref message, propName, reader);
            }

            throw new JsonException("Expected EndObject token");
        }
    }
}
