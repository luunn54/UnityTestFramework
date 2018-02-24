using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;


namespace TestFramework
{
    public class TestFramework
    {
        public enum Mode
        {
            Record,
            Replay,
            None
        }

        public static void SaveMode(Mode mode)
        {
            PlayerPrefs.SetString("MODE", mode.ToString());
        }

        public static void Init()
        {
            var mode = PlayerPrefs.GetString("MODE");
            SaveMode(Mode.None);

            if (mode == "Record")
            {
                GameObject.Instantiate(Resources.Load("Record"));
            }
            else if(mode == "Replay")
            {
                GameObject.Instantiate(Resources.Load("Replay"));
            }
        }
    }
}