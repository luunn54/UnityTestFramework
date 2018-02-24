using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace TestFramework
{
    public class UIContextReplayComponent : ReplayComponent
    {
        private ReplayController Controller;
        private string PathFileTarget;

        public override void StartReplay(ReplayController controller)
        {
            Controller = controller;
            Controller.RegistGlobal("WaitUILoad", (Func<string, IEnumerator>)WaitUILoad);
        }

        private string NewContext = null;
        private IEnumerator WaitUILoad(string context)
        {
            NewContext = null;
            var wait = true;
            while (wait)
            {
                yield return null;
                if (NewContext != null)
                {
                    //Debug.LogError("pc " + NewContext + "_" + context + "_");
                    if (NewContext == context)
                    {
                        //Debug.LogError("ok pc " + context);
                        wait = false;
                    }
                    else
                    {
                        NewContext = null;
                    }
                }
            }
        }

        public void UIContextDidChange(string context)
        {
            NewContext = context;
        }
    }
}