using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Globalization;

namespace AppCenterEditor
{
#if UNITY_5_3_OR_NEWER
    [CreateAssetMenu(fileName = "AppCenterEditorPrefsSO", menuName = "App Center/Make Prefs SO", order = 1)]
#endif
    public class AppCenterEditorPrefsSO : ScriptableObject
    {
        public const string InstanceKey = "AppCenterEditorPrefsSOKey";
        public string SdkPath;
        public string EdExPath;
        public bool PanelIsShown;
        public int curMainMenuIdx;
        [SerializeField]
        private string _latestSdkVersion;
        [SerializeField]
        private string _latestEdExVersion;
        [SerializeField]
        private string _latestEdExVersionCheckDateString;
        [SerializeField]
        private string _lastSdkVersionCheckDateString;
        private DateTime _lastSdkVersionCheck;
        private DateTime _lastEdExVersionCheck;
        private static AppCenterEditorPrefsSO _instance;

        public string EdSet_latestSdkVersion
        {
            get { return _latestSdkVersion; }
            set { _latestSdkVersion = value; _lastSdkVersionCheck = DateTime.UtcNow; _lastSdkVersionCheckDateString = _lastSdkVersionCheck.ToString(CultureInfo.InvariantCulture); }
        }
        public string EdSet_latestEdExVersion
        {
            get { return _latestEdExVersion; }
            set { _latestEdExVersion = value; _lastEdExVersionCheck = DateTime.UtcNow; _latestEdExVersionCheckDateString = _lastEdExVersionCheck.ToString(CultureInfo.InvariantCulture); }
        }
        public DateTime EdSet_lastSdkVersionCheck { get { return string.IsNullOrEmpty(_lastSdkVersionCheckDateString) ? _lastSdkVersionCheck : DateTime.Parse(_lastSdkVersionCheckDateString); } }
        public DateTime EdSet_lastEdExVersionCheck { get { return string.IsNullOrEmpty(_latestEdExVersionCheckDateString) ? _lastEdExVersionCheck : DateTime.Parse(_latestEdExVersionCheckDateString); } }

        public static AppCenterEditorPrefsSO Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                var settingsList = Resources.LoadAll<AppCenterEditorPrefsSO>("AppCenterEditorPrefsSO");
                if (settingsList.Length == 1)
                    _instance = settingsList[0];
                if (_instance != null)
                {
                    if (PlayerPrefs.HasKey(InstanceKey))
                    {
                        JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(InstanceKey), _instance);
                    }
                    return _instance;
                }
                _instance = CreateInstance<AppCenterEditorPrefsSO>();
                if (!Directory.Exists(Path.Combine(Application.dataPath, "AppCenterEditorExtensions/Editor/Resources")))
                    Directory.CreateDirectory(Path.Combine(Application.dataPath, "AppCenterEditorExtensions/Editor/Resources"));
                AssetDatabase.CreateAsset(_instance, "Assets/AppCenterEditorExtensions/Editor/Resources/AppCenterEditorPrefsSO.asset");
                AssetDatabase.SaveAssets();
                EdExLogger.LoggerInstance.LogWithTimeStamp("Created missing AppCenterEditorPrefsSO file");
                return _instance;
            }
        }

        public static void Save()
        {
            EditorUtility.SetDirty(_instance);
            AssetDatabase.SaveAssets();
        }
    }
}
