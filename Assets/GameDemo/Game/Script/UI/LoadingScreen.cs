using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadingScreen : UI.Screen
{
    public GameObject LoadingIcon;

    public override void OnActive(object data)
    {
        Init();
    }

    private void Init()
    {
        UIManager.Instance.SetSwitchScreenCallback(screen =>
        {
            if (screen is HomeScreen || screen is PlayScreen)
                UIManager.Instance.ClearBackHistory();

            if (UIManager.Instance.HasBackScreen())
            {
                UIManager.Instance.ShowElement<BackElement>();
            }
            else
            {
                UIManager.Instance.HideElement<BackElement>();
            }
        });

        Runner.Delay(2, () => UIManager.Instance.SwitchScreenAndScene<HomeScreen>("Home"));

        UIManager.Instance.AddScreenAction<HomeScreen>(onSuccess =>
        {
            UIManager.Instance.Notify("Hello 1", onSuccess);
        });

        UIManager.Instance.AddScreenAction<HomeScreen>(onSuccess =>
        {
            UIManager.Instance.Notify("Hello 2", onSuccess);
        });

        UIManager.Instance.AddScreenAction<HomeScreen>(onSuccess =>
        {
            TutorialService.Instance.ExecuteTutorialIfNotFinish<SampleTutorial>(onSuccess);
        });

        UIManager.Instance.SetScreenAction<HomeScreen>(onSuccess =>
        {
            Debug.Log("Check home");
            onSuccess.Execute();
        });

        UIManager.Instance.AddScreenAction<MapScreen>(onSuccess =>
        {
            UIManager.Instance.Notify("Hello MapScreen", onSuccess);
        });
    }

    private void Update()
    {
        LoadingIcon.transform.Rotate(0, 0, -540*Time.deltaTime);
    }
}