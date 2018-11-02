namespace AppCenterEditor
{
    public class AppCenterAnalyticsPackage : AppCenterSDKPackage
    {
        private const string AnalyticsLatestDownload = "https://mobilecentersdkdev.blob.core.windows.net/sdk/AppCenterAnalyticsLatest.unitypackage";
        private const string AnalyticsDownloadFormat = "https://github.com/Microsoft/AppCenter-SDK-Unity/releases/download/{0}/AppCenterAnalytics-v{0}.unitypackage";

        public static AppCenterAnalyticsPackage Instance = new AppCenterAnalyticsPackage();

        public override string Name { get { return "Analytics"; } }

        public override string TypeName { get { return "Microsoft.AppCenter.Unity.Analytics.Analytics"; } }

        public override string VersionFieldName { get { return "AnalyticsSDKVersion"; } }

        public override string DownloadLatestUrl { get { return AnalyticsLatestDownload; } }

        public override string DownloadUrlFormat { get { return AnalyticsDownloadFormat; } }

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

        private AppCenterAnalyticsPackage()
        {
        }
    }
}
