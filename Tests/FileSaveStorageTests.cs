using System.IO;
using DarkSave.Runtime;
using NUnit.Framework;
using UnityEngine;

namespace DarkSave.Tests
{
    public class FileSaveStorageTests
    {
        private string tempFolder;
        private string saveName = "testSave";

        [SetUp]
        public void Setup()
        {
            tempFolder = Path.Combine(Application.temporaryCachePath, "FileSaveStorageTests");
            if (!Directory.Exists(tempFolder))
                Directory.CreateDirectory(tempFolder);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(tempFolder))
                Directory.Delete(tempFolder, true);
        }

        [Test]
        public void SaveData_CreatesFileWithCorrectContent()
        {
            var storage = new FileSaveStorage(saveName, tempFolder);
            string testData = "Hello, world!";

            storage.SaveData(testData);

            string expectedFilePath = Path.Combine(tempFolder, saveName + ".save");
            Assert.IsTrue(File.Exists(expectedFilePath), "Файл не создан");
            string content = File.ReadAllText(expectedFilePath);
            Assert.AreEqual(testData, content, "Содержимое файла не совпадает");
        }

        [Test]
        public void LoadData_ReturnsSavedContent()
        {
            var storage = new FileSaveStorage(saveName, tempFolder);
            string testData = "Load me!";

            storage.SaveData(testData);

            string loadedData = storage.LoadData();
            Assert.AreEqual(testData, loadedData);
        }

        [Test]
        public void HasData_ReturnsTrue_WhenFileExists()
        {
            var storage = new FileSaveStorage(saveName, tempFolder);
            storage.SaveData("some data");
            Assert.IsTrue(storage.HasData());
        }

        [Test]
        public void HasData_ReturnsFalse_WhenFileDoesNotExist()
        {
            var storage = new FileSaveStorage(saveName, tempFolder);
            Assert.IsFalse(storage.HasData());
        }
    }
}