using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine.Networking;

namespace AppCenterEditor
{
    public class AppCenterEditorHttp : Editor
    {
        internal static void MakeDownloadCall(string url, Action<string> resultCallback)
        {
            EdExLogger.LoggerInstance.LogWithTimeStamp("Downloading file: " + url);
            var www = UnityWebRequest.Get(url);
            AppCenterEditor.RaiseStateUpdate(AppCenterEditor.EdExStates.OnHttpReq, url, AppCenterEditorHelper.MSG_SPIN_BLOCK);
            Coroutiner.StartCoroutine(PostDownload(www, response =>
            {
                resultCallback(WriteResultFile(url, response));
            }, AppCenterEditorHelper.SharedErrorCallback));
        }

        internal static void MakeDownloadCall(IEnumerable<string> urls, Action<IEnumerable<string>> resultCallback)
        {
            EdExLogger.LoggerInstance.LogWithTimeStamp("Downloading files: " + string.Join(", ", urls.ToArray()));
            var wwws = new List<UnityWebRequest>();
            var downloadRequests = new List<DownloadRequest>();
            foreach (var url in urls)
            {
                var www = UnityWebRequest.Get(url);
                wwws.Add(www);
                downloadRequests.Add(new DownloadRequest(url, www));
            }
            AppCenterEditor.RaiseStateUpdate(AppCenterEditor.EdExStates.OnHttpReq, "Downloading files", AppCenterEditorHelper.MSG_SPIN_BLOCK);
            Coroutiner.StartCoroutine(DownloadFiles(downloadRequests, resultCallback, AppCenterEditorHelper.SharedErrorCallback));
        }

        internal static void MakeGitHubApiCall(string url, Action<string> resultCallback)
        {
            var www = UnityWebRequest.Get(url);
            Coroutiner.StartCoroutine(Post(www, response => { OnGitHubSuccess(resultCallback, response); }, AppCenterEditorHelper.SharedErrorCallback));
        }

        private static IEnumerator Post(UnityWebRequest www, Action<string> callBack, Action<string> errorCallback)
        {
            yield return www.SendWebRequest();
            if (!string.IsNullOrEmpty(www.error))
            {
                errorCallback(www.error);
            }
            else
            {
                callBack(www.downloadHandler.text);
            }
        }

        private static IEnumerator PostDownload(UnityWebRequest www, Action<byte[]> callBack, Action<string> errorCallback)
        {
            yield return www.SendWebRequest();
            if (!string.IsNullOrEmpty(www.error))
            {
                errorCallback(www.error);
            }
            else
            {
                callBack(www.downloadHandler.data);
            }
        }

        private static IEnumerator DownloadFiles(IEnumerable<DownloadRequest> downloadRequests, Action<IEnumerable<string>> resultCallback, Action<string> errorCallback)
        {
            var downloadedFiles = new List<string>();
            foreach (var downloadRequest in downloadRequests)
            {
                yield return downloadRequest.WWW.SendWebRequest();

                if (!downloadRequest.WWW.isHttpError && !downloadRequest.WWW.isNetworkError)
                {
                    var downloadedFile = WriteResultFile(downloadRequest.Url, downloadRequest.WWW.downloadHandler.data);
                    downloadedFiles.Add(downloadedFile);
                }
                else
                {
                    errorCallback(downloadRequest.WWW.error);
                    yield break;
                }
            }
            resultCallback(downloadedFiles);
        }

        private static void OnGitHubSuccess(Action<string> resultCallback, string response)
        {
            if (resultCallback == null)
            {
                return;
            }
            var jsonResponse = JsonWrapper.DeserializeObject<List<object>>(response);
            if (jsonResponse == null || jsonResponse.Count == 0)
            {
                return;
            }
            // list seems to come back in ascending order (oldest -> newest)
            var latestSdkTag = (JsonObject)jsonResponse[jsonResponse.Count - 1];
            object tag;
            if (latestSdkTag.TryGetValue("ref", out tag))
            {
                var startIndex = tag.ToString().LastIndexOf('/') + 1;
                var length = tag.ToString().Length - startIndex;
                resultCallback(tag.ToString().Substring(startIndex, length));
            }
            else
            {
                resultCallback(null);
            }
        }

        private static string WriteResultFile(string url, byte[] response)
        {
            AppCenterEditor.RaiseStateUpdate(AppCenterEditor.EdExStates.OnHttpRes, url);
            string fileName;
            if (url.IndexOf("AppCenterEditorExtensions-v") > -1)
            {
                fileName = AppCenterEditorHelper.EDEX_UPGRADE_PATH;
            }
            else if (url.IndexOf("AppCenterAnalytics-v") > -1)
            {
                fileName = AppCenterEditorHelper.ANALYTICS_SDK_DOWNLOAD_PATH;
            }
            else if (url.IndexOf("AppCenterCrashes-v") > -1)
            {
                fileName = AppCenterEditorHelper.CRASHES_SDK_DOWNLOAD_PATH;
            }
            else if (url.IndexOf("AppCenterDistribute-v") > -1)
            {
                fileName = AppCenterEditorHelper.DISTRIBUTE_SDK_DOWNLOAD_PATH;
            }
            else
            {
                fileName = AppCenterEditorHelper.EDEX_PACKAGES_PATH;
            }
            var fileSaveLocation = AppCenterEditorHelper.EDEX_ROOT + fileName;
            var fileSaveDirectory = Path.GetDirectoryName(fileSaveLocation);
            EdExLogger.LoggerInstance.LogWithTimeStamp("Saving " + response.Length + " bytes to: " + fileSaveLocation);
            if (!Directory.Exists(fileSaveDirectory))
            {
                Directory.CreateDirectory(fileSaveDirectory);
            }
            File.WriteAllBytes(fileSaveLocation, response);
            return fileSaveLocation;
        }

        private class DownloadRequest
        {
            public string Url { get; private set; }
            public UnityWebRequest WWW { get; private set; }

            public DownloadRequest(string url, UnityWebRequest www)
            {
                Url = url;
                WWW = www;
            }
        }
    }
}
