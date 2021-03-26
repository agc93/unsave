using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace UnSave.Json
{
    public class CustomFormatDataConverter : JsonConverter<CustomFormatData>
    {
        public override CustomFormatData? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Expected StartObject token");
            var entryConverter = options.GetConverter(typeof(CustomFormatDataEntry));
            var data = new CustomFormatData();
	
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return data;
		
                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException("Expected PropertyName token");

                var propName = reader.GetString();
                reader.Read();

                switch(propName)
                {
                    case nameof(data.Count):
                        data.Count = reader.GetInt32();
                        break;
                    case nameof(data.Entries):
                        var entriesConv = (JsonConverter<IEnumerable<CustomFormatDataEntry>>)options.GetConverter(typeof(IEnumerable<CustomFormatDataEntry>));
                        var result = entriesConv.Read(ref reader, typeof(IEnumerable<CustomFormatDataEntry>), options);
                        data.Entries = result.ToArray();
                        break;
                }
            }

            throw new JsonException("Expected end token");
        }

        public override void Write(Utf8JsonWriter writer, CustomFormatData value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}