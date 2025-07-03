# DarkSave

**DarkSave** is a flexible and extensible save system for Unity.  
It allows you to easily save and load serializable data with support for custom serializers, storage systems, and converters.  
An integrated editor window is also provided to inspect and modify save files directly within the Unity Editor.

---

## 🔧 Features

- ✅ Generic save system based on a serializable data class.
- 💾 File-based storage by default (can be replaced).
- 🔁 Auto-saving support with adjustable frequency.
- 👀 Editor window for viewing and modifying save data in real time.
- 🔌 Plug-in architecture for custom:
  - Serializers
  - Storage backends
  - JSON converters (e.g. `Vector3`, `Color`)
- ❗ Runtime validation and type checking.

---

## 📦 Installation

Download the `DarkSave.unitypackage` file and import it into your Unity project via  
**Assets → Import Package → Custom Package...**

---

## 📥 Installation via Git

You can install **Dark Save** directly in your Unity project using the Package Manager with the Git URL:

1. Open **Unity Editor**.
2. Go to **Window → Package Manager**.
3. Click the **+** button (top left).
4. Choose **Add package from Git URL...**
5. Enter the repository URL: https://github.com/smagoll/darksave.git

## 🚀 Quick Start

### 1. Create your save data class

```csharp
[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int level;
    public float experience;
    public Vector3 position;
}
```

> ⚠️ Your class **must** be marked with `[System.Serializable]`.

---

### 2. Initialize the SaveSystem

```csharp
using DarkSave.Runtime;

public class GameManager : MonoBehaviour
{
    private SaveSystem<PlayerData> _saveSystem;

    private void Awake()
    {
        _saveSystem = new SaveSystem<PlayerData>();
    }

    private void Start()
    {
        var data = _saveSystem.Data;
        Debug.Log($"Loaded Player: {data.playerName}");
    }

    private void OnApplicationQuit()
    {
        _saveSystem.Save();
    }
}
```

---

### 3. Access and modify data

```csharp
_saveSystem.Data.level++;
_saveSystem.Data.position = new Vector3(1, 0, 0);
_saveSystem.Save();
```

---

## 💡 Auto Save

```csharp
_saveSystem.EnableAutoSave(10f); // saves every 10 seconds
_saveSystem.DisableAutoSave();   // disables auto save
```

---

## 🧩 Custom Serializer

To create your own serializer:

```csharp
public class MyJsonSerializer : ISerializer
{
    public string Serialize<T>(T obj) { ... }
    public T Deserialize<T>(string json) where T : new() { ... }
}
```

Usage:

```csharp
var mySerializer = new MyJsonSerializer();
var system = new SaveSystem<PlayerData>(null, mySerializer);
```

---

## 💾 Custom Storage

Create your own implementation of `ISaveStorage`:

```csharp
public class CloudStorage : ISaveStorage
{
    public void SaveData(string data) { /* upload to server */ }
    public string LoadData() { /* download from server */ }
    public bool HasData() { /* check existence */ }
}
```

Usage:

```csharp
var storage = new CloudStorage();
var system = new SaveSystem<PlayerData>(storage);
```

---

## 🧱 Add Custom JSON Converter

You can extend the JSON support with your own converter:

```csharp
public class MyCustomConverter : JsonConverter
{
    public override bool CanConvert(Type objectType) { ... }
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) { ... }
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) { ... }
}
```

Register it:

```csharp
var serializer = new NewtonsoftSerializer();
serializer.AddConverter(new MyCustomConverter());

var system = new SaveSystem<PlayerData>(null, serializer);
```

---

## 🛠 Editor Integration

A built-in Unity Editor window is available to view and modify saved data directly.

### Open it via:

**Menu → Tools → DarkSave Viewer**

From the window you can:

- See current saved data
- Edit values in real time
- Save/load/reset the file

---

## ✅ Example Save File (JSON)

```json
{
  "playerName": "Alice",
  "level": 5,
  "experience": 42.0,
  "position": {
    "x": 3,
    "y": 0,
    "z": 2
  }
}
```

---

## 🧪 Tips

- Ensure your data class and its fields are fully serializable (no Unity components like `GameObject` or `Transform`).
- Use `[System.Serializable]` and prefer primitive fields or supported types like `Vector3`, `Color`, etc.
- You can have multiple `SaveSystem<T>` for different types (e.g. `SettingsData`, `GameData`, etc.).

---

## 📂 Folder Structure

```
DarkSave/
├── Runtime/
│   ├── SaveSystem.cs
│   ├── ISaveSystem.cs
│   ├── ISaveStorage.cs
│   ├── FileSaveStorage.cs
│   ├── ISerializer.cs
│   ├── NewtonsoftSerializer.cs
│   └── Converters/
├── Editor/
│   └── SaveSystemEditorWindow.cs
```

---

## 📃 License

MIT License (or your own license terms here)
