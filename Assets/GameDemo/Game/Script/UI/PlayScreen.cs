using System.Collections.Generic;
using AppLogEditor;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayScreen : UI.Screen
{
    public Button[] btns;
    //public Text Time;
    //public Text Score;
    //public Text TapStart;
    public enum GameState
    {
        YOU_WIN,
        DRAW,
        BOT_WIN,
        PLAYING
    }

    public override void OnActive(object data)
    {
        //Time.text = string.Empty;
        //Score.text = string.Empty;
        //TapStart.text = "Tap on screen to start";

        foreach (var btn in btns)
        {
            btn.GetComponent<Button>().interactable = true;
            btn.GetComponentInChildren<Text>().text = string.Empty;
        }
    }

    public void OnTap(int index)
    {
        int row = index/10;
        int col = index%10;

        StartCoroutine(Set(row, col));
    }

    private IEnumerator Set(int x, int y)
    {
        var body = new Dictionary<string, object>()
        {
            {"x", x},
            {"y", y},
            {"user", UserAccountService.shareAccount.User},
        };

        yield return HttpService.HTTPPost("/set", body, data =>
        {
            UserAccountService.UpdateAccount(JsonParser.GetDict(data, "user"));

            UpdateCell(x, y, true);
            x = JsonParser.GetInt(data, "x", -1);
            y = JsonParser.GetInt(data, "y", -1);

            if (x != -1 && y != -1)
            {
                UpdateCell(x, y, false);
            }

            var gameState = JsonParser.GetEnum<GameState>(data, "game_status", GameState.PLAYING);
            switch (gameState)
            {
                    case GameState.PLAYING:
                    break;
                    case GameState.DRAW:
                    UIManager.Instance.ShowPopup<Popup1>("Draw", null, () =>
                    {
                        UIManager.Instance.Back();
                    });
                    break;

                    case GameState.YOU_WIN:
                    UIManager.Instance.ShowPopup<Popup1>("You win", null, () =>
                    {
                        UIManager.Instance.Back();
                    });
                    break;

                    case GameState.BOT_WIN:
                    UIManager.Instance.ShowPopup<Popup1>("Bot win!", null, () =>
                    {
                        UIManager.Instance.Back();
                    });
                    break;
            }
        }, (i, s) =>
        {
            UIManager.Instance.ShowPopup<Popup1>(i + " : " + s);
        });
    }

    private void UpdateCell(int x, int y, bool isMine)
    {
        var btn = btns[x * 3 + y].gameObject;
        btn.GetComponent<Button>().interactable = false;

        btn.GetComponentInChildren<Text>().text = isMine ? "X" : "O";
    }
}