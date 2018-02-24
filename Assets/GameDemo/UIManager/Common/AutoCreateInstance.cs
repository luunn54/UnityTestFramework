using UnityEngine;

public class AutoCreateInstance<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                var obj = new GameObject(typeof(T).Name);
                DontDestroyOnLoad(obj);
                _instance = obj.AddComponent<T>();
            }

            return _instance;
        }
    }
}