using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Service
{
    private static readonly HashSet<Type> Services = new HashSet<Type>();
    private readonly Queue<Action> _onDestroyQueue = new Queue<Action>();

    protected Service()
    {
        Services.Add(GetType());
    }

    public static void DestroyAllServices()
    {
        foreach (var type in Services)
        {
            try
            {
                var generic = typeof (Service<>);
                var serviceType = generic.MakeGenericType(type);
                serviceType.GetMethod("DestroyInstance").Invoke(null, null);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        Services.Clear();
    }

    public virtual void OnDestroy()
    {
        while (_onDestroyQueue.Count > 0)
        {
            _onDestroyQueue.Dequeue().Execute();
        }
    }

    protected void ExecuteOnDestroy(Action action)
    {
        _onDestroyQueue.Enqueue(action);
    }
}

public abstract class Service<T> : Service where T : Service, new()
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
                _instance = new T();
            return _instance;
        }
    }

    public static void DestroyInstance()
    {
        if (_instance != null)
        {
            _instance.OnDestroy();
            _instance = null;
        }
    }
}