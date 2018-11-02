namespace AppCenterEditor
{
    public class AppCenterCrashesPackage : AppCenterSDKPackage
    {
        private const string CrashesLatestDownload = "https://mobilecentersdkdev.blob.core.windows.net/sdk/AppCenterCrashesLatest.unitypackage";
        private const string CrashesDownloadFormat = "https://github.com/Microsoft/AppCenter-SDK-Unity/releases/download/{0}/AppCenterCrashes-v{0}.unitypackage";

        public static AppCenterCrashesPackage Instance = new AppCenterCrashesPackage();

        public override string Name { get { return "Crashes"; } }

        public override string TypeName { get { return "Microsoft.AppCenter.Unity.Crashes.Crashes"; } }

        public override string VersionFieldName { get { return "CrashesSDKVersion"; } }

        public override string DownloadLatestUrl { get { return CrashesLatestDownload; } }

        public override string DownloadUrlFormat { get { return CrashesDownloadFormat; } }

        public override bool IsPackageInstalling { get { return false; } set { } }

        public override bool IsObjectFieldActive { get { return true; } set { } }

        public override UnityEngine.Object SdkPackageFolder { get { return null; } set { } }

        public override UnityEngine.Object PreviousSdkPackageFolder { get { return null; } set { } }

        protected override bool IsSdkPackageSupported()
        {
            return true;
        }

        protected override void RemovePackage()
        {
        }

        private AppCenterCrashesPackage()
        {
        }
    }
}
