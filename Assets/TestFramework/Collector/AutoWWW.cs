using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace TestFramework
{
    public interface WWWInterface : IDisposable
    {
        Dictionary<string, string> responseHeaders { get; }

        string text { get; }

        string error { get; }

        bool isDone { get; }
    }

    public class AutoWWW : WWWInterface
    {
        public static WWWInterface Create(string url, byte[] postData, Dictionary<string, string> headers)
        {
            var recordComponent = GameObject.FindObjectOfType<ApiRecordComponent>();

            if (recordComponent != null)
                return recordComponent.CreateWWW(url, postData, headers);

            var replayComponent = GameObject.FindObjectOfType<ApiReplayComponent>();

            WWWInterface ww = null;
            if (replayComponent != null)
                ww = replayComponent.CreateWWW(url, postData, headers);
            if (ww == null)
            {
                ww = new AutoWWW(url, postData, headers);
            }

            return ww;
        }

        //public ApiRecordComponent RecordComponent;

        private WWW www;
        private float TimeStart;
        private bool didWriteResponse = false;
        public delegate void OnReceiveResponse(AutoWWW www, bool succes);

        public event OnReceiveResponse OnResponse;
        public string PathFile;

        public Dictionary<string, string> responseHeaders
        {
            get { return www.responseHeaders; }
        }

        public string text
        {
            get { return www.text; }
        }

        public string error
        {
            get { return www.error; }
        }

        public bool isDone
        {
            get
            {
                if (www.isDone && !didWriteResponse)
                {
                    didWriteResponse = true;
                    //Debug.LogError((www == null) +"___" + (www.error == null));
                    if (OnResponse != null)
                        OnResponse(this, string.IsNullOrEmpty(www.error));

                    if (PathFile != null)
                    {
                        File.AppendAllText(PathFile,
                        "RESPONSE_HEADER=" + DictionaryToJson(www.responseHeaders) + Environment.NewLine
                        + "TIME=" + (Time.realtimeSinceStartup - TimeStart) + Environment.NewLine
                        + (www.error != null ? 
                        "RESPONSE_ERROR=" + www.error + Environment.NewLine :
                        "RESPONSE_DATA=" + www.text + Environment.NewLine
                        )
                    );
                    }
                }
                return www.isDone;
            }
        }

        public AutoWWW(string url, byte[] postData, Dictionary<string, string> headers, string pathFile = null)
        {
            www = new WWW(url, postData, headers);

            PathFile = pathFile;
            if (PathFile != null)
            {
                var postString = System.Text.Encoding.UTF8.GetString(postData);
                File.WriteAllText(PathFile, 
                    "URL=" + url + Environment.NewLine
                    + "HEADER=" + DictionaryToJson(headers) + Environment.NewLine
                    + "POST_DATA=" + postString + Environment.NewLine
                    );
            }
            TimeStart = Time.realtimeSinceStartup;
        }

        string DictionaryToJson(Dictionary<string, string> dict)
        {
            var entries = dict.Select(d =>
                string.Format("\"{0}\": [{1}]", d.Key, d.Value)).ToArray();
            return "{" + string.Join(",", entries) + "}";
        }

        public void Dispose()
        {
            Debug.LogError("Did dispose!");
            www.Dispose();
        }
    }

    public class MockupApiData
    {
        public string Error;
        public string Body;
        public Dictionary<string, string> Header; 
        public float Time = 0.5f;
    }

    public class WWWReplay : WWWInterface
    {
        public MockupApiData mockupApi;

        public Dictionary<string, string> responseHeaders
        {
            get { return mockupApi == null ? null : mockupApi.Header; }
        }

        public string text
        {
            get { return mockupApi == null ? null : mockupApi.Body; }
        }

        public string error
        {
            get { return mockupApi == null ? null : mockupApi.Error; }
        }

        public bool isDone
        {
            get
            {
                return mockupApi != null;
            }
        }

        public void Dispose()
        {
            //www.Dispose();
        }
    }
}
