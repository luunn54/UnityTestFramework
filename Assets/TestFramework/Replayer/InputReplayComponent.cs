using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace TestFramework
{
    public class InputReplayComponent : ReplayComponent
    {
        private ReplayController Controller;
        private string PathFileTarget;
        private RecordExecuteEvent recordExecuteEvent;
//        private Dictionary<int, PointerEventData> m_PointerData;

        public override void StartReplay(ReplayController controller)
        {
            Controller = controller;

            var customInput = Object.FindObjectOfType<ReplayInput>();
            if (customInput == null)
            {
                customInput = gameObject.AddComponent<ReplayInput>();
                customInput.ReplayComponent = this;

                var inputModule = Object.FindObjectOfType<StandaloneInputModule>();
                var prop = inputModule.GetType().GetField("m_InputOverride", System.Reflection.BindingFlags.NonPublic
                | System.Reflection.BindingFlags.Instance);
                prop.SetValue(inputModule, customInput);

                //prop = inputModule.GetType().GetField("m_PointerData", System.Reflection.BindingFlags.NonPublic
                //| System.Reflection.BindingFlags.Instance);
                //m_PointerData = prop.GetValue(inputModule) as Dictionary<int, PointerEventData>;
            }

            if (recordExecuteEvent == null)
            {
                recordExecuteEvent = gameObject.AddComponent<RecordExecuteEvent>();
                recordExecuteEvent.OnEvent += ProcessEvent;
            }

            //customInput.ReplayComponent = this;

            PathFileTarget = Path.Combine(controller.PathData(), InputRecordComponent.FOLER_DATA);
            //Directory.CreateDirectory(PathFileTarget);

            //Controller.RegistGlobal("MouseClick", (Func<string, IEnumerator>)MouseClick);
            Controller.RegistGlobal("MouseInput", (Func<string, string, IEnumerator>)MouseInput);
            Controller.RegistGlobal("KeyboardInput", (Func<string, IEnumerator>)KeyboardInput);
            Controller.RegistGlobal("ClickObject", (Func<string, IEnumerator>)ClickObject);

            //Controller.RegistGlobal("CanClick", (Func<string, bool>)CanClick);
            Controller.RegistGlobal("KeyboardInputText", (Func<string, IEnumerator>)KeyboardInputText);
            Controller.RegistGlobal("InputTextField", (Func<string, string, IEnumerator>)InputTextField);
            //Controller.RegistGlobal("MouseDown", (Action<string>)MouseDown);
            //Controller.RegistGlobal("MouseUp", (Action)MouseUp);
        }

        #region record current touch Go this frame

        private GameObject targetGameObject;
        private bool didProcessThisFrame = false;

        private void ProcessEvent(IEventSystemHandler handler, BaseEventData eventData)
        {
            if (!didProcessThisFrame)
            {
                didProcessThisFrame = true;
                targetGameObject = (handler as Component).gameObject;

                if (CurrentPointerData != null && CurrentPointerData.SelectGameObject == null)
                {
                    CurrentPointerData.SelectGameObject = targetGameObject;
                }
            }
        }

        private void LateUpdate()
        {
            didProcessThisFrame = false;
            targetGameObject = null;
        }

        #endregion

        public PointerData CurrentPointerData;

        

        private IEnumerable<GameObject> FindGameObject(string name)
        {
            return Object.FindObjectsOfType<GameObject>().Where(gOb =>
            {
                if (gOb.name == name)
                {
                    var result = true;
                    var selectable = gOb.GetComponent<Selectable>();
                    if (selectable != null && !selectable.interactable)
                    {
                        result = false;
                    }

                    return result;
                }

                return false;
            });
        }

        private IEnumerator InputTextField(string textFieldName, string text)
        {
            yield return ClickObject(textFieldName);
            yield return KeyboardInputText(text);
        }

        private IEnumerator KeyboardInputText(string text)
        {
            while (CurrentPointerData != null)
            {
                yield return null;
            }

            var target = EventSystem.current.currentSelectedGameObject;
            if (target == null)
            {
                throw new Exception("Select InputField first!");
            }

            var inputField = target.GetComponent<InputField>();
            if (inputField == null)
            {
                throw new Exception("InputField not found!");
            }

            if (inputField.readOnly)
            {
                throw new Exception("InputField is read only!");
            }

            inputField.text = text;
            yield return null;
        }

        private IEnumerator KeyboardInput(string fileName)
        {
            while (CurrentPointerData != null)
            {
                yield return null;
            }

            var target = EventSystem.current.currentSelectedGameObject;
            if (target == null)
            {
                throw new Exception("Select InputField first!");
            }

            var inputField = target.GetComponent<InputField>();
            if (inputField == null)
            {
                throw new Exception("InputField not found!");
            }

            if (inputField.readOnly)
            {
                throw new Exception("InputField is read only!");
            }

            var pathFile = Path.Combine(PathFileTarget, fileName);
            var lines = File.ReadAllLines(pathFile);

            foreach (var line in lines)
            {
                var ar = line.Split(new[] { '|' }, 2);
                var frame = int.Parse(ar[0]);
                string value = null;
                if (ar.Length > 1)
                    value = ar[1];

                for (int i = 0; i < frame; i++)
                {
                    yield return null;
                }

                if(value != null)
                    inputField.text = value;
            }
        }

        private IEnumerator MouseInput(string goName, string fileName) // click on button
        {
            while (CurrentPointerData != null)
            {
                yield return null;
            }

            //Debug.LogError(pointerDic.Count);

            var founds = FindGameObject(goName).ToArray();
            if (founds.Length > 1)
            {
                Debug.LogError(founds.Length + " gameobject found with name " + goName);
            }
            else if (founds.Length == 0)
            {
                throw new Exception("gameobject " + goName + " not found!");
            }

            var target = founds[0];
            var rectTransform = target.transform as RectTransform;

            var pathFile = Path.Combine(PathFileTarget, fileName);
            var lines = File.ReadAllLines(pathFile);

            CurrentPointerData = new PointerData();
            TouchDown();
            StartCoroutine(DelayRevertTouchDown());
            //yield return null;

            foreach (var line in lines)
            {
                var ar = line.Split(new[] {'|'}, 3);
                var frame = int.Parse(ar[0]);
                var x = float.Parse(ar[1]);
                var y = float.Parse(ar[2]);
                for (int i = 0; i < frame; i++)
                {
                    yield return null;
                }

                //Debug.LogError(pointerDic.Count);

                Vector3 min = rectTransform.TransformPoint(rectTransform.rect.min + new Vector2(x, y));
                //Vector3 max = rectTransform.TransformPoint(rectTransform.rect.max);

                var min2D = RectTransformUtility.WorldToScreenPoint(Camera.main, min);

                CurrentPointerData.position = min2D;
            }

            //Debug.LogError(targetGameObject);

            //Debug.LogError(pointerDic.Count);
            TouchUp();
            yield return null;
            var clickGameObject = CurrentPointerData.SelectGameObject;
            CurrentPointerData = null;

            if (clickGameObject != target)
            {
                throw new Exception("Cannot select " + goName + " - found : " + clickGameObject);
            }
        }

        private IEnumerator ClickObject(String name)
        {
            while (CurrentPointerData != null)
            {
                yield return null;
            }

            var founds = FindGameObject(name).Where(gOb => gOb.ExitHander<Selectable>()).ToArray();
            if (founds.Length > 1)
            {
                Debug.LogError(founds.Length + " Object found with name " + name);
            }
            else if (founds.Length == 0)
            {
                throw new Exception("Object " + name + " not found!");
            }

            var target = founds[0];

            var rectTransform = target.transform as RectTransform;

            Vector3 min = rectTransform.TransformPoint((rectTransform.rect.min + rectTransform.rect.max) / 2);

            var min2D = RectTransformUtility.WorldToScreenPoint(Camera.main, min);

            CurrentPointerData = new PointerData()
            {
                position = min2D
            };
            TouchDown();
            StartCoroutine(DelayRevertTouchDown());

            yield return null;
            CurrentPointerData.TouchUp();
            yield return null;

            var clickGameObject = CurrentPointerData.SelectGameObject;
            CurrentPointerData = null;

            if (clickGameObject != target)
            {
                throw new Exception("Cannot select " + name + " - found :" + clickGameObject);
            }
            CurrentPointerData = null;
        }

        public bool CanClick(String buttonName)
        {
            var pointerDataBacup = CurrentPointerData;
            recordExecuteEvent.SkipSendTouch = true;
            var selectedGameObjectBacup = EventSystem.current.currentSelectedGameObject;

            var founds = FindGameObject(buttonName).Where(gOb => gOb.ExitHander<Selectable>()).ToArray();
            if (founds.Length > 1)
            {
                Debug.LogError(founds.Length + " Button found with name " + buttonName);
            }
            else if (founds.Length == 0)
            {
                return false;
            }

            var target = founds[0];

            var rectTransform = target.transform as RectTransform;

            Vector3 min = rectTransform.TransformPoint((rectTransform.rect.min + rectTransform.rect.max) / 2);

            var min2D = RectTransformUtility.WorldToScreenPoint(Camera.main, min);
            var inputModule = Object.FindObjectOfType<StandaloneInputModule>();
            
            CurrentPointerData = new PointerData()
            {
                position = min2D
            };
            CurrentPointerData.TouchDown();
            CurrentPointerData.TouchUp();

            didProcessThisFrame = false;
            inputModule.Process();

            CurrentPointerData.RevertTouchDown();
            CurrentPointerData.RevertTouchUp();

            var clickGameObject = CurrentPointerData.SelectGameObject;

            CurrentPointerData = pointerDataBacup;
            recordExecuteEvent.SkipSendTouch = false;
            EventSystem.current.SetSelectedGameObject(selectedGameObjectBacup);

            return clickGameObject == target;
        }

        private void TouchDown()
        {
            CurrentPointerData.TouchDown();
        }

        private IEnumerator DelayRevertTouchDown()
        {
            yield return null;
            CurrentPointerData.RevertTouchDown();
        }

        private void TouchUp()
        {
            CurrentPointerData.TouchUp();
        }

        private IEnumerator DelayRevertTouchUp()
        {
            yield return null;
            CurrentPointerData.RevertTouchUp();
        }
    }
}