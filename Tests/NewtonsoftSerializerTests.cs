using DarkSave.Runtime;
using NUnit.Framework;
using UnityEngine;

namespace DarkSave.Tests
{
    public class NewtonsoftSerializerTests
    {
        private NewtonsoftSerializer _serializer;

        [SetUp]
        public void Setup()
        {
            _serializer = new NewtonsoftSerializer();
        }

        private class TestData
        {
            public string Name;
            public int Score;
            public Vector3 Position;
        }

        [Test]
        public void Serialize_ShouldIncludeAllFields()
        {
            var data = new TestData
            {
                Name = "Player",
                Score = 42,
                Position = new Vector3(1, 2, 3)
            };

            string json = _serializer.Serialize(data);
        
            Assert.IsTrue(json.Contains("name"));
            Assert.IsTrue(json.Contains("score"));
            Assert.IsTrue(json.Contains("position"));
        }

        [Test]
        public void Deserialize_ShouldRestoreObject()
        {
            string json = @"
        {
            ""name"": ""Player"",
            ""score"": 42,
            ""position"": {""x"":1,""y"":2,""z"":3}
        }";

            var data = _serializer.Deserialize<TestData>(json);

            Assert.AreEqual("Player", data.Name);
            Assert.AreEqual(42, data.Score);
            Assert.AreEqual(new Vector3(1, 2, 3), data.Position);
        }
    }
}