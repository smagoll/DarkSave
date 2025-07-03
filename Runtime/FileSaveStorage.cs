using System.IO;
using UnityEngine;

namespace DarkSave.Runtime
{
    /// <summary>
    /// Interface for implementing a data storage backend for saving and loading serialized data.
    /// </summary>
    public interface ISaveStorage
    {
        /// <summary>
        /// Saves the provided data string to storage.
        /// </summary>
        /// <param name="data">The serialized data to store.</param>
        void SaveData(string data);

        /// <summary>
        /// Loads the stored data string from storage.
        /// </summary>
        /// <returns>The loaded serialized data.</returns>
        string LoadData();

        /// <summary>
        /// Checks if data already exists in the storage.
        /// </summary>
        /// <returns>True if data exists; otherwise, false.</returns>
        bool HasData();
    }

    /// <summary>
    /// Implementation of ISaveStorage that stores save data in a file on disk.
    /// </summary>
    public class FileSaveStorage : ISaveStorage
    {
        private string _saveName;
        private readonly string _saveFolder;
        private readonly string _saveExtension = ".save";

        /// <summary>
        /// Full file path to the save file.
        /// </summary>
        private string FilePath => GetFilePath();

        /// <summary>
        /// Creates a new FileSaveStorage instance with a given file name and optional folder path.
        /// </summary>
        /// <param name="saveName">The name of the save file (without extension).</param>
        /// <param name="saveFolder">Optional custom folder to save the file in. Defaults to Application.persistentDataPath.</param>
        public FileSaveStorage(string saveName, string saveFolder = null)
        {
            _saveFolder = saveFolder ?? Application.persistentDataPath;
            _saveName = saveName;
        }

        /// <summary>
        /// Constructs the full path to the save file including folder, name, and extension.
        /// </summary>
        /// <returns>The full file path to the save file.</returns>
        private string GetFilePath()
        {
            if (!_saveName.EndsWith(_saveExtension))
                _saveName += _saveExtension;

            return Path.Combine(_saveFolder, _saveName);
        }

        /// <summary>
        /// Saves the provided serialized data to a file on disk.
        /// </summary>
        /// <param name="data">Serialized string data to save.</param>
        public void SaveData(string data)
        {
            File.WriteAllText(FilePath, data);
        }

        /// <summary>
        /// Loads the serialized data from the save file.
        /// </summary>
        /// <returns>The string content of the saved file.</returns>
        public string LoadData()
        {
            return File.ReadAllText(FilePath);
        }

        /// <summary>
        /// Checks whether the save file exists at the expected location.
        /// </summary>
        /// <returns>True if the save file exists; otherwise, false.</returns>
        public bool HasData()
        {
            return File.Exists(FilePath);
        }
    }
}
