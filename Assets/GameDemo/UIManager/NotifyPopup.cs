using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class NotifyPopupData
{
    public string Message;
    public Action OnClose;
}

public class NotifyPopup : UI.Popup
{
    public Text Title;
    public Text Message;
    public Text ButtonText;
    private readonly Queue<Action> _onCloses = new Queue<Action>();

    public override void OnActive(object data)
    {
        var popupData = (NotifyPopupData) data;
        SetTitle(string.Empty);
        SetMessage(popupData.Message);
        if (popupData.OnClose != null)
            _onCloses.Enqueue(popupData.OnClose);
    }

    public override void OnDeactivate()
    {
        while (_onCloses.Count > 0)
            _onCloses.Dequeue().Execute();
    }

    public NotifyPopup SetTitle(string text)
    {
        Title.text = text;
        return this;
    }

    public NotifyPopup SetMessage(string text)
    {
        Message.text = text;
        return this;
    }

    public NotifyPopup SetButtonText(string text)
    {
        ButtonText.text = text;
        return this;
    }
}