using System;
using UnityEngine;

namespace AppCenterEditor
{
    class EdExLogger
    {
        public static void LogWithTimeStamp(string message)
        {
            Debug.Log(GetUniqueMessage(message));
        }

        public static void LogWarning(string message)
        {
            Debug.LogWarning(GetUniqueMessage(message));
        }

        public static void LogError(string message)
        {
            Debug.LogError(GetUniqueMessage(message));
        }
  
        public static void Log(string message)
        {
            Debug.Log(message);
        }

        private static string GetUniqueMessage(string message)
        {
            // Return unique message in order to distinguish similar messages.
            return string.Format("[App Center EdEx MSG{0}] {1}", DateTime.Now.Millisecond, message);
        }
    }
}
