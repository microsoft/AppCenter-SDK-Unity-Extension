namespace AppCenterEditor
{
    public class AppCenterAnalyticsPackage : AppCenterSDKPackage
    {
        public override string Name
        {
            get
            {
                return "Analytics";
            }
        }

        public override string InstalledVersion
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public override bool IsPackageInstalling
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        public override bool IsObjectFieldActive
        {
            get
            {
                return true;
            }
            set
            {
            }
        }

        public override UnityEngine.Object SdkPackageFolder
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public override UnityEngine.Object PreviousSdkPackageFolder
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public override bool IsPackageInstalled()
        {
            return false;
        }

        //protected override void ImportLatestPackageSDK()
        //{
        //}

        protected override bool IsSdkPackageSupported()
        {
            return true;
        }

        protected override void RemovePackage()
        {
        }
    }
}
