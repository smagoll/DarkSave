using System;
using System.Timers;
using DarkSave.Editor;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace DarkSave.Runtime
{
    /// <summary>
    /// Generic save system for Unity that handles serialization, persistence, and optional auto-save for a given data type.
    /// </summary>
    public class SaveSystem<T> : ISaveSystem where T : class, new()
    {
        private ISaveStorage _storage;
        private ISerializer _serializer = new NewtonsoftSerializer();
        private Timer _autoSaveTimer;
        private AutoSaveManager _autoSaveManager;

        /// <summary>
        /// Current data object of type T. Serialized and saved by the system.
        /// </summary>
        public T Data { get; private set; }

        /// <summary>
        /// Indicates whether auto-saving is currently enabled.
        /// </summary>
        public bool AutoSaveEnabled => _autoSaveManager.AutoSaveEnabled;

        /// <summary>
        /// Creates a SaveSystem using default storage and serializer.
        /// </summary>
        public SaveSystem()
        {
            Initialize();
        }

        /// <summary>
        /// Creates a SaveSystem with the specified storage.
        /// </summary>
        /// <param name="storage">Custom save storage implementation.</param>
        public SaveSystem(ISaveStorage storage)
        {
            Initialize(storage);
        }

        /// <summary>
        /// Creates a SaveSystem with the specified storage and serializer.
        /// </summary>
        /// <param name="storage">Custom save storage implementation.</param>
        /// <param name="serializer">Custom serializer implementation.</param>
        public SaveSystem(ISaveStorage storage, ISerializer serializer)
        {
            Initialize(storage, serializer);
        }

        private void Initialize(ISaveStorage storage = null, ISerializer serializer = null)
        {
            if (!IsValidClass()) return;

            _serializer = serializer ?? new NewtonsoftSerializer();
            _storage = storage ?? new FileSaveStorage("Saves");
            _autoSaveManager = new AutoSaveManager(this);
            Data = null;
            Load();

            Application.quitting += Save;
        }

        /// <summary>
        /// Saves the current data to the storage. Creates a new instance if data is null.
        /// </summary>
        public void Save()
        {
            Data ??= new T();

            var json = _serializer.Serialize(Data);
            _storage.SaveData(json);

#if UNITY_EDITOR
            UpdateEditorWindow();
#endif
        }

        /// <summary>
        /// Loads data from storage. If no data exists, creates a new instance and saves it.
        /// </summary>
        public void Load()
        {
            if (_storage == null)
            {
                Debug.LogError("Storage not initialized");
                Data = new T();
                return;
            }

            if (_storage.HasData())
            {
                string json = _storage.LoadData();
                Data = _serializer.Deserialize<T>(json);
            }
            else
            {
                Data = new T();
                Save();
            }

#if UNITY_EDITOR
            UpdateEditorWindow();
#endif
        }

        /// <summary>
        /// Resets the data by creating a new instance of type T and saving it.
        /// </summary>
        public void Reset()
        {
            Data = new T();
            Save();
        }

        private bool IsValidClass()
        {
            var attr = typeof(T).GetCustomAttributes(typeof(SerializableAttribute), false);

            if (attr.Length == 0)
            {
                Debug.LogError($"[SaveSystem] Type {typeof(T)} must be marked with [System.Serializable]");
                return false;
            }

            if (!SaveSystemUtils.IsSerializableRecursively(typeof(T)))
            {
                Debug.LogError($"[SaveSystem] Type {typeof(T).Name} or its fields are not fully serializable.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if the provided JSON string can be deserialized into a valid object of type T.
        /// </summary>
        /// <param name="json">Serialized JSON string.</param>
        /// <returns>True if the JSON is valid; otherwise, false.</returns>
        public bool IsValid(string json)
        {
            try
            {
                var obj = _serializer.Deserialize<T>(json);
                return obj != null;
            }
            catch (JsonException)
            {
                return false;
            }
        }

        /// <summary>
        /// Enables automatic saving at a specified interval in seconds.
        /// </summary>
        /// <param name="timePeriodic">Interval between auto-saves, in seconds (default is 15).</param>
        public void EnableAutoSave(float timePeriodic = 15f) => _autoSaveManager.Enable(timePeriodic);

        /// <summary>
        /// Disables the automatic saving feature.
        /// </summary>
        public void DisableAutoSave() => _autoSaveManager.Disable();

        private void UpdateEditorWindow()
        {
#if UNITY_EDITOR
            var window = EditorWindow.GetWindow<SaveSystemEditorWindow>();
            window?.SetSaveSystem(this);
#endif
        }

        /// <summary>
        /// Gets the serialized string (e.g., JSON) representing the current data.
        /// </summary>
        string ISaveSystem.GetFileText() => _serializer.Serialize(Data);

        /// <summary>
        /// Gets the current data object.
        /// </summary>
        public object GetData() => Data;

        /// <summary>
        /// Sets the data by deserializing the provided string.
        /// </summary>
        /// <param name="json">Serialized data string.</param>
        void ISaveSystem.SetData(string json) => Data = _serializer.Deserialize<T>(json);

        /// <summary>
        /// Gets the runtime type of the stored data.
        /// </summary>
        /// <returns>The <see cref="Type"/> of the data object.</returns>
        Type ISaveSystem.GetDataType() => typeof(T);

        /// <summary>
        /// Saves the current data to storage.
        /// </summary>
        void ISaveSystem.Save() => Save();
    }

    /// <summary>
    /// Interface for generic save system operations.
    /// </summary>
    public interface ISaveSystem
    {
        /// <summary>
        /// Gets the serialized string (e.g., JSON) representing the current data.
        /// </summary>
        string GetFileText();

        /// <summary>
        /// Gets the current data object.
        /// </summary>
        object GetData();

        /// <summary>
        /// Checks whether the provided data string can be deserialized correctly.
        /// </summary>
        /// <param name="json">Serialized data string.</param>
        /// <returns>True if deserialization is successful; otherwise, false.</returns>
        bool IsValid(string json);

        /// <summary>
        /// Sets the data by deserializing the provided string.
        /// </summary>
        /// <param name="data">Serialized data string.</param>
        void SetData(string data);

        /// <summary>
        /// Gets the runtime type of the stored data.
        /// </summary>
        /// <returns>The <see cref="Type"/> of the data object.</returns>
        Type GetDataType();

        /// <summary>
        /// Saves the current data to storage.
        /// </summary>
        void Save();

        /// <summary>
        /// Loads data from the storage.
        /// </summary>
        void Load();

        /// <summary>
        /// Resets the data to a new instance and saves it.
        /// </summary>
        void Reset();
    }
}
