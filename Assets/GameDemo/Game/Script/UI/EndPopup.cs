using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EndPopup : UI.Popup
{
    public Text Score;

    public override void OnActive(object data)
    {
        Score.text = string.Format("Score: {0}", data);
    }

    public override void OnTransitionFinish()
    {
        base.OnTransitionFinish();
        TutorialTrigger.Instance.Set("SHOW_END_POPUP");
    }
}