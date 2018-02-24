using UnityEngine;
using System;

public static class RunnerExtension
{
    public static Runner Delay(this MonoBehaviour mono, float delay, bool skipTimeScale, Action action)
    {
        return Runner.Delay(delay, skipTimeScale, action).StopOnDestroy(mono);
    }

    public static Runner Delay(this MonoBehaviour mono, float delay, Action action)
    {
        return Runner.Delay(delay, false, action).StopOnDestroy(mono);
    }

    public static Runner Update(this MonoBehaviour mono, Action action)
    {
        return Runner.Update(action).StopOnDestroy(mono);
    }

    public static Runner UpdateUntil(this MonoBehaviour mono, Func<bool> condition, Action onSuccess)
    {
        return Runner.UpdateUntil(condition, onSuccess).StopOnDestroy(mono);
    }

    public static Runner DelayFrame(this MonoBehaviour mono, int frame, Action action)
    {
        return Runner.DelayFrame(frame, action).StopOnDestroy(mono);
    }

    public static Runner DelayOneFrame(this MonoBehaviour mono, Action action)
    {
        return Runner.DelayOneFrame(action).StopOnDestroy(mono);
    }

    public static Runner UpdateValue(this MonoBehaviour mono, float from, float to, float time, bool skipTimeScale,
        Action<float> action,
        Action onSuccess = null)
    {
        return Runner.UpdateValue(from, to, time, skipTimeScale, action, onSuccess).StopOnDestroy(mono);
    }

    public static Runner UpdateValue(this MonoBehaviour mono, float from, float to, float time, Action<float> action,
        Action onSuccess = null)
    {
        return Runner.UpdateValue(from, to, time, action, onSuccess).StopOnDestroy(mono);
    }

    public static Runner UpdateInterval(this MonoBehaviour mono, float interval, bool skipTimeScale, Action action)
    {
        return Runner.UpdateInterval(interval, skipTimeScale, action).StopOnDestroy(mono);
    }

    public static Runner UpdateInterval(this MonoBehaviour mono, float interval, Action action)
    {
        return Runner.UpdateInterval(interval, action).StopOnDestroy(mono);
    }
}