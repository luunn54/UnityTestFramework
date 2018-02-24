using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace TestFramework
{
    public class ApiReplayComponent : ReplayComponent
    {
        private ReplayController Controller;
        //private string PathFileTarget;
        private Dictionary<string, List<MockupApiData>> MockupApis; 
        
        public override void StartReplay(ReplayController controller)
        {
            Controller = controller;

            //PathFileTarget = Path.Combine(controller.PathData(), ApiRecordComponent.);
            //Directory.CreateDirectory(PathFileTarget);

            MockupApis = new Dictionary<string, List<MockupApiData>>();

            Controller.RegistGlobal("LoadApiFiles", (Action<string>)LoadApiFiles);
            Controller.RegistGlobal("MockupApiError", (Action<string, string>)MockupApiError);
            Controller.RegistGlobal("MockupApi", (Action<string, string>)MockupApi);
            //Controller.RegistGlobal("MouseInput", (Func<string, string, IEnumerator>)MouseInput);
        }

        private List<MockupApiData> GetListForApi(string url)
        {
            if (MockupApis.ContainsKey(url))
            {
                return MockupApis[url];
            }

            var data = new List<MockupApiData>();
            MockupApis[url] = data;

            return data;
        }

        public void LoadApiFiles(string folderName)
        {
            var path = Path.Combine(Controller.PathData(), folderName);
            var files = Directory.GetFiles(path);

            // sort file if need


            foreach (var file in files)
            {
                var lines = File.ReadAllLines(file);
                string url = null, body = null, error = null;
                Dictionary<string, string> header = null;
                float time = 0.5f;
                foreach (var line in lines)
                {
                    if (line.StartsWith("URL="))
                    {
                        url = line.Substring("URL=".Length);
                    }
                    else if (line.StartsWith("TIME="))
                    {
                        var timeString = line.Substring("TIME=".Length);
                        time = float.Parse(timeString);
                    }
                    else if (line.StartsWith("RESPONSE_DATA="))
                    {
                        body = line.Substring("RESPONSE_DATA=".Length);
                    }
                    else if (line.StartsWith("RESPONSE_ERROR="))
                    {
                        error = line.Substring("RESPONSE_ERROR=".Length);
                    }
                    else if (line.StartsWith("RESPONSE_HEADER="))
                    {
                        var headerString = line.Substring("RESPONSE_HEADER=".Length);
                        // string to dic
                        header = new Dictionary<string, string>();
                    }
                }

                var mockupApi = new MockupApiData()
                {
                    Body = body,
                    Error = error,
                    Time = time,
                    Header = header
                };

                GetListForApi(url).Add(mockupApi);
            }
        }

        public void MockupApiError(string url, string error)
        {
            GetListForApi(url).Insert(0, new MockupApiData()
            {
                Error = error
            });
        }

        public void MockupApi(string url, string body)
        {
            GetListForApi(url).Insert(0, new MockupApiData()
            {
                Body = body
            });
        }

        public MockupApiData GetNextApi(string url)
        {
            var list = GetListForApi(url);
            if (list.Count > 0)
            {
                var next = list[0];
                list.RemoveAt(0);

                return next;
            }

            return null;
        }

        public WWWInterface CreateWWW(string url, byte[] postData, Dictionary<string, string> headers)
        {
            var mockup = GetNextApi(url);
            if (mockup != null)
            {
                var replay = new WWWReplay();
                StartCoroutine(DelayApi(replay, mockup));

                return replay;
            }

            return null;
        }

        private IEnumerator DelayApi(WWWReplay replay, MockupApiData mockup)
        {
            yield return new WaitForSeconds(mockup.Time);
            replay.mockupApi = mockup;
        }
    }
}