using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace TestFramework
{
    public class InputRecordComponent : RecordComponent
    {
        public abstract class InputWriter
        {
            protected string fileName;
            protected GameObject targetGameObject;
            public int FrameCount { get; protected set; }

            public virtual void StartWriter(GameObject selectGameObject, string _fileName)
            {
                FrameCount = 0;
                fileName = _fileName;
                targetGameObject = selectGameObject;
            }

            public abstract void ReceiveEvent(PointerEventData pointerEventData);

            public virtual void Finish()
            {
                
            }

            public void InCreaseFrameCount()
            {
                FrameCount++;
            }
        }

        public class TouchWriter : InputWriter
        {
            public override void ReceiveEvent(PointerEventData pointerEventData)
            {
                Vector2 a;
                var transformGob = targetGameObject.transform as RectTransform;

                RectTransformUtility.ScreenPointToLocalPointInRectangle(transformGob,
                    pointerEventData.position, pointerEventData.pressEventCamera, out a);

                a = a - transformGob.rect.min;
                File.AppendAllText(fileName, FrameCount + "|" + a.x + "|" + a.y + Environment.NewLine);

                FrameCount = 0;
            }
        }

        public class KeyboardWriter : InputWriter
        {
            protected InputField TargetInputField;
            //private bool didWriteTouchEvent;
            public override void StartWriter(GameObject selectGameObject, string _fileName)
            {
                base.StartWriter(selectGameObject, _fileName);

                TargetInputField = targetGameObject.GetComponent<InputField>();
                TargetInputField.onValueChanged.AddListener(OnTextChange);
                //didWriteTouchEvent = true;

            }

            public override void ReceiveEvent(PointerEventData pointerEventData)
            {
                Debug.LogError("Receive Evetn ?? " + FrameCount);
            }

            //public override bool FinishWhenMouseUp()
            //{
            //    return false;
            //}

            public override void Finish()
            {
                TargetInputField.onValueChanged.RemoveListener(OnTextChange);
                //Debug.LogError("Finish " + FrameCount);
                File.AppendAllText(fileName, FrameCount + Environment.NewLine);
            }

            private void OnTextChange(string value)
            {
                File.AppendAllText(fileName, FrameCount + "|" + value + Environment.NewLine);
                FrameCount = 0;
            }
        }

        public const string FOLER_DATA = "Inputs";

        private RecordController Controller;
        private string PathFileTarget;

        private BaseInput recordInput;
        private RecordExecuteEvent recordExecuteEvent;

        private InputWriter currentInputWriter;

        public override void StartRecord(RecordController controller)
        {
            Controller = controller;
            if (recordExecuteEvent == null)
            {
                var inputModule = Object.FindObjectOfType<StandaloneInputModule>();
                recordInput = inputModule.input;

                recordExecuteEvent = gameObject.AddComponent<RecordExecuteEvent>();
                recordExecuteEvent.OnEvent += ProcessEvent;
            }

            PathFileTarget = Path.Combine(controller.PathData(), FOLER_DATA);
            Directory.CreateDirectory(PathFileTarget);
        }

        private bool didProcessThisFrame = false;

        private void ProcessEvent(IEventSystemHandler handler, BaseEventData eventData)
        {
//            Debug.LogError("process event " + Environment.StackTrace + "  " + recordInput.GetMouseButtonUp(0));
            if (!didProcessThisFrame)
            {
                //Debug.LogError("Process event!");
                didProcessThisFrame = true;
                if (recordInput.GetMouseButtonDown(0))
                {
                    CreateTouchWriter(handler);
                }

                if (currentInputWriter == null)
                {
                    //Debug.LogError("Receive Event but not recorder!");
                    return;
                }

                currentInputWriter.ReceiveEvent(eventData as PointerEventData);
                //frameCount = 0;

                if (recordInput.GetMouseButtonUp(0))
                {
                    DestroyWriter();
                }
            }
        }

        private InputWriter CreateTouchWriter(IEventSystemHandler handler)
        {
            if (currentInputWriter != null)
            {
                DestroyWriter();
            }

            if(recordExecuteEvent.SkipSendTouch)
                return null;

            var selectable = handler as Selectable;
            if (selectable != null && !selectable.interactable)
            {
                return null;
            }
            
            var selectGameObject = (handler as Component).gameObject;
            if (selectGameObject.tag == "SkipRecord")
            {
                return null;
            }

            Controller.WriteDelay();
            var isDrag = handler is ScrollRect;
            var currentDragSessionName = (isDrag ? "Drag_" : "Click_") + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + selectGameObject.name + ".txt";
            var currentDragSessionPath = Path.Combine(PathFileTarget, currentDragSessionName);

            Controller.WriteScript((isDrag ? "--drag " : "--click ") + selectGameObject.name);
            Controller.WriteScript("coroutine.yield(MouseInput(\"" + selectGameObject.name + "\", \"" + currentDragSessionName + "\"));" + Environment.NewLine);
                
            currentInputWriter = new TouchWriter();

            currentInputWriter.StartWriter(selectGameObject, currentDragSessionPath);
            Controller.BeginWrite();

            return currentInputWriter;
        }

        private InputWriter CreateKeyboardWriter(InputField inputField)
        {
            if (currentInputWriter != null)
            {
                DestroyWriter();
            }

            Controller.WriteDelay();

            var selectGameObject = inputField.gameObject;
            var currentDragSessionName = "Keyboard_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + selectGameObject.name + ".txt";
            var currentDragSessionPath = Path.Combine(PathFileTarget, currentDragSessionName);

            Controller.WriteScript("--select input field " + selectGameObject.name);
            Controller.WriteScript("coroutine.yield(KeyboardInput(\"" + currentDragSessionName + "\"));" + Environment.NewLine);
            currentInputWriter = new KeyboardWriter();
            currentInputWriter.StartWriter(selectGameObject, currentDragSessionPath);

            Controller.BeginWrite();
            return currentInputWriter;
        }

        private void DestroyWriter()
        {
            //Debug.LogError("Call DestroyWriter " +  (currentInputWriter == null));
            if (currentInputWriter != null)
            {
                currentInputWriter.Finish();
                currentInputWriter = null;
                Controller.EndWriting();
            }
        }

        //private int frameCount;
        private void LateUpdate()
        {
            if (recordInput == null)
                return;

            didProcessThisFrame = false;

            var isTouchUp = recordInput.GetMouseButtonUp(0);
            if (isTouchUp)
            {
                DestroyWriter();

                var currentSelect = EventSystem.current.currentSelectedGameObject;
                if (currentSelect != null)
                {
                    var inputField = currentSelect.GetComponent<InputField>();
                    if (inputField != null && !inputField.readOnly)
                    {
                        CreateKeyboardWriter(inputField);
                    }
                }
            }

            if (currentInputWriter != null)
            {
                currentInputWriter.InCreaseFrameCount();
            }
        }

        public void SkipSendTouch()
        {
            recordExecuteEvent.SkipSendTouch = true;
        }

        public void UnSkipSendTouch()
        {
            recordExecuteEvent.SkipSendTouch = false;
        }
    }
}