#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using DarkSave.Runtime;
using UnityEditor;
using UnityEngine;

namespace DarkSave.Editor
{
    public class SaveSystemEditorWindow : EditorWindow
    {
        private ISaveSystem _saveSystem;
        private string _fileText = string.Empty;
        private Vector2 _scroll;
        private string _selectedPath = string.Empty;
        private string[] _filePaths = Array.Empty<string>();
        private int _selectedIndex;
        private readonly Dictionary<string, bool> _cache = new();
        private readonly string[] _extensions = {"*.save"};
        
        private const string LastSelectedPathKey = "DarkSaveEditor_LastSelectedSavePath";

        [MenuItem("Dark/Save System/Editor Window")]
        public static void ShowWindow() =>
            GetWindow<SaveSystemEditorWindow>(false, "Save System", false);

        public void SetSaveSystem(ISaveSystem system)
        {
            if (_saveSystem != system) _cache.Clear();
            _saveSystem = system;
            RefreshFiles();
            RefreshFileText();
        }

        private void RefreshFiles()
        {
            var roots = new[] 
            {
                Application.dataPath,
                Application.streamingAssetsPath,
                Application.persistentDataPath,
                Directory.GetParent(Application.dataPath)?.FullName
            };

            _filePaths = roots
                .Where(Directory.Exists)
                .SelectMany(dir => _extensions.SelectMany(ext => SafeGetFiles(dir, ext)))
                .Distinct()
                .OrderBy(p => p)
                .Where(path => _saveSystem == null || IsValid(path))
                .ToArray();
        }

        private IEnumerable<string> SafeGetFiles(string dir, string pattern)
        {
            try { return Directory.GetFiles(dir, pattern, SearchOption.AllDirectories); }
            catch { return Array.Empty<string>(); }
        }

        private bool IsValid(string path)
        {
            if (_cache.TryGetValue(path, out var ok)) return ok;
            try
            {
                var json = File.ReadAllText(path);
                ok = _saveSystem?.IsValid(json) ?? true;
            }
            catch { ok = false; }

            return _cache[path] = ok;
        }

        private void RefreshFileText()
        {
            if (_saveSystem != null && _selectedIndex == 0)
                _fileText = _saveSystem.GetFileText();
            else if (File.Exists(_selectedPath))
                _fileText = File.ReadAllText(_selectedPath);
            else _fileText = string.Empty;

            Repaint();
        }

        private void OnGUI()
        {
            EditorGUILayout.HelpBox(
                !string.IsNullOrEmpty(_selectedPath)
                    ? $"Editing: {Path.GetFileName(_selectedPath)}"
                    : "No save files found.",
                MessageType.Info);

            DrawSelection();
            EditorGUILayout.Space();

            if ((_saveSystem == null && string.IsNullOrEmpty(_selectedPath)))
            {
                EditorGUILayout.HelpBox("Select SaveSystem or file.", MessageType.Info);
                return;
            }

            EditorGUILayout.LabelField("JSON Editor", EditorStyles.boldLabel);
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            _fileText = EditorGUILayout.TextArea(_fileText, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Apply")) Apply();
            if (GUILayout.Button("Refresh")) RefreshFileText();
            if (GUILayout.Button("Backup")) Backup();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawSelection()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh List", GUILayout.Width(100))) RefreshFiles();
            EditorGUILayout.EndHorizontal();

            var names = _filePaths.Select(Path.GetFileName).ToArray();

            int newIndex = EditorGUILayout.Popup(_selectedIndex, names);

            if (newIndex != _selectedIndex)
            {
                _selectedIndex = newIndex;
                _selectedPath = _filePaths.Length > 0
                    ? _filePaths[Mathf.Clamp(_selectedIndex, 0, _filePaths.Length - 1)]
                    : string.Empty;

                RefreshFileText();
                EditorPrefs.SetString(LastSelectedPathKey, _selectedPath);
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Browse...", GUILayout.Width(80)))
                Browse();
            if (!string.IsNullOrEmpty(_selectedPath) && GUILayout.Button("Show in Explorer", GUILayout.Width(120)))
                EditorUtility.RevealInFinder(_selectedPath);
        }


        private void Browse()
        {
            var path = EditorUtility.OpenFilePanel("Select Save File", 
                Application.persistentDataPath, "save");
            if (!string.IsNullOrEmpty(path))
            {
                _selectedPath = path;
                _selectedIndex = 0;
                RefreshFileText();
                EditorPrefs.SetString(LastSelectedPathKey, _selectedPath);
            }
        }

        private void Apply()
        {
            try
            {
                if (_saveSystem != null && _selectedIndex == 0)
                {
                    _saveSystem.SetData(_fileText);
                    _saveSystem.Save();
                }
                else if (!string.IsNullOrEmpty(_selectedPath))
                {
                    File.WriteAllText(_selectedPath, _fileText);
                    AssetDatabase.Refresh();
                    if (_saveSystem != null) _saveSystem.SetData(_fileText);
                }
                Debug.Log("[SaveEditor] Applied.");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveEditor] Apply failed: {e.Message}");
            }
        }

        private void Backup()
        {
            var dir = Application.persistentDataPath;
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var backupFile = Path.Combine(dir, $"backup_{timestamp}.save");
            try
            {
                if (File.Exists(_selectedPath))
                    File.Copy(_selectedPath, backupFile);
                Debug.Log($"[SaveEditor] Backup created: {backupFile}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveEditor] Backup failed: {e.Message}");
            }
        }
        
        private void OnFocus()
        {
            RefreshFiles();
            RefreshFileText();
        }
        
        private void OnEnable()
        {
            RefreshFiles();
            
            if (_filePaths.Length > 0)
            {
                string lastPath = EditorPrefs.GetString(LastSelectedPathKey, "");

                if (!string.IsNullOrEmpty(lastPath) && File.Exists(lastPath))
                {
                    _selectedPath = lastPath;
                    _selectedIndex = Array.IndexOf(_filePaths, _selectedPath);
                    if (_selectedIndex < 0) _selectedIndex = 0;
                }
                else
                {
                    _selectedPath = _filePaths[0];
                    _selectedIndex = 0;
                }

                RefreshFileText();
            }
        }
    }
}
#endif
