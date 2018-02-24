using UnityEngine;
using UnityEngine.UI;

public class LoadingTemplate : UI.Loading
{
    public GameObject Icon;

    private void Update()
    {
        Icon.transform.Rotate(0, 0, -540*Time.unscaledDeltaTime);
    }
}