using DarkSave.Runtime;

namespace DarkSave.Tests.Mocks
{
    public class MockSerializer : ISerializer
    {
        public string LastSerialized;
        public object LastDeserialized;

        public string Serialize<T>(T obj)
        {
            LastSerialized = $"Serialized:{obj}";
            return LastSerialized;
        }

        public T Deserialize<T>(string json) where T : new()
        {
            var obj = new T();
            LastDeserialized = obj;
            return obj;
        }
    }
}