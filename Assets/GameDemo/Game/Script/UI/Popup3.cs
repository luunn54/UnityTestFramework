using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Popup3 : UI.Popup
{
    public override void OnActive(object data)
    {

    }

    public void Confirm()
    {
        UIManager.Instance.Confirm("Test?", () => UIManager.Instance.Notify("OK", Close));
    }
}