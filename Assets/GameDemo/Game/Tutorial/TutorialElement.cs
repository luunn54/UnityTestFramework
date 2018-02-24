using UnityEngine;

public class TutorialElement : UI.Element
{
    public GameObject LockLayer;
    public RectTransform ObjectParent;
    public RectTransform Pointer;

    public void ShowLockLayer()
    {
        if (LockLayer != null) LockLayer.SetActive(true);
    }

    public void HideLockLayer()
    {
        if (LockLayer != null) LockLayer.SetActive(false);
    }

    public void ShowPointer(GameObject obj)
    {
        if (Pointer != null && obj != null)
        {
            Pointer.gameObject.SetActive(true);
            var rectTransform = obj.GetComponent<RectTransform>();

            if (rectTransform != null)
            {
                Pointer.position = rectTransform.position;
            }
            else
            {
                var screenPoint = Camera.main.WorldToScreenPoint(obj.transform.position);
                Vector2 anchoredPosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(),
                    screenPoint, null, out anchoredPosition);
                Pointer.anchoredPosition = anchoredPosition;
            }

            Pointer.localEulerAngles = Vector3.zero;
        }
    }

    public void HidePointer()
    {
        if (Pointer != null)
        {
            Pointer.gameObject.SetActive(false);
        }
    }
}