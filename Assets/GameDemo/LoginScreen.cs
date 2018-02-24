using System.Collections;
using System.Collections.Generic;
using AppLogEditor;
using UnityEngine;
using UnityEngine.UI;

public class LoginScreen : UI.Screen
{

    public InputField userInput;
    public InputField passInput;

	// Use this for initialization
    public override void OnActive(object data)
    {
        base.OnActive(data);
        UserAccountService.Logout();
    }

    public void OnLogin()
    {
        var user = userInput.text;
        var pass = passInput.text;

        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
        {
            UIManager.Instance.ShowPopup<Popup1>("Input User and Password!");
            return;
        }

        StartCoroutine(Login(user, pass));
    }

    private IEnumerator Login(string user, string pass)
    {
        var body = new Dictionary<string, object>()
        {
            {"user", user},
            {"pw", pass}
        };

        yield return HttpService.HTTPPost("/login", body, data =>
        {
            UserAccountService.UpdateAccount(JsonParser.GetDict(data, "user"));
            UserAccountService.shareAccount.User = user;

            UIManager.Instance.SwitchScreen<HomeScreen>();
        }, (i, s) =>
        {
            UIManager.Instance.ShowPopup<Popup1>(i + " : " + s);
        });
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
