using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BackElement : UI.Element
{
    public override void OnActive(object data)
    {

    }

    public void Back()
    {
        UIManager.Instance.Back();
    }
}