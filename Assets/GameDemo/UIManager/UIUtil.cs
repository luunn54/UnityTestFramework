using UnityEngine;

public static class UIUtil
{
    public static GameObject Clone(string path, GameObject parent)
    {
        var prefab = Resources.Load<GameObject>(path);
        return Clone(prefab, parent);
    }

    public static GameObject Clone(GameObject prefab, GameObject parent)
    {
        var oldRect = prefab.GetComponent<RectTransform>();
        var newObj = Object.Instantiate(prefab);
        var newRect = newObj.GetComponent<RectTransform>();
        newRect.SetParent(parent.transform);
        CopyRectTransform(newRect, oldRect);
        return newObj;
    }

    public static void CopyRectTransform(RectTransform toRect, RectTransform fromRect)
    {
        toRect.localScale = fromRect.localScale;
        toRect.localEulerAngles = fromRect.localEulerAngles;
        toRect.anchoredPosition3D = fromRect.anchoredPosition3D;
        toRect.anchorMax = fromRect.anchorMax;
        toRect.anchorMin = fromRect.anchorMin;
        toRect.offsetMax = fromRect.offsetMax;
        toRect.offsetMin = fromRect.offsetMin;
        toRect.pivot = fromRect.pivot;
        toRect.sizeDelta = fromRect.sizeDelta;
    }    
}