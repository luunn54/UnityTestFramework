using System;
using UnityEngine;
using System.Collections;

public class ScalePopup : UI.PopupTransition
{
    public const float SmallScale = 0.9f;
    public const float ScaleTime = 0.2f;
    private Vector3 _originalScale;

    private void Awake()
    {
        _originalScale = transform.localScale;
    }

    public override void OnShowTransition(Action onSuccess)
    {
        TweenUtil.Scale(gameObject, SmallScale*_originalScale, _originalScale, TweenType.OutBack, ScaleTime, onSuccess);
    }

    public override void OnHideTransition(Action onSuccess)
    {
        TweenUtil.Scale(gameObject, _originalScale, SmallScale*_originalScale, TweenType.InBack, ScaleTime, onSuccess);
    }
}