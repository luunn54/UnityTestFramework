using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace TestFramework
{
    public class AssertReplayComponent : ReplayComponent
    {
        private ReplayController Controller;
        public InputReplayComponent InputReplay;
        
        public override void StartReplay(ReplayController controller)
        {
            Controller = controller;

            //PathFileTarget = Path.Combine(controller.PathData(), ApiRecordComponent.);
            //Directory.CreateDirectory(PathFileTarget);

            //Controller.RegistGlobal("RestoreFiles", (Action<string>)RestoreFiles);
            Controller.RegistGlobal("CanClick", (Func<string, bool>)CanClick);
            Controller.RegistGlobal("GetText", (Func<string, string>)GetText);
            Controller.RegistGlobal("Assert", (Action<bool, string>)Assert);
        }

        private string GetText(String pathText)
        {
            var names = pathText.Split('\\');
            Transform objectFound = null;
            foreach (var name in names)
            {
                if (objectFound == null)
                {
                    objectFound = GameObject.Find(name).transform;
                    if (objectFound == null)
                    {
                        throw new Exception(string.Format("Can not found {0}", name));
                    }
                }
                else
                {
                    foreach (Transform children in objectFound)
                    {
                        if (children.gameObject.name == name)
                        {
                            objectFound = children;
                            break;
                        }

                        throw new Exception(string.Format("Can not found {0} in {1}", name, objectFound.gameObject.name));
                    }
                }
            }

            var text = objectFound.GetComponent<Text>();
            if (text == null)
            {
                throw new Exception(string.Format("Path {0} is not Text", pathText));
            }

            return text.text;
        }

        private bool CanClick(String buttonName)
        {
            return InputReplay.CanClick(buttonName);
        }

        private void Assert(bool result, string message)
        {
            if (!result)
            {
                throw new Exception("Assert Fail : " + message);
            }
        }
    }
}