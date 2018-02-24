using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Popup2 : UI.Popup
{
    public override void OnActive(object data)
    {

    }

    public void ShowPopup3()
    {
        UIManager.Instance.ShowPopup<Popup3>();
    }
}