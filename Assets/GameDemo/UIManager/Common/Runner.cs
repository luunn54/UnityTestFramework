using UnityEngine;
using System;
using System.Collections;

public class Runner
{
    public const float BigDeltaTime = 0.5f;
    private bool _stop;

    public Runner StopOnDestroy(MonoBehaviour mono)
    {
        if (mono != null) mono.QueueOnDestroy(Stop);
        return this;
    }

    public void Stop()
    {
        _stop = true;
    }

    public Runner ExecuteDelay(float delay, bool skipTimeScale, Action action)
    {
        StartCoroutine(DelayCo(delay, skipTimeScale, action));
        return this;
    }

    private IEnumerator DelayCo(float delay, bool skipTimeScale, Action action)
    {
        while (NotStop() && delay > 0)
        {
            yield return null;
            delay -= GetDeltaTime(skipTimeScale);
        }

        CallAction(action);
    }

    public Runner ExecuteUpdateInterval(float interval, bool skipTimeScale, Action action)
    {
        StartCoroutine(UpdateIntervalCo(interval, skipTimeScale, action));
        return this;
    }

    private IEnumerator UpdateIntervalCo(float interval, bool skipTimeScale, Action action)
    {
        var time = interval;

        while (NotStop())
        {
            yield return null;
            time -= GetDeltaTime(skipTimeScale);

            if (time <= 0)
            {
                time = interval;
                CallAction(action);
            }
        }
    }

    public Runner ExecuteUpdate(Action action)
    {
        StartCoroutine(UpdateCo(action));
        return this;
    }

    private IEnumerator UpdateCo(Action action)
    {
        while (NotStop())
        {
            yield return null;
            CallAction(action);
        }
    }

    public Runner ExecuteDelayFrame(int frame, Action action)
    {
        StartCoroutine(DelayFrameCo(frame, action));
        return this;
    }

    private IEnumerator DelayFrameCo(int frame, Action action)
    {
        for (int i = 0; i < frame; i++)
        {
            yield return null;
        }

        CallAction(action);
    }

    public Runner ExecuteUpdateValue(float from, float to, float time, bool skipTimeScale, Action<float> action, Action onSuccess = null)
    {
        StartCoroutine(UpdateValueCo(from, to, time, skipTimeScale, action, onSuccess));
        return this;
    }

    private IEnumerator UpdateValueCo(float from, float to, float time, bool skipTimeScale, Action<float> action,
        Action onSuccess = null)
    {
        if (time > 0)
        {
            var curTime = 0f;
            CallAction(action, from);

            while (NotStop())
            {
                yield return null;
                curTime += GetDeltaTime(skipTimeScale);
                var t = Mathf.Clamp01(curTime/time);
                CallAction(action, Mathf.Lerp(from, to, t));
                if (t >= 1) break;
            }
        }
        else
        {
            // Neu time <= 0 thi ket thuc (gia tri to)
            CallAction(action, to);
        }

        CallAction(onSuccess);
    }

    private void CallAction(Action action)
    {
        if (NotStop() && action != null) action();
    }

    private void CallAction<T>(Action<T> action, T value)
    {
        if (NotStop() && action != null) action(value);
    }

    private bool NotStop()
    {
        return !_stop;
    }

    private float GetDeltaTime(bool skipTimeScale)
    {
        var deltaTime = skipTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;

        if (deltaTime > BigDeltaTime)
            deltaTime = 0; // Skip big delta time

        return deltaTime;
    }

    private void StartCoroutine(IEnumerator coroutine)
    {
        RunController.Instance.StartCoroutine(coroutine);
    }

    public static Runner Delay(float delay, Action action)
    {
        return Delay(delay, false, action);
    }

    public static Runner Delay(float delay, bool skipTimeScale, Action action)
    {
        return new Runner().ExecuteDelay(delay, skipTimeScale, action);
    }

    public static Runner Update(Action action)
    {
        return new Runner().ExecuteUpdate(action);
    }

    public static Runner UpdateUntil(Func<bool> condition, Action onSuccess)
    {
        Runner runner = null;

        runner = new Runner().ExecuteUpdate(() =>
        {
            if (condition())
            {
                runner.Stop();
                if (onSuccess != null) onSuccess();
            }
        });

        return runner;
    }

    public static Runner DelayFrame(int frame, Action action)
    {
        return new Runner().ExecuteDelayFrame(frame, action);
    }

    public static Runner DelayOneFrame(Action action)
    {
        return DelayFrame(1, action);
    }

    public static Runner UpdateValue(float from, float to, float time, bool skipTimeScale, Action<float> action, Action onSuccess = null)
    {
        return new Runner().ExecuteUpdateValue(from, to, time, skipTimeScale, action, onSuccess);
    }

    public static Runner UpdateValue(float from, float to, float time, Action<float> action, Action onSuccess = null)
    {
        return UpdateValue(from, to, time, false, action, onSuccess);
    }

    public static Runner UpdateInterval(float interval, bool skipTimeScale, Action action)
    {
        return new Runner().ExecuteUpdateInterval(interval, skipTimeScale, action);
    }

    public static Runner UpdateInterval(float interval, Action action)
    {
        return UpdateInterval(interval, false, action);
    }
}