using UnityEngine.UI;

public class PopupHolder : UI.BaseUI
{
    private const float FadeTime = 0.05f;
    private const float TargetAlpha = 0.5f;
    private Image _bg;

    public UI.Popup Popup
    {
        get { return _popup; }
    }

    private UI.Popup _popup;

    public override void OnCreate()
    {
        if (_popup != null) _popup.OnCreate();
    }

    public override void OnActive(object data)
    {
        if (_popup != null) _popup.OnActive(data);
        var bg = GetBg();
        TweenUtil.Fade(bg, 0, TargetAlpha, TweenType.Linear, FadeTime);
    }

    public override void OnDeactivate()
    {
        if (_popup != null) _popup.OnDeactivate();
        var bg = GetBg();
        TweenUtil.Fade(bg, TargetAlpha, 0, TweenType.Linear, FadeTime);
    }

    public void SetPopup(UI.Popup popup)
    {
        _popup = popup;
    }

    private Image GetBg()
    {
        if (_bg == null) _bg = GetComponent<Image>();
        return _bg;
    }
}