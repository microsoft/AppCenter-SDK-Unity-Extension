namespace AppCenterEditor
{
    public class AppCenterAuthPackage : AppCenterSDKPackage
    {
        private const string AuthLatestDownload = "https://mobilecentersdkdev.blob.core.windows.net/sdk/AppCenterAuthLatest.unitypackage";
        private const string AuthDownloadFormat = "https://github.com/Microsoft/AppCenter-SDK-Unity/releases/download/{0}/AppCenterAuth-v{0}.unitypackage";

        public static AppCenterAuthPackage Instance = new AppCenterAuthPackage();

        public override string Name { get { return "Auth"; } }

        protected override bool IsSupportedForWSA { get { return false; } }

        public override string TypeName { get { return "Microsoft.AppCenter.Unity.Auth.Auth"; } }

        public override string VersionFieldName { get { return "AuthSDKVersion"; } }

        public override string DownloadLatestUrl { get { return AuthLatestDownload; } }

        public override string DownloadUrlFormat { get { return AuthDownloadFormat; } }

        protected override bool IsSdkPackageSupported()
        {
            return true;
        }

        private AppCenterAuthPackage()
        {
        }
    }
}
