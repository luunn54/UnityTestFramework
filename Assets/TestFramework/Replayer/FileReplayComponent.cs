using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace TestFramework
{
    public class FileReplayComponent : ReplayComponent
    {
        private ReplayController Controller;
        
        public override void StartReplay(ReplayController controller)
        {
            Controller = controller;

            //PathFileTarget = Path.Combine(controller.PathData(), ApiRecordComponent.);
            //Directory.CreateDirectory(PathFileTarget);

            Controller.RegistGlobal("RestoreFiles", (Action<string>)RestoreFiles);
        }

        private void RestoreFiles(string folderName)
        {
            var source = Path.Combine(Controller.PathData(), folderName);
            Copy(source, Application.persistentDataPath);
        }


        void Copy(string sourceDir, string targetDir)
        {
            if(!Directory.Exists(targetDir))
                Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(targetDir))
                File.Delete(file);

            foreach (var directory in Directory.GetDirectories(targetDir))
            {
                var folderName = new DirectoryInfo(directory).Name;
                if (folderName == RecordController.FRAME_WORK_FOLDER)
                {
                    continue; // skip framework data folder
                }

                try
                {
                    Directory.Delete(directory);
                }
                catch (Exception)
                {
                    
                }
            }
                
            foreach (var file in Directory.GetFiles(sourceDir))
                File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)));

            foreach (var directory in Directory.GetDirectories(sourceDir))
            {
                Copy(directory, Path.Combine(targetDir, Path.GetFileName(directory)));
            }
        }
    }
}