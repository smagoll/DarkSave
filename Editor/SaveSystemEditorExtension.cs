#if UNITY_EDITOR
using UnityEditor;
using DarkSave.Editor;
using DarkSave.Runtime;

namespace DarkSave.Editor
{
    public static class SaveSystemEditorExtension
    {
        public static void UpdateEditorWindow<T>(this SaveSystem<T> saveSystem) where T : class, new()
        {
            var window = EditorWindow.GetWindow<SaveSystemEditorWindow>();
            window?.SetSaveSystem(saveSystem);
        }
    }
}
#endif