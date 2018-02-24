using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace TestFramework
{
    public class ApiRecordComponent : RecordComponent
    {
        private RecordController Controller;
        private string PathFileTarget;
        //private int counter = 0;

        public override void StartRecord(RecordController controller)
        {
            Controller = controller;
            //AutoWWW.RecordComponent = this;

            PathFileTarget = Path.Combine(controller.PathData(), "Apis");
            Directory.CreateDirectory(PathFileTarget);

            controller.WriteScript("--Load api files");
            controller.WriteScript("LoadApiFiles(\"Apis\");" + Environment.NewLine);
        }

        //public string StartRequest(string url, byte[] postData, Dictionary<string, string> headers)
        //{
        //    var guid = counter + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + url.Replace("https://", string.Empty).Replace("\\", "-") + ".txt";
        //    counter++;

        //    var fileName = guid;
        //    Controller.WriteScript("// start request : " + fileName);
        //    var postString = System.Text.Encoding.UTF8.GetString(postData);

        //    File.WriteAllText(Path.Combine(PathFileTarget, fileName), 
        //        "URL=" + url + Environment.NewLine
        //        + "HEADER=" + DictionaryToJson(headers) + Environment.NewLine
        //        + "POST_DATA=" + postString + Environment.NewLine
        //        );

        //    return fileName;
        //}

        //string DictionaryToJson(Dictionary<string, string> dict)
        //{
        //    var entries = dict.Select(d =>
        //        string.Format("\"{0}\": [{1}]", d.Key, d.Value)).ToArray();
        //    return "{" + string.Join(",", entries) + "}";
        //}

        //public void ReceiveResponse(AutoWWW www)
        //{
        //    var fileName = www.GUID;
        //    Controller.WriteScript("// receive response : " + fileName);

        //    File.AppendAllText(Path.Combine(PathFileTarget, fileName),
        //        "RESPONSE_HEADER=" + DictionaryToJson(www.responseHeaders) + Environment.NewLine
        //        + (www.error != null ? 
        //            "RESPONSE_ERROR=" + www.error + Environment.NewLine :
        //            "RESPONSE_DATA=" + www.text + Environment.NewLine
        //            )
        //        );
        //}

        public AutoWWW CreateWWW(string url, byte[] postData, Dictionary<string, string> headers)
        {
            var guid = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" 
                + url.Replace("https://", string.Empty).Replace("http://", string.Empty).Replace("/", "-").Replace(":", "_") + ".txt";
            //counter++;

            var fileName = guid;
            Controller.WriteScript("-- start request : " + url);

            var wwwRecord = new AutoWWW(url, postData, headers, Path.Combine(PathFileTarget, fileName));
            wwwRecord.OnResponse += OnResponse;

            return wwwRecord;
        }

        public void OnResponse(AutoWWW www, bool isSuccess)
        {
            www.OnResponse -= OnResponse;
            Controller.WriteScript("-- finish request, is success : " + isSuccess);
        }
    }
}