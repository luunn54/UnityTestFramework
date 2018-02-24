using System.Collections.Generic;
using AppLogEditor;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HomeScreen : UI.Screen
{
    public Text Name;
    public Text Diamond;

    //public GameObject Buttons;
    //private Vector3 _buttonOriginalPosition;
    //private Vector3 _buttonTweenPosition;

    public void OnClickSetting()
    {
        UIManager.Instance.ShowPopup<PopupSetting>();
    }

    public void StartPlay(int diamond)
    {
        if (diamond > UserAccountService.shareAccount.Diamond)
        {
            UIManager.Instance.ShowPopup<Popup1>("Not Enough diamond!");
            return;
        }

        StartCoroutine(Play(diamond));
    }

    private IEnumerator Play(int diamond)
    {
        var body = new Dictionary<string, object>()
        {
            {"diamond", diamond},
            {"user", UserAccountService.shareAccount.User},
        };

        yield return HttpService.HTTPPost("/create_room", body, data =>
        {
            UserAccountService.UpdateAccount(JsonParser.GetDict(data, "user"));

            UIManager.Instance.SwitchScreen<PlayScreen>(diamond);
        }, (i, s) =>
        {
            UIManager.Instance.ShowPopup<Popup1>(i + " : " + s);
        });
    }

    public override void OnActive(object data)
    {
        base.OnActive(data);

        Name.text = UserAccountService.shareAccount.FullName;
        Diamond.text = UserAccountService.shareAccount.Diamond.ToString();
    }

    //private void Awake()
    //{
    //    _buttonOriginalPosition = Buttons.transform.localPosition;
    //    _buttonTweenPosition = _buttonOriginalPosition;
    //    _buttonTweenPosition.y -= 150;
    //}

    //public override void OnActive(object data)
    //{
    //    UIManager.Instance.ShowElement<InfoElement>();
    //    Buttons.transform.localPosition = _buttonTweenPosition;
    //}

    //public override void OnTransitionFinish()
    //{
    //    TweenUtil.MoveLocal(Buttons, _buttonTweenPosition, _buttonOriginalPosition, TweenType.OutBack, 0.6f);
    //}

    //public void Play()
    //{
    //    UIManager.Instance.SlideScreen<MapScreen>();
    //}

    //public void Notify()
    //{
    //    UIManager.Instance.Notify("Test", () => Debug.Log("Close"));
    //}

    //public void Confirm()
    //{
    //    UIManager.Instance.Confirm("OK?", () => Debug.Log("Yes"), () => Debug.Log("Cancel"));
    //}

    //public void ShowLoading()
    //{
    //    var loadingId = UIManager.Instance.ShowLoading();
    //    Runner.Delay(2f, () => UIManager.Instance.HideLoading(loadingId));
    //}

    //public void ShowLoading2()
    //{
    //    var loadingId = UIManager.Instance.ShowLoading<Loading2>();
    //    Runner.Delay(2f, () => UIManager.Instance.HideLoading<Loading2>(loadingId));
    //}

    //public void TestPopup()
    //{
    //    UIManager.Instance.ShowPopup<Popup1>();
    //}

    //public void NotifyText()
    //{
    //    UIManager.Instance.NotifyText("Not enough ruby");
    //}
}