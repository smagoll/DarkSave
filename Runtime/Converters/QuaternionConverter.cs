using System;
using Newtonsoft.Json;
using UnityEngine;

namespace DarkSave.Runtime.Converters
{
    public class QuaternionConverter : JsonConverter<Quaternion>
    {
        public override void WriteJson(JsonWriter writer, Quaternion value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("x"); writer.WriteValue(value.x);
            writer.WritePropertyName("y"); writer.WriteValue(value.y);
            writer.WritePropertyName("z"); writer.WriteValue(value.z);
            writer.WritePropertyName("w"); writer.WriteValue(value.w);
            writer.WriteEndObject();
        }

        public override Quaternion ReadJson(JsonReader reader, Type objectType, Quaternion existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            float x = 0, y = 0, z = 0, w = 1;
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    switch ((string)reader.Value)
                    {
                        case "x": reader.Read(); x = Convert.ToSingle(reader.Value); break;
                        case "y": reader.Read(); y = Convert.ToSingle(reader.Value); break;
                        case "z": reader.Read(); z = Convert.ToSingle(reader.Value); break;
                        case "w": reader.Read(); w = Convert.ToSingle(reader.Value); break;
                    }
                }
                else if (reader.TokenType == JsonToken.EndObject) break;
            }
            return new Quaternion(x, y, z, w);
        }
    }

}