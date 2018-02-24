using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Popup1 : UI.Popup
{
    public Text text;
    public override void OnActive(object data)
    {
        text.text = data as string;
    }
}