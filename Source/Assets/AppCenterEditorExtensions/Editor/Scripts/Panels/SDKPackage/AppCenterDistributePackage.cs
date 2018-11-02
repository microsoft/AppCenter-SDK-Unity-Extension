namespace AppCenterEditor
{
    public class AppCenterDistributePackage : AppCenterSDKPackage
    {
        private const string DistributeLatestDownload = "https://mobilecentersdkdev.blob.core.windows.net/sdk/AppCenterDistributeLatest.unitypackage";
        private const string DistributeDownloadFormat = "https://github.com/Microsoft/AppCenter-SDK-Unity/releases/download/{0}/AppCenterDistribute-v{0}.unitypackage";
        private UnityEngine.Object pfodler;

        public static AppCenterDistributePackage Instance = new AppCenterDistributePackage();

        public override string TypeName { get { return "Microsoft.AppCenter.Unity.Distribute.Distribute"; } }

        public override string VersionFieldName { get { return "DistributeSDKVersion"; } }

        public override string Name { get { return "Distribute"; } }

        public override string DownloadLatestUrl { get { return DistributeLatestDownload; } }

        public override string DownloadUrlFormat { get { return DistributeDownloadFormat; } }

        public override bool IsPackageInstalling { get { return false; } set { } }

        public override bool IsObjectFieldActive { get { return true; } set { } }

        public override UnityEngine.Object SdkPackageFolder { get { return null; } set { pfodler = value; } }

        public override UnityEngine.Object PreviousSdkPackageFolder { get { return new UnityEngine.Object(); } set { } }

        protected override bool IsSdkPackageSupported()
        {
            return true;
        }

        protected override void RemovePackage()
        {
        }

        private AppCenterDistributePackage()
        {
        }
    }
}
