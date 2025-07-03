using DarkSave.Runtime;
using DarkSave.Tests.Mocks;
using NUnit.Framework;

namespace DarkSave.Tests
{
    [System.Serializable]
    public class TestSaveData
    {
        public int score = 42;
        public string playerName = "Player";
    }

    public class SaveSystemTests
    {
        private SaveSystem<TestSaveData> _saveSystem;
        private MockStorage _mockStorage;
        private MockSerializer _mockSerializer;

        [SetUp]
        public void SetUp()
        {
            _mockStorage = new MockStorage();
            _mockSerializer = new MockSerializer();
            _saveSystem = new SaveSystem<TestSaveData>(_mockStorage, _mockSerializer);
        }

        [Test]
        public void SaveSystem_SavesSerializedDataToStorage()
        {
            _saveSystem.Save();

            Assert.IsTrue(_mockStorage.SavedData.StartsWith("Serialized:"));
        }

        [Test]
        public void SaveSystem_LoadsData_WhenStorageHasData()
        {
            _mockStorage.HasSavedData = true;
            _mockStorage.SaveData("some json");

            _saveSystem.Load();

            Assert.IsNotNull(_saveSystem.Data);
        }

        [Test]
        public void SaveSystem_ResetsData_CreatesNewData()
        {
            _saveSystem.Data.score = 99;

            _saveSystem.Reset();

            Assert.AreEqual(42, _saveSystem.Data.score);
        }

        [Test]
        public void SaveSystem_IsValid_ReturnsTrueForValidJson()
        {
            string fakeJson = "anything";

            bool result = _saveSystem.IsValid(fakeJson);

            Assert.IsTrue(result);
        }
    }
}