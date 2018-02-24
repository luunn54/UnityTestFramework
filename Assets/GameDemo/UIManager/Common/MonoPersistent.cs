using UnityEngine;

public abstract class MonoPersistent<T> : MonoBehaviour where T : MonoBehaviour
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
            DontDestroyOnLoad(gameObject);
        }

        OnAwake();
    }

    public virtual void OnAwake() { }
}