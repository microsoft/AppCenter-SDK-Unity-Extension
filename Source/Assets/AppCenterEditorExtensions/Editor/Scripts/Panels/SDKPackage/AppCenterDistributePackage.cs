using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AppCenterEditor
{
    public class AppCenterDistributePackage : AppCenterSDKPackage
    {
        public override string Name
        {
            get
            {
                return "Distribute";
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
                return true;
            }
            set
            {
            }
        }        

        public override bool IsObjectFieldActive
        {
            get
            {
                return false;
            }
            set
            {
            }
        }
        private UnityEngine.Object pfodler;

        public override UnityEngine.Object SdkPackageFolder
        {
            get
            {
                return null;
            }
            set
            {
                pfodler = value;
            }
        }

        public override UnityEngine.Object PreviousSdkPackageFolder
        {
            get
            {
                return new UnityEngine.Object();
            }
            set
            {
            }
        }

        public override bool IsPackageInstalled()
        {
            return true;
        }

        protected override void ImportLatestPackageSDK()
        {
        }

        protected override bool IsSdkPackageSupported()
        {
            return true;
        }

        protected override void RemovePackage()
        { 
        }
    }
}
