using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TestFramework
{
    public interface TestController
    {
        string PathData();
        void WriteScript(string line);
    }

    public abstract class RecordComponent : MonoBehaviour
    {
        public abstract void StartRecord(RecordController controller);
    }

    public class RecordController : MonoBehaviour, TestController
    {
        public const string FRAME_WORK_FOLDER = "FRAME_WORK_FOLDER";
        public const string LUA_FILE = "AutoRecord.lua";

        [SerializeField]
        private RecordComponent[] Components;

        private string _pathDataScriptFile;

        private string _pathData;
        public float TimeDelay { get; private set; }
        private bool IsWriting = true;

        public string PathData()
        {
            return _pathData;
        }

        public void WriteScript(string line)
        {
            File.AppendAllText(_pathDataScriptFile, line + Environment.NewLine);
        }

        // Use this for initialization
        void Start()
        {
            var folder = "RecordData";// DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"); // case sensitive
            _pathData = Path.Combine(Path.Combine(Application.persistentDataPath, FRAME_WORK_FOLDER), folder);

            if (Directory.Exists(_pathData))
            {
                DirectoryInfo di = new DirectoryInfo(_pathData);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }

            Directory.CreateDirectory(_pathData);

            _pathDataScriptFile = Path.Combine(_pathData, LUA_FILE);

            WriteScript("--Start Record" + Environment.NewLine);

            foreach (var component in Components)
            {
                component.StartRecord(this);
            }

            IsWriting = false;
            TimeDelay = 0f;
        }

        public void WriteDelay()
        {
            //Debug.LogError("IsWriting " + IsWriting);
            if (!IsWriting)
            {
                var seconds = Math.Round(TimeDelay, 1);
                //Debug.LogError("seconds " + seconds);
                if (seconds > 0.1f)
                {
                    //seconds = Math.Round(seconds, 1);
                    WriteScript("--delay " + seconds + " seconds.");
                    WriteScript("coroutine.yield(DelaySecond(" + seconds + "));" + Environment.NewLine);                    
                }
            }
        }

        public void BeginWrite()
        {
            TimeDelay = 0f;
            IsWriting = true;
        }

        public void EndWriting()
        {
            IsWriting = false;
            TimeDelay = 0f;
            // LastAction = DateTime.Now;
        }

        void Update()
        {
            if (!IsWriting)
            {
                TimeDelay += Time.deltaTime;
            }
        }
    }
}