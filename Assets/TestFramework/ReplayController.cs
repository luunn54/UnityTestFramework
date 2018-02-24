using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using UnityEngine;

namespace TestFramework
{
    public abstract class ReplayComponent : MonoBehaviour
    {
        public abstract void StartReplay(ReplayController controller);
    }

    public class ReplayController : MonoBehaviour {

        [SerializeField]
        private ReplayComponent[] Components;

        private string _pathData;
        private Script script;

        public string PathData()
        {
            return _pathData;
        }

        void Start()
        {
            var folder = "RecordData";// DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"); // case sensitive
            _pathData = Path.Combine(Path.Combine(Application.persistentDataPath, RecordController.FRAME_WORK_FOLDER), folder);

            var _pathDataScriptFile = Path.Combine(_pathData, RecordController.LUA_FILE);
            var lua = File.ReadAllText(_pathDataScriptFile);


            // Load the code and get the returned function
            script = new Script();
            UserData.RegisterAssembly();

            script.Options.DebugPrint = s => Debug.LogError(s);
            script.Globals["DelayFrame"] = (Func<int, IEnumerator>)DelayFrame;
            script.Globals["DelaySecond"] = (Func<float, IEnumerator>)DelaySecond;

            foreach (var component in Components)
            {
                component.StartReplay(this);
            }

            StartCoroutine(RunLua(lua));
        }

        public void RegistGlobal(string name, object obj)
        {
            script.Globals[name] = obj;
        }

        private IEnumerator RunLua(string lua)
        {
            yield return null;
            string code = @"
	            return function()
		            " + lua + @"
	            end
	            ";

            DynValue function = script.DoString(code);

            // Create the coroutine in C#
            DynValue coroutine = script.CreateCoroutine(function);
            //function.Function.Call();

            while (true)
            {
                //Debug.LogError("Start Resume");
                DynValue x = coroutine.Coroutine.Resume();
                if (x.IsVoid())
                {
                    Debug.LogError("---------Done---------");
                    // end of script
                    break;
                }
                else
                {
                    if (x.Type == DataType.Tuple
                        && x.Tuple[0].UserData != null
                        && x.Tuple[0].UserData.Object is EnumerableWrapper
                        )
                    {
                        var enumerableWrapper = x.Tuple[0].UserData.Object as EnumerableWrapper;
                        yield return enumerableWrapper.Enumerator;
                    }
                }

                yield return null;
            }
        }

        IEnumerator DelayFrame(int numberFrame)
        {
            for (int i = 0; i < numberFrame; i++)
            {
                yield return null;
            }
        }

        IEnumerator DelaySecond(float second)
        {
            yield return new WaitForSeconds(second);
        }
	
	    // Update is called once per frame
	    void Update () {
		
	    }
    }
}