using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AppCenterEditor
{
    public class AppCenterEditorSDKTools : Editor
    {
        public static bool IsInstalled { get { return CheckIfSomePackagesInstalled(); } }
        public static bool IsFullSDK { get { return CheckIfAllPackagesInstalled(); } }
        public static bool IsInstalling { get; set; }
        public static string LatestSdkVersion { get; private set; }
        public static UnityEngine.Object SdkFolder { get; private set; }
        public static string InstalledSdkVersion { get; private set; }

        private static Type appCenterSettingsType = null;
        private static bool isInitialized; // used to check once, gets reset after each compile
        private static UnityEngine.Object _previousSdkFolderPath;
        private static bool isObjectFieldActive;
        public static bool isSdkSupported = true;
        private static int angle = 0;

        public static void DrawSdkPanel()
        {
            if (!isInitialized)
            {
                //SDK is installed.
                CheckSdkVersion();
                isInitialized = true;
                GetLatestSdkVersion();
                SdkFolder = FindSdkAsset();

                if (SdkFolder != null)
                {
                    AppCenterEditorPrefsSO.Instance.SdkPath = AssetDatabase.GetAssetPath(SdkFolder);
                    // AppCenterEditorDataService.SaveEnvDetails();
                }
            }

            if (IsInstalled)
            {
                ShowSdkInstalledMenu();
            }
            else
            {
                ShowSdkNotInstalledMenu();
            }
        }

        public static void DisplayPackagePanel(AppCenterSDKPackage sdkPackage)
        {
            using (new AppCenterGuiFieldHelper.UnityVertical(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleGray1")))
            {
                using (new AppCenterGuiFieldHelper.UnityHorizontal(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleClear")))
                {
                    GUILayout.FlexibleSpace();
                    if (sdkPackage.IsInstalled)
                    {
                        sdkPackage.ShowPackageInstalledMenu();
                    }
                    else
                    {
                        sdkPackage.ShowPackageNotInstalledMenu();
                    }
                    GUILayout.FlexibleSpace();
                }
            }
        }

        private static void ShowSdkInstalledMenu()
        {
            isObjectFieldActive = SdkFolder == null;

            if (_previousSdkFolderPath != SdkFolder)
            {
                // something changed, better save the result.
                _previousSdkFolderPath = SdkFolder;

                AppCenterEditorPrefsSO.Instance.SdkPath = (AssetDatabase.GetAssetPath(SdkFolder));
                //TODO: check if we need this?
                // AppCenterEditorDataService.SaveEnvDetails();

                isObjectFieldActive = false;
            }

            var labelStyle = new GUIStyle(AppCenterEditorHelper.uiStyle.GetStyle("titleLabel"));
            using (new AppCenterGuiFieldHelper.UnityVertical(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleGray1")))
            {                
                EditorGUILayout.LabelField(string.Format(IsFullSDK ? "SDK {0} is installed" : "Several SDK packages installed", string.IsNullOrEmpty(InstalledSdkVersion) ? Constants.UnknownVersion : InstalledSdkVersion),
                    labelStyle, GUILayout.MinWidth(EditorGUIUtility.currentViewWidth));

                if (!isObjectFieldActive)
                {
                    GUI.enabled = false;
                }
                else
                {
                    EditorGUILayout.LabelField(
                        "An SDK was detected, but we were unable to find the directory. Drag-and-drop the top-level App Center SDK folder below.",
                        AppCenterEditorHelper.uiStyle.GetStyle("orTxt"));
                }

                GUILayout.Space(10);
                using (new AppCenterGuiFieldHelper.UnityHorizontal(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleClear")))
                {
                    GUILayout.FlexibleSpace();
                    SdkFolder = EditorGUILayout.ObjectField(SdkFolder, typeof(UnityEngine.Object), false, GUILayout.MaxWidth(200));
                    GUILayout.FlexibleSpace();
                }

                if (!isObjectFieldActive)
                {
                    // this is a hack to prevent our "block while loading technique" from breaking up at this point.
                    GUI.enabled = !EditorApplication.isCompiling && AppCenterEditor.blockingRequests.Count == 0;
                }

                if (isSdkSupported && SdkFolder != null)
                {
                    using (new AppCenterGuiFieldHelper.UnityHorizontal(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleClear")))
                    {
                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("REMOVE SDK", AppCenterEditorHelper.uiStyle.GetStyle("textButton"), GUILayout.MinHeight(32), GUILayout.MinWidth(200)))
                        {
                            RemoveSdk();
                        }

                        GUILayout.FlexibleSpace();
                    }
                }
            }

            if (SdkFolder != null)
            {
                using (new AppCenterGuiFieldHelper.UnityVertical(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleGray1")))
                {
                    isSdkSupported = false;
                    string[] versionNumber = !string.IsNullOrEmpty(InstalledSdkVersion) ? InstalledSdkVersion.Split('.') : new string[0];

                    var numerical = 0;
                    if (string.IsNullOrEmpty(InstalledSdkVersion) || versionNumber == null || versionNumber.Length == 0 ||
                        (versionNumber.Length > 0 && int.TryParse(versionNumber[0], out numerical) && numerical < 0))
                    {
                        //older version of the SDK
                        using (new AppCenterGuiFieldHelper.UnityHorizontal(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleClear")))
                        {
                            EditorGUILayout.LabelField("SDK is outdated. Consider upgrading to the get most features.", AppCenterEditorHelper.uiStyle.GetStyle("orTxt"));
                        }
                    }
                    else if (numerical >= 0)
                    {
                        isSdkSupported = true;
                    }

                    using (new AppCenterGuiFieldHelper.UnityHorizontal(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleClear")))
                    {
                        var buttonWidth = 200;

                        if (ShowSDKUpgrade() && isSdkSupported)
                        {
                            GUILayout.FlexibleSpace();
                            if (IsInstalling)
                            {
                                GUI.enabled = false;
                                var image = DrawUtils.RotateImage(AssetDatabase.LoadAssetAtPath("Assets/AppCenterEditorExtensions/Editor/UI/Images/wheel.png", typeof(Texture2D)) as Texture2D, angle++);
                                GUILayout.Button(new GUIContent("  Upgrading to " + LatestSdkVersion, image), AppCenterEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MaxWidth(buttonWidth), GUILayout.MinHeight(32));
                            }
                            else
                            {
                                if (GUILayout.Button("Upgrade to " + LatestSdkVersion, AppCenterEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MinHeight(32)))
                                {
                                    IsInstalling = true;
                                    UpgradeSdk();
                                }
                            }
                            GUILayout.FlexibleSpace();
                        }
                        else if (isSdkSupported)
                        {
                            GUILayout.FlexibleSpace();
                            EditorGUILayout.LabelField("You have the latest SDK!", labelStyle, GUILayout.MinHeight(32));
                            GUILayout.FlexibleSpace();
                        }
                    }

                    using (new AppCenterGuiFieldHelper.UnityHorizontal(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleClear")))
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("VIEW RELEASE NOTES", AppCenterEditorHelper.uiStyle.GetStyle("textButton"), GUILayout.MinHeight(32), GUILayout.MinWidth(200)))
                        {
                            Application.OpenURL("https://github.com/Microsoft/AppCenter-SDK-Unity/releases");
                        }
                        GUILayout.FlexibleSpace();
                    }
                }
            }
        }

        private static void ShowSdkNotInstalledMenu()
        {
            using (new AppCenterGuiFieldHelper.UnityVertical(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleGray1")))
            {
                var labelStyle = new GUIStyle(AppCenterEditorHelper.uiStyle.GetStyle("titleLabel"));

                EditorGUILayout.LabelField("No SDK is installed.", labelStyle, GUILayout.MinWidth(EditorGUIUtility.currentViewWidth));
                GUILayout.Space(20);

                using (new AppCenterGuiFieldHelper.UnityHorizontal(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleGray1")))
                {
                    var buttonWidth = 250;

                    GUILayout.FlexibleSpace();
                    if (IsInstalling)
                    {
                        GUI.enabled = false;
                        var image = DrawUtils.RotateImage(AssetDatabase.LoadAssetAtPath("Assets/AppCenterEditorExtensions/Editor/UI/Images/wheel.png", typeof(Texture2D)) as Texture2D, angle++);
                        GUILayout.Button(new GUIContent("  SDK is installing", image), AppCenterEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MaxWidth(buttonWidth), GUILayout.MinHeight(32));
                    }
                    else
                    {
                        if (GUILayout.Button("Install all App Center SDK packages", AppCenterEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MaxWidth(buttonWidth), GUILayout.MinHeight(32)))
                        {
                            IsInstalling = true;
                            ImportLatestSDK();
                        }
                    }
                    GUILayout.FlexibleSpace();
                }

                GUILayout.Space(10);
            }
        }

        public static void ImportLatestSDK(string existingSdkPath = null)
        {
            PackagesInstaller.ImportLatestSDK(AppCenterSDKPackage.SupportedPackages, LatestSdkVersion, existingSdkPath);
        }

        public static bool CheckIfSomePackagesInstalled()
        {
            return GetAppCenterSettings() != null;
        }

        public static bool CheckIfAllPackagesInstalled()
        {
            foreach (var package in AppCenterSDKPackage.SupportedPackages)
            {
                if (!package.IsInstalled)
                {
                    return false;
                }
            }
            return GetAppCenterSettings() != null;
        }

        public static Type GetAppCenterSettings()
        {
            if (appCenterSettingsType == typeof(object))
                return null; // Sentinel value to indicate that AppCenterSettings doesn't exist
            if (appCenterSettingsType != null)
                return appCenterSettingsType;

            appCenterSettingsType = typeof(object); // Sentinel value to indicate that AppCenterSettings doesn't exist
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in allAssemblies)
                foreach (var eachType in assembly.GetTypes())
                    if (eachType.Name == AppCenterEditorHelper.APPCENTER_SETTINGS_TYPENAME)
                        appCenterSettingsType = eachType;
            //if (appCenterSettingsType == typeof(object))
            //    Debug.LogWarning("Should not have gotten here: "  + allAssemblies.Length);
            //else
            //    Debug.Log("Found Settings: " + allAssemblies.Length + ", " + appCenterSettingsType.Assembly.FullName);
            return appCenterSettingsType == typeof(object) ? null : appCenterSettingsType;
        }

        private static bool ShowSDKUpgrade()
        {
            if (string.IsNullOrEmpty(LatestSdkVersion) || LatestSdkVersion == Constants.UnknownVersion)
            {
                return false;
            }

            if (string.IsNullOrEmpty(InstalledSdkVersion) || InstalledSdkVersion == Constants.UnknownVersion)
            {
                return true;
            }

            string[] currrent = InstalledSdkVersion.Split('.');
            string[] latest = LatestSdkVersion.Split('.');

            return int.Parse(latest[0]) > int.Parse(currrent[0])
                || int.Parse(latest[1]) > int.Parse(currrent[1])
                || int.Parse(latest[2]) > int.Parse(currrent[2]);
        }

        private static void UpgradeSdk()
        {
            if (EditorUtility.DisplayDialog("Confirm SDK Upgrade", "This action will remove the current App Center SDK and install the lastet version.", "Confirm", "Cancel"))
            {
                IEnumerable<AppCenterSDKPackage> installedPackages = GetInstalledPackages();
                RemoveSdkBeforeUpdate();
                PackagesInstaller.ImportLatestSDK(installedPackages, LatestSdkVersion);
                ImportLatestSDK(AppCenterEditorPrefsSO.Instance.SdkPath);
            }
        }

        private static IEnumerable<AppCenterSDKPackage> GetInstalledPackages()
        {
            var installedPackages = new List<AppCenterSDKPackage>();
            foreach (var package in AppCenterSDKPackage.SupportedPackages)
            {
                if (package.IsInstalled)
                {
                    installedPackages.Add(package);
                }
            }
            return installedPackages;
        }

        private static void RemoveSdkBeforeUpdate()
        {
            var skippedFiles = new[]
            {
                "AppCenterSettings.asset",
                "AppCenterSettings.asset.meta"
            };

            RemoveAndroidSettings();

            var toDelete = new List<string>();
            toDelete.AddRange(Directory.GetFiles(AppCenterEditorPrefsSO.Instance.SdkPath));
            toDelete.AddRange(Directory.GetDirectories(AppCenterEditorPrefsSO.Instance.SdkPath));

            foreach (var path in toDelete)
            {
                if (!skippedFiles.Contains(Path.GetFileName(path)))
                {
                    FileUtil.DeleteFileOrDirectory(path);
                }
            }
        }

        private static void RemoveSdk(bool prompt = true)
        {
            if (prompt && !EditorUtility.DisplayDialog("Confirm SDK Removal", "This action will remove the current App Center SDK.", "Confirm", "Cancel"))
            {
                return;
            }

            RemoveAndroidSettings();

            if (FileUtil.DeleteFileOrDirectory(AppCenterEditorPrefsSO.Instance.SdkPath))
            {
                FileUtil.DeleteFileOrDirectory(AppCenterEditorPrefsSO.Instance.SdkPath + ".meta");
                AppCenterEditor.RaiseStateUpdate(AppCenterEditor.EdExStates.OnSuccess, "App Center SDK removed.");

                // HACK for 5.4, AssetDatabase.Refresh(); seems to cause the install to fail.
                if (prompt)
                {
                    AssetDatabase.Refresh();
                }
            }
            else
            {
                AppCenterEditor.RaiseStateUpdate(AppCenterEditor.EdExStates.OnError, "An unknown error occured and the App Center SDK could not be removed.");
            }
        }

        private static void RemoveAndroidSettings()
        {
            if (Directory.Exists(Application.dataPath + "/Plugins/Android/res/values"))
            {
                var files = Directory.GetFiles(Application.dataPath + "/Plugins/Android/res/values", "appcenter-settings.xml*", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    FileUtil.DeleteFileOrDirectory(file);
                }
            }
        }

        private static void CheckSdkVersion()
        {
            if (!string.IsNullOrEmpty(InstalledSdkVersion))
                return;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        foreach (var package in AppCenterSDKPackage.SupportedPackages)
                        {
                            package.CheckIfInstalled(type);
                        }
                        if (type.FullName == "Microsoft.AppCenter.Unity.WrapperSdk")
                        {
                            foreach (var field in type.GetFields())
                            {
                                if (field.Name == "WrapperSdkVersion")
                                {
                                    InstalledSdkVersion = field.GetValue(field).ToString();
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (ReflectionTypeLoadException)
                {
                    // For this failure, silently skip this assembly unless we have some expectation that it contains App Center
                    if (assembly.FullName.StartsWith("Assembly-CSharp")) // The standard "source-code in unity proj" assembly name
                    {
                        Debug.LogWarning("App Center Editor Extension error, failed to access the main CSharp assembly that probably contains App Center SDK");
                    }
                    continue;
                }
            }
        }

        private static void GetLatestSdkVersion()
        {
            var threshold = AppCenterEditorPrefsSO.Instance.EdSet_lastSdkVersionCheck != DateTime.MinValue ? AppCenterEditorPrefsSO.Instance.EdSet_lastSdkVersionCheck.AddHours(1) : DateTime.MinValue;

            if (DateTime.Today > threshold)
            {
                AppCenterEditorHttp.MakeGitHubApiCall("https://api.github.com/repos/Microsoft/AppCenter-SDK-Unity/git/refs/tags", (version) =>
                {
                    LatestSdkVersion = version ?? Constants.UnknownVersion;
                    AppCenterEditorPrefsSO.Instance.EdSet_latestSdkVersion = LatestSdkVersion;
                });
            }
            else
            {
                LatestSdkVersion = AppCenterEditorPrefsSO.Instance.EdSet_latestSdkVersion;
            }
        }

        private static UnityEngine.Object FindSdkAsset()
        {
            UnityEngine.Object sdkAsset = null;

            // look in editor prefs
            if (AppCenterEditorPrefsSO.Instance.SdkPath != null)
            {
                sdkAsset = AssetDatabase.LoadAssetAtPath(AppCenterEditorPrefsSO.Instance.SdkPath, typeof(UnityEngine.Object));
            }
            if (sdkAsset != null)
                return sdkAsset;

            sdkAsset = AssetDatabase.LoadAssetAtPath(AppCenterEditorHelper.DEFAULT_SDK_LOCATION, typeof(UnityEngine.Object));
            if (sdkAsset != null)
                return sdkAsset;

            var fileList = Directory.GetDirectories(Application.dataPath, "*AppCenter", SearchOption.AllDirectories);
            if (fileList.Length == 0)
                return null;

            var relPath = fileList[0].Substring(fileList[0].LastIndexOf("Assets" + Path.DirectorySeparatorChar));
            return AssetDatabase.LoadAssetAtPath(relPath, typeof(UnityEngine.Object));
        }
    }
}
