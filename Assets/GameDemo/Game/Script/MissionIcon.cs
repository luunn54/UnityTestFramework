using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MissionIcon : MonoBehaviour
{
    public Text Number;
    public int MissionNumber;

    private void Start()
    {
        Number.text = MissionNumber.ToString();
    }

    public void Click()
    {
        UIManager.Instance.SwitchScreenAndScene<PlayScreen>("Play");
    }
}