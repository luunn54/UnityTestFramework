using System;
using UnityEngine;
using System.Collections;

public class MoveTopDownPopup : UI.PopupTransition
{
    public const float TopPosition = 640;
    public const float MoveTime = 0.7f;

    public override void OnShowTransition(Action onSuccess)
    {
        TweenUtil.MoveLocal(gameObject, new Vector3(0, TopPosition, 0),
            Vector3.zero, TweenType.OutBack, MoveTime, onSuccess);
    }

    public override void OnHideTransition(Action onSuccess)
    {
        TweenUtil.MoveLocal(gameObject, Vector3.zero, new Vector3(0, TopPosition, 0),
            TweenType.InBack, MoveTime, onSuccess);
    }
}