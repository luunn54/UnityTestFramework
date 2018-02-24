using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InfoElement : UI.Element
{
    public Text Ruby;
    public Text Gold;

    private void Start()
    {

    }

    public override void OnActive(object data)
    {
        Ruby.text = "Ruby: 100";
        Gold.text = "Gold: 200";
    }
}