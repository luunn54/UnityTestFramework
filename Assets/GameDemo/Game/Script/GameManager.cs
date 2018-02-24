using TestFramework;
using UnityEngine;
using System.Collections;

public class GameManager : MonoPersistent<GameManager>
{
    private void Start()
    {
        TestFramework.TestFramework.Init();

        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        yield return null;

        UIManager.Instance.SetSwitchScreenCallback(screen =>
        {
            //            if (screen is HomeScreen || screen is PlayScreen)
            //                UIManager.Instance.ClearBackHistory();

            var screenName = screen.GetType().ToString();
            //Debug.LogError(screenName);

            var contextRecord = Object.FindObjectOfType<UIContextRecordComponent>();
            if (contextRecord != null)
            {
                contextRecord.UIContextDidChange(screenName);
            }

            var contextReplay = Object.FindObjectOfType<UIContextReplayComponent>();
            if (contextReplay != null)
            {
                contextReplay.UIContextDidChange(screenName);
            }

            if (UIManager.Instance.HasBackScreen())
            {
                UIManager.Instance.ShowElement<BackElement>();
            }
            else
            {
                UIManager.Instance.HideElement<BackElement>();
            }
        });

        UIManager.Instance.SwitchScreen<LoginScreen>();
    }
}