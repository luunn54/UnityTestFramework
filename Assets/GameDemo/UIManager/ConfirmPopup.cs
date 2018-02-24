using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class ConfirmPopupData
{
    public string Message;
    public Action<bool> OnClose;
}

public class ConfirmPopup : UI.Popup
{
    public Text Title;
    public Text Message;
    public Text ButtonYes;
    public Text ButtonNo;
    private readonly Queue<Action<bool>> _onCloses = new Queue<Action<bool>>();
    private bool _isOk;

    public override void OnActive(object data)
    {
        _isOk = false;
        var popupData = (ConfirmPopupData) data;
        SetTitle(string.Empty);
        Message.text = popupData.Message;
        if (popupData.OnClose != null)
            _onCloses.Enqueue(popupData.OnClose);
    }

    public override void OnDeactivate()
    {
        while (_onCloses.Count > 0)
            _onCloses.Dequeue().Execute(_isOk);
    }

    public void OnOk()
    {
        _isOk = true;
        Close();
    }

    public ConfirmPopup SetTitle(string text)
    {
        Title.text = text;
        return this;
    }

    public ConfirmPopup SetMessage(string text)
    {
        Message.text = text;
        return this;
    }

    public ConfirmPopup SetButtonYes(string text)
    {
        ButtonYes.text = text;
        return this;
    }

    public ConfirmPopup SetButtonNo(string text)
    {
        ButtonNo.text = text;
        return this;
    }
}