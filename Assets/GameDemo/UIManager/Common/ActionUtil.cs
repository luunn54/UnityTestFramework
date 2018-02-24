using System;

public static class ActionUtil
{
    public static void Execute(this Action action)
    {
        if (action != null) action();
    }

    public static void Execute<T>(this Action<T> action, T data)
    {
        if (action != null) action(data);
    }

    public static void Execute<T1, T2>(this Action<T1, T2> action, T1 data1, T2 data2)
    {
        if (action != null) action(data1, data2);
    }

    public static void ExecuteOnce(ref Action action)
    {
        if (action != null)
        {
            var temp = action;
            action = null;
            temp();
        }
    }

    public static void ExecuteOnce<T>(ref Action<T> action, T data)
    {
        if (action != null)
        {
            var temp = action;
            action = null;
            temp(data);
        }
    }

    public static void ExecuteOnce<T1, T2>(ref Action<T1, T2> action, T1 data1, T2 data2)
    {
        if (action != null)
        {
            var temp = action;
            action = null;
            temp(data1, data2);
        }
    }
}