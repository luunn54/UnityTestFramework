using UnityEngine;
using UnityEngine.UI;

public class NotifyTextElement : UI.Element
{
    public CanvasGroup CanvasGroup;
    public Text Text;

    public override void OnActive(object param)
    {
        Text.text = (string) param;

        TweenUtil.FadeCanvasGroup(CanvasGroup, 0, 1, TweenType.OutSine, 0.1f, () =>
        {
            Runner.Delay(0.5f, true, () =>
            {
                TweenUtil.FadeCanvasGroup(CanvasGroup, 1, 0, TweenType.InSine, 0.8f, () =>
                {
                    UIManager.Instance.HideElement<NotifyTextElement>();
                });
            });
        });
    }
}