using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TestFramework
{
    public class TestMenuItem
    {

        [MenuItem("TestFramework/Record")]
        private static void Record()
        {
            TestFramework.SaveMode(TestFramework.Mode.Record);
            EditorApplication.ExecuteMenuItem("Edit/Play");
        }

        [MenuItem("TestFramework/Replay")]
        private static void Replay()
        {
            TestFramework.SaveMode(TestFramework.Mode.Replay);
            EditorApplication.ExecuteMenuItem("Edit/Play");
        }
    }

}
