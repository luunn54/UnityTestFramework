using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace TestFramework
{
    public class UIContextRecordComponent : RecordComponent
    {
        private RecordController Controller;
        //private string PathFileTarget;

        public override void StartRecord(RecordController controller)
        {
            Controller = controller;

            //PathFileTarget = Path.Combine(controller.PathData(), "Apis");
            //Directory.CreateDirectory(PathFileTarget);

            //controller.WriteScript("//Load api files");
            //controller.WriteScript("LoadApiFiles(\"Apis\");");
        }

        [ContextMenu("Load Scene")]
        private void TestLoad()
        {
            UIContextDidChange("Menu");
        }

        public void UIContextDidChange(string context)
        {
            Controller.BeginWrite();
            Controller.WriteScript("--wait UI load " + context);
            Controller.WriteScript("coroutine.yield(WaitUILoad(\"" + context + "\"));" + Environment.NewLine);
            Controller.EndWriting();
        }
    }
}