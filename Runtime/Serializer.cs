using System.Collections.Generic;
using DarkSave.Runtime.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DarkSave.Runtime
{
    /// <summary>
    /// Interface for serialization and deserialization of objects to and from JSON.
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Serializes an object of type T into a JSON string.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="obj">The object instance to serialize.</param>
        /// <returns>Serialized JSON string.</returns>
        string Serialize<T>(T obj);

        /// <summary>
        /// Deserializes a JSON string back into an object of type T.
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize into.</typeparam>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <returns>The deserialized object.</returns>
        T Deserialize<T>(string json) where T : new();
    }

    /// <summary>
    /// Implementation of ISerializer using Newtonsoft.Json with custom settings and converters.
    /// </summary>
    public class NewtonsoftSerializer : ISerializer
    {
        private JsonSerializerSettings _settings;

        /// <summary>
        /// Creates a new NewtonsoftSerializer instance with predefined settings and converters.
        /// </summary>
        public NewtonsoftSerializer()
        {
            SetupSettings();
        }

        /// <summary>
        /// Configures serialization settings including naming strategy and custom type converters.
        /// </summary>
        private void SetupSettings()
        {
            _settings = new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy(),
                    IgnoreSerializableInterface = true
                },
                Converters = new List<JsonConverter>
                {
                    new Vector2Converter(),
                    new Vector3Converter(),
                    new QuaternionConverter(),
                    new ColorConverter()
                },
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }

        /// <summary>
        /// Serializes an object of type T into a JSON string using the configured settings.
        /// </summary>
        /// <typeparam name="T">The type of object to serialize.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>Formatted JSON string.</returns>
        public string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented, _settings);
        }

        /// <summary>
        /// Deserializes a JSON string into an object of type T using the configured settings.
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize into.</typeparam>
        /// <param name="json">The JSON string.</param>
        /// <returns>Deserialized object of type T.</returns>
        public T Deserialize<T>(string json) where T : new()
        {
            return JsonConvert.DeserializeObject<T>(json, _settings);
        }

        /// <summary>
        /// Adds a custom JSON converter to the serializer.
        /// </summary>
        /// <param name="converter">The JSON converter to add.</param>
        public void AddConverter(JsonConverter converter)
        {
            _settings.Converters.Add(converter);
        }
    }
}
