using UnityEditor;
using UnityEngine;

namespace AppCenterEditor
{
    public abstract class AppCenterSDKPackage
    {
        public abstract string Name { get; }
        public abstract string InstalledVersion { get; set; }
        public abstract bool IsPackageInstalling { get; set; }
        public abstract bool IsObjectFieldActive { get; set; }
        public abstract UnityEngine.Object SdkPackageFolder { get; set; }
        public abstract UnityEngine.Object PreviousSdkPackageFolder { get; set; }
        public abstract string DownloadLatestUrl { get; }
        public abstract string DownloadUrlFormat { get; }
        protected abstract bool IsSdkPackageSupported();
        public abstract bool IsPackageInstalled();
        protected abstract void RemovePackage();
        private static int angle = 0;

        public void ShowPackageInstalledMenu()
        {
            var isPackageSupported = IsSdkPackageSupported();

            IsObjectFieldActive = SdkPackageFolder == null;

            if (PreviousSdkPackageFolder != SdkPackageFolder)
            {
                PreviousSdkPackageFolder = SdkPackageFolder;
                AppCenterEditorPrefsSO.Instance.SdkPath = (AssetDatabase.GetAssetPath(SdkPackageFolder));
                IsObjectFieldActive = false;
            }

            using (new AppCenterGuiFieldHelper.UnityVertical(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleGray1")))
            {
                using (new AppCenterGuiFieldHelper.UnityHorizontal(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleClear")))
                {
                    GUILayout.FlexibleSpace();
                    var sdkPackageVersion = InstalledVersion;
                    var labelStyle = new GUIStyle(AppCenterEditorHelper.uiStyle.GetStyle("versionText"));
                    EditorGUILayout.LabelField(string.Format("App Center {0} SDK {1} is installed", Name, string.IsNullOrEmpty(sdkPackageVersion) ? "Unknown" : sdkPackageVersion), labelStyle);
                    GUILayout.FlexibleSpace();
                }

                if (!IsObjectFieldActive)
                {
                    GUI.enabled = false;
                }
                else
                {
                    EditorGUILayout.LabelField(
                        "An Analytics SDK was detected, but we were unable to find the directory. Drag-and-drop the top-level App Center SDK folder below.",
                        AppCenterEditorHelper.uiStyle.GetStyle("orTxt"));
                }

                using (new AppCenterGuiFieldHelper.UnityHorizontal(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleClear")))
                {
                    GUILayout.FlexibleSpace();
                    SdkPackageFolder = EditorGUILayout.ObjectField(SdkPackageFolder, typeof(UnityEngine.Object), false, GUILayout.MaxWidth(200));
                    GUILayout.FlexibleSpace();
                }

                if (!IsObjectFieldActive)
                {
                    // this is a hack to prevent our "block while loading technique" from breaking up at this point.
                    GUI.enabled = !EditorApplication.isCompiling && AppCenterEditor.blockingRequests.Count == 0;
                }

                if (isPackageSupported && SdkPackageFolder != null)
                {
                    using (new AppCenterGuiFieldHelper.UnityHorizontal(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleEmpty")))
                    {
                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("Remove SDK", AppCenterEditorHelper.uiStyle.GetStyle("textButtonMagenta"), GUILayout.MinHeight(32), GUILayout.MinWidth(200)))
                        {
                            RemovePackage();
                        }

                        GUILayout.FlexibleSpace();
                    }
                }
            }
        }

        public void ShowPackageNotInstalledMenu()
        {
            using (new AppCenterGuiFieldHelper.UnityVertical(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleEmpty")))
            {
                using (new AppCenterGuiFieldHelper.UnityHorizontal(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleEmpty")))
                {
                    GUILayout.FlexibleSpace();
                    var labelStyle = new GUIStyle(AppCenterEditorHelper.uiStyle.GetStyle("versionText"));
                    EditorGUILayout.LabelField(string.Format("{0} SDK is not installed.", Name), labelStyle);
                    GUILayout.FlexibleSpace();
                }

                using (new AppCenterGuiFieldHelper.UnityHorizontal(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleEmpty")))
                {
                    GUILayout.FlexibleSpace();
                    if (IsPackageInstalling)
                    {
                        GUI.enabled = false;
                        var image = DrawUtils.RotateImage(AssetDatabase.LoadAssetAtPath("Assets/AppCenterEditorExtensions/Editor/UI/Images/wheel.png", typeof(Texture2D)) as Texture2D, angle++);
                        GUILayout.Button(new GUIContent(string.Format("  {0} SDK is installing", Name), image), AppCenterEditorHelper.uiStyle.GetStyle("customButton"), GUILayout.MaxWidth(200), GUILayout.MinHeight(32));
                    }
                    else
                    {
                        if (GUILayout.Button("Install SDK", AppCenterEditorHelper.uiStyle.GetStyle("textButtonMagenta")))
                        {
                            IsPackageInstalling = true;
                            ImportLatestPackageSDK();
                        }
                    }
                    GUILayout.FlexibleSpace();
                }
            }
        }

        public string GetDownloadUrl(string version)
        {
            if (string.IsNullOrEmpty(version) || version == "Unknown")
            {
                return DownloadLatestUrl;
            }
            else
            {
                return string.Format(DownloadUrlFormat, version);
            }
        }

        private void ImportLatestPackageSDK()
        {
            PackagesInstaller.ImportLatestSDK(new[] { this }, AppCenterEditorSDKTools.LatestSdkVersion);
        }
    }
}
