using System;
using Newtonsoft.Json;
using UnityEngine;

namespace DarkSave.Runtime.Converters
{
    public class ColorConverter : JsonConverter<Color>
    {
        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("r"); writer.WriteValue(value.r);
            writer.WritePropertyName("g"); writer.WriteValue(value.g);
            writer.WritePropertyName("b"); writer.WriteValue(value.b);
            writer.WritePropertyName("a"); writer.WriteValue(value.a);
            writer.WriteEndObject();
        }

        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            float r = 0, g = 0, b = 0, a = 1;
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    switch ((string)reader.Value)
                    {
                        case "r": reader.Read(); r = Convert.ToSingle(reader.Value); break;
                        case "g": reader.Read(); g = Convert.ToSingle(reader.Value); break;
                        case "b": reader.Read(); b = Convert.ToSingle(reader.Value); break;
                        case "a": reader.Read(); a = Convert.ToSingle(reader.Value); break;
                    }
                }
                else if (reader.TokenType == JsonToken.EndObject) break;
            }
            return new Color(r, g, b, a);
        }
    }

}