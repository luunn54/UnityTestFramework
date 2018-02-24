using System;
using UnityEngine;

public class TutorialPointer
{
    public RectTransform Pointer;
    public Action HideAction;

    public TutorialPointer Translate(float x, float y)
    {
        var anchoredPosition = Pointer.anchoredPosition;
        anchoredPosition += new Vector2(x, y);
        Pointer.anchoredPosition = anchoredPosition;
        return this;
    }

    public TutorialPointer SetRotation(float rotation)
    {
        var angle = Pointer.localEulerAngles;
        angle.z = rotation;
        Pointer.localEulerAngles = angle;
        return this;
    }
}