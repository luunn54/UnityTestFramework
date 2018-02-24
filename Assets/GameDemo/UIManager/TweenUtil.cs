using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public enum TweenType
{
    Linear,
    InSine,
    OutSine,
    InOutSine,
    InQuad,
    OutQuad,
    InOutQuad,
    InCubic,
    OutCubic,
    InOutCubic,
    InQuart,
    OutQuart,
    InOutQuart,
    InQuint,
    OutQuint,
    InOutQuint,
    InExpo,
    OutExpo,
    InOutExpo,
    InCirc,
    OutCirc,
    InOutCirc,
    InElastic,
    OutElastic,
    InOutElastic,
    InBack,
    OutBack,
    InOutBack,
    InBounce,
    OutBounce,
    InOutBounce
}

public static class TweenUtil
{
    private static Dictionary<TweenType, Ease> _doTweenMapping;

    public static Dictionary<TweenType, Ease> DoTweenMapping
    {
        get
        {
            if (_doTweenMapping == null)
            {
                _doTweenMapping = new Dictionary<TweenType, Ease>
                {
                    {TweenType.Linear, Ease.Linear},
                    {TweenType.InSine, Ease.InSine},
                    {TweenType.OutSine, Ease.OutSine},
                    {TweenType.InOutSine, Ease.InOutSine},
                    {TweenType.InQuad, Ease.InQuad},
                    {TweenType.OutQuad, Ease.OutQuad},
                    {TweenType.InOutQuad, Ease.InOutQuad},
                    {TweenType.InCubic, Ease.InCubic},
                    {TweenType.OutCubic, Ease.OutCubic},
                    {TweenType.InOutCubic, Ease.InOutCubic},
                    {TweenType.InQuart, Ease.InQuart},
                    {TweenType.OutQuart, Ease.OutQuart},
                    {TweenType.InOutQuart, Ease.InOutQuart},
                    {TweenType.InQuint, Ease.InQuint},
                    {TweenType.OutQuint, Ease.OutQuint},
                    {TweenType.InOutQuint, Ease.InOutQuint},
                    {TweenType.InExpo, Ease.InExpo},
                    {TweenType.OutExpo, Ease.OutExpo},
                    {TweenType.InOutExpo, Ease.InOutExpo},
                    {TweenType.InCirc, Ease.InCirc},
                    {TweenType.OutCirc, Ease.OutCirc},
                    {TweenType.InOutCirc, Ease.InOutCirc},
                    {TweenType.InElastic, Ease.InElastic},
                    {TweenType.OutElastic, Ease.OutElastic},
                    {TweenType.InOutElastic, Ease.InOutElastic},
                    {TweenType.InBack, Ease.InBack},
                    {TweenType.OutBack, Ease.OutBack},
                    {TweenType.InOutBack, Ease.InOutBack},
                    {TweenType.InBounce, Ease.InBounce},
                    {TweenType.OutBounce, Ease.OutBounce},
                    {TweenType.InOutBounce, Ease.InOutBounce}
                };
            }

            return _doTweenMapping;
        }
    }

    public static void MoveLocal(GameObject obj, Vector3 from, Vector3 to, TweenType tweenType, float time,
        Action onSuccess = null)
    {
        obj.transform.localPosition = from;
        obj.transform.DOLocalMove(to, time).FinalTween(tweenType, onSuccess);
    }

    public static void Scale(GameObject obj, Vector3 from, Vector3 to, TweenType tweenType, float time,
        Action onSuccess = null)
    {
        obj.transform.localScale = from;
        obj.transform.DOScale(to, time).FinalTween(tweenType, onSuccess);
    }

    public static void Fade(Graphic graphic, float from, float to, TweenType tweenType, float time,
        Action onSuccess = null)
    {
        SetAlpha(graphic, from);
        graphic.DOFade(to, time).FinalTween(tweenType, onSuccess);
    }

    public static void FadeCanvasGroup(CanvasGroup canvasGroup, float from, float to, TweenType tweenType, float time,
        Action onSuccess = null)
    {
        canvasGroup.alpha = from;
        canvasGroup.DOFade(to, time).FinalTween(tweenType, onSuccess);
    }

    public static void SetAlpha(Graphic graphic, float alpha)
    {
        var color = graphic.color;
        color.a = alpha;
        graphic.color = color;
    }

    private static Tweener FinalTween(this Tweener tweener, TweenType tweenType, Action onSuccess)
    {
        var ease = Ease.Linear;
        if (DoTweenMapping.TryGetValue(tweenType, out ease))
            tweener.SetEase(ease);
        if (onSuccess != null)
            tweener.OnComplete(onSuccess.Execute);
        tweener.SetUpdate(true);
        return tweener;
    }
}