using UnityEngine;
using System.Collections;

public class Gameplay : MonoReference<Gameplay>
{
    public const float PlayTime = 5;

    public bool IsPlaying
    {
        get { return _isPlaying; }
    }

    private int _score;
    private PlayScreen _screen;
    private bool _isPlaying;

    public void Play()
    {
        //_isPlaying = true;
        //_screen = UIManager.Instance.GetScreen<PlayScreen>();
        //_screen.UpdateTime(PlayTime);
        //_screen.UpdateScore(_score);

        //Runner.UpdateValue(PlayTime, 0, PlayTime, value =>
        //{
        //    _screen.UpdateTime(value);
        //}, () =>
        //{
        //    UIManager.Instance.ShowPopup<EndPopup>(_score, popup =>
        //    {
        //        UIManager.Instance.SwitchScreenAndScene<HomeScreen>("Home");
        //    }, () =>
        //    {
        //        UIManager.Instance.SwitchScreenAndScene<HomeScreen>("Home");
        //    });
        //});

        //TutorialTrigger.Instance.Set("ON_PLAY");
    }

    public void Tap()
    {
        //_score++;
        //_screen.UpdateScore(_score);
    }
}