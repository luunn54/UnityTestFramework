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
    public class AssertRecordComponent : RecordComponent
    {
        private RecordController Controller;
        public InputRecordComponent inputRecord;
        public AssertMenuControl AssertMenu;

        public override void StartRecord(RecordController controller)
        {
            Controller = controller;
            AssertMenu.OnClickAddAssert.AddListener(OnClickAssert);
        }

        public void OnClickAssert(string s)
        {
            Controller.WriteDelay();
            Controller.WriteScript(s);

            HidenMenu();
        }

        private void ShowMenu()
        {
            var position = Input.mousePosition;


            var dataText = GameObject.FindObjectsOfType<Text>().Where(text =>
            {
                var result = RectTransformUtility.RectangleContainsScreenPoint(text.rectTransform, position, Camera.main);
                return result;
            }).Select(text =>
            {
                var parent = text.transform.parent.gameObject;
                var textPath = text.gameObject.name;
                if (parent.ExitHander<Selectable>())
                {
                    textPath = string.Format("{0}\\\\{1}", parent.gameObject.name, textPath);                }

                return new KeyValuePair<string, string>(
                    String.Format("{0} = \"{1}\"", textPath, text.text),
                    string.Format("Assert(GetText(\"{0}\") == \"{1}\", \"{0} is not '{1}'\");" + Environment.NewLine, textPath, text.text));
            });

            var dataButton = GameObject.FindObjectsOfType<Selectable>().Where(btn =>
            {
                var component = btn as Component;
                var result = RectTransformUtility.RectangleContainsScreenPoint(component.transform as RectTransform, position, Camera.main);
                return result;
            }).SelectMany(btn =>
            {
                //var parent = text.transform.parent.gameObject;
                //if (parent.ExitHander<Selectable>())
                //{
                //    return new KeyValuePair<string, string>(
                //    String.Format("{0}\\{1} = \"{2}\"", parent.name, text.gameObject.name, text.text),
                //    string.Format("AssertText(\"{0}\\{1}\", \"{2}\");" + Environment.NewLine, parent.name, text.gameObject.name, text.text));
                //}

                return new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>(
                    String.Format("Can select {0}", btn.gameObject.name),
                    string.Format("Assert(CanClick(\"{0}\"), \"can select {0}\");" + Environment.NewLine, btn.gameObject.name)),

                    new KeyValuePair<string, string>(
                    String.Format("Can not select {0}", btn.gameObject.name),
                    string.Format("Assert(not CanClick(\"{0}\"), \"can not select {0}\");" + Environment.NewLine, btn.gameObject.name)),
                };
                    
            });

            var data = dataText.Concat(dataButton).ToList();
            if (data.Count > 0)
            {
                AssertMenu.Show(data);
            }
        }

        private void HidenMenu()
        {
            AssertMenu.Hiden();
        }

        public void OnStartMoveFly()
        {
            HidenMenu();
            inputRecord.SkipSendTouch();
        }

        public void OnEndMoveFly()
        {
            //HidenMenu();
            inputRecord.UnSkipSendTouch();
        }

        public void OnHoldFly()
        {
            ShowMenu();
        }
    }
}