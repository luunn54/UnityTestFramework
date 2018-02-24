using System;
using UnityEngine.UI;

public class FadeScreenTransition : UI.ScreenTransition
{
    public const float FadeTime = 0.2f;
    public Image Image;

    public override void OnActive(object data)
    {
        Image.enabled = true;
        TweenUtil.SetAlpha(Image, 1);
    }

    public override void OnCloseTransition(Action onSuccess)
    {
        Image.enabled = true;
        TweenUtil.Fade(Image, 0, 1, TweenType.Linear, FadeTime, onSuccess);
    }

    public override void OnOpenTransition(Action onSuccess)
    {
        Image.enabled = true;

        TweenUtil.Fade(Image, 1, 0, TweenType.Linear, FadeTime, () =>
        {
            Image.enabled = false;
            onSuccess.Execute();
        });
    }
}