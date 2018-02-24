using UnityEngine;
using System;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class MonoEventHandler : MonoBehaviour
{
    private readonly Queue<Action> _onEnableQueue = new Queue<Action>();
    private readonly Queue<Action> _onDestroyQueue = new Queue<Action>();

    void OnEnable()
    {
        ProcessQueue(_onEnableQueue);
    }

    void OnDestroy()
    {
        ProcessQueue(_onDestroyQueue);
    }

    public void ExecuteOnEnable(Action action)
    {
        if (gameObject.activeInHierarchy) ExecuteAction(action);
        else _onEnableQueue.Enqueue(action);
    }

    public void QueueOnDestroy(Action action)
    {
        _onDestroyQueue.Enqueue(action);
    }

    private void ExecuteAction(Action action)
    {
        if (action != null) action();
    }

    private void ProcessQueue(Queue<Action> queue)
    {
        while (queue.Count > 0) ExecuteAction(queue.Dequeue());
    }
}

public static class MonoEventHandlerExt
{
    public static void ExecuteOnEnable(this MonoBehaviour mono, Action action)
    {
        var handler = GetHandler(mono);
        handler.ExecuteOnEnable(action);
    }

    public static void QueueOnDestroy(this MonoBehaviour mono, Action action)
    {
        var handler = GetHandler(mono);
        handler.QueueOnDestroy(action);
    }

    static MonoEventHandler GetHandler(MonoBehaviour mono)
    {
        var handler = mono.GetComponent<MonoEventHandler>();
        if (handler == null)
        {
            handler = mono.gameObject.AddComponent<MonoEventHandler>();
        }

        return handler;
    }
}