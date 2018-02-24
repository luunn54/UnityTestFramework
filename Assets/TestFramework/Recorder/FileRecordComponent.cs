using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TestFramework
{
    public class FileRecordComponent : RecordComponent
    {

        public override void StartRecord(RecordController controller)
        {
            var fileTarget = Path.Combine(controller.PathData(), "Files");

            controller.WriteScript("-- Restore files");
            Copy(Application.persistentDataPath, fileTarget);
            controller.WriteScript("RestoreFiles(\"Files\");" + Environment.NewLine);
        }

        void Copy(string sourceDir, string targetDir)
        {
            Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(sourceDir))
                File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)));

            foreach (var directory in Directory.GetDirectories(sourceDir))
            {
                var folderName = new DirectoryInfo(directory).Name;
                if (folderName == RecordController.FRAME_WORK_FOLDER)
                {
                    continue; // skip framework data folder
                }

                Copy(directory, Path.Combine(targetDir, Path.GetFileName(directory)));
            }
        }
    }
}