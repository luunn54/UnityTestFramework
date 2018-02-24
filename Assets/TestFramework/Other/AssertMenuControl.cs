using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TestFramework
{

    public class AssertMenuControl : MonoBehaviour
    {
        public UnityEvent<string> OnClickAddAssert = new InputField.SubmitEvent();

        public RectTransform panelMenuRoot;
        public RectTransform templatelMenuAssertButton;

        public void Show(List<KeyValuePair<string, string>> buttonInfos)
        {
            foreach (Transform children in panelMenuRoot)
            {
                GameObject.Destroy(children.gameObject);
            }

            foreach (var s in buttonInfos)
            {
                var newBtn = GameObject.Instantiate(templatelMenuAssertButton);
                newBtn.gameObject.SetActive(true);
                newBtn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    OnClickAddAssert.Invoke(s.Value);
                });

                newBtn.GetComponentInChildren<Text>().text = s.Key;

                newBtn.SetParent(panelMenuRoot);
            }

            var size = (panelMenuRoot.parent as RectTransform).sizeDelta;
            Debug.LogError(size);

            var pivotX = -0.05f;
            var pivotY = -0.05f;
            if (Input.mousePosition.x > size.x/2)
            {
                pivotX = 1.05f;
            }

            if (Input.mousePosition.y > size.y / 2)
            {
                pivotY = 1.05f;
            }

            panelMenuRoot.pivot = new Vector2(pivotX, pivotY);

            panelMenuRoot.position = Input.mousePosition;
            this.gameObject.SetActive(true);
        }

        public void Hiden()
        {
            this.gameObject.SetActive(false);
        }
    }

    public static class Helper
    {
        public static bool ExitHander<T>(this GameObject obj)
        {
            //return obj.GetComponents<>()
            return obj.GetComponents<T>().Any(c => c is T);
        }
    }
}