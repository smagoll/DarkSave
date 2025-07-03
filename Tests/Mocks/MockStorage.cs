using DarkSave.Runtime;

namespace DarkSave.Tests.Mocks
{
    public class MockStorage : ISaveStorage
    {
        public string SavedData { get; private set; } = string.Empty;
        public bool HasSavedData = false;

        public void SaveData(string data) => SavedData = data;
        public string LoadData() => SavedData;
        public bool HasData() => HasSavedData;
    }
}