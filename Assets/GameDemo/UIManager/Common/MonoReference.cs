using UnityEngine;

public abstract class MonoReference<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get { return instance; }
    }

    public void Awake()
    {
        if (instance == null)
        {
            instance = gameObject.GetComponent<T>();
        }

        OnAwake();
    }

    public void OnDestroy()
    {
        OnDestroyObj();
        instance = null;
    }

    public virtual void OnAwake() { }
    public virtual void OnDestroyObj() { }
}