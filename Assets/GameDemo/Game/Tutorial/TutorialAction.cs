using System;
using UnityEngine;
using System.Linq;

public abstract class TutorialAction
{
    public abstract void OnExecute();
    private Action _onSuccess;

    public void Execute(Action onSuccess = null)
    {
        var enableBackKey = UIManager.Instance.EnableBackKey;
        UIManager.Instance.EnableBackKey = false; // Disable back key

        _onSuccess = () =>
        {
            UIManager.Instance.EnableBackKey = enableBackKey; // Restore back key status
            onSuccess.Execute();
        };

        OnExecute();
    }

    protected void Finish()
    {
        TutorialService.Instance.SetFinishTutorial(GetType(), () =>
        {
            ActionUtil.ExecuteOnce(ref _onSuccess);
        });
    }

    protected TutorialPointer ShowPointerAnimButton(string buttonId, Action onHide)
    {
        var button = UnityEngine.Object.FindObjectsOfType<AnimButton>()
            .FirstOrDefault(x => x.Id == buttonId && x.gameObject.activeInHierarchy);

        if (button != null)
        {
            var pointer = ShowPointer(button.gameObject, true, onHide);
            button.BeforeOnClick = pointer.HideAction;
            return pointer;
        }
        else
        {
            Debug.LogError("Cannot find button: " + buttonId);
        }

        return null;
    }

    protected TutorialPointer ShowPointerAndChangeParent(GameObject obj, string triggerName, Action onHide)
    {
        var pointer = ShowPointer(obj, true, onHide);
        OnTrigger(triggerName, pointer.HideAction);
        return pointer;
    }

    protected TutorialPointer ShowPointer(GameObject obj, string triggerName, Action onHide)
    {
        var pointer = ShowPointer(obj, false, onHide);
        OnTrigger(triggerName, pointer.HideAction);
        return pointer;
    }

    private TutorialPointer ShowPointer(GameObject obj, bool changeParent, Action onHide)
    {
        var element = UIManager.Instance.GetElement<TutorialElement>();
        Action restoreParent = null;

        if (changeParent)
        {
            restoreParent = TutorialService.Instance.ChangeParent(obj.transform, element.ObjectParent);
            element.ShowLockLayer();
        }

        element.ShowPointer(obj);

        Action hideAction = () =>
        {
            ActionUtil.ExecuteOnce(ref restoreParent);
            element.HideLockLayer();
            element.HidePointer();
            onHide.Execute();
        };

        var pointer = new TutorialPointer
        {
            Pointer = element.Pointer,
            HideAction = hideAction
        };

        return pointer;
    }

    protected void OnTrigger(string trigger, Action onSuccess)
    {
        Runner.UpdateUntil(() => TutorialTrigger.Instance.ContainAndRemove(trigger), onSuccess);
    }

    protected void LockUntilTrigger(string trigger, Action onSuccess)
    {
        var lockId = Lock();

        Runner.UpdateUntil(() => TutorialTrigger.Instance.ContainAndRemove(trigger), () =>
        {
            Unlock(lockId);
            onSuccess.Execute();
        });
    }

    protected void OnShowScreen<T>(Action onShow) where T : UI.Screen
    {
        UIManager.Instance.AddScreenAction<T>(onSuccess =>
        {
            onShow.Execute();
            onSuccess.Execute();
        });
    }

    protected long Lock()
    {
        return UIManager.Instance.Lock();
    }

    protected void Unlock(long lockId)
    {
        UIManager.Instance.Unlock(lockId);
    }
}