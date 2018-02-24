using UnityEngine;

public class DefaultLoading : UI.Loading
{
    public GameObject Icon;

    private void Update()
    {
        Icon.transform.Rotate(0, 0, -540*Time.unscaledDeltaTime);
    }
}