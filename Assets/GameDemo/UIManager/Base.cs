using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class BaseUI : MonoBehaviour
    {
        public virtual void OnCreate() { }
        public virtual void OnActive(object data) { }
        public virtual void OnDeactivate() { }

        private bool _isActive;
        private readonly Vector3 _inactivePosition = new Vector3(0, -10000);
        private CanvasGroup _canvasGroup;

        public bool IsActive()
        {
            return _isActive;
        }

        public void Active(bool isActive)
        {
            SetIsActive(isActive);
            var type = GetType();
            if (type == typeof(PopupHolder))
                GetComponent<PopupHolder>().Popup.SetIsActive(isActive);

            if (type.IsSubclassOf(typeof (Screen)) || type == typeof (PopupHolder))
            {
                transform.localPosition = isActive ? Vector3.zero : _inactivePosition;
                CanvasGroup.alpha = isActive ? 1 : 0;
                CanvasGroup.interactable = isActive;
                CanvasGroup.blocksRaycasts = isActive;
            }
            else
            {
                gameObject.SetActive(isActive);
            }

            if (isActive) transform.SetAsLastSibling();
        }

        public void SetIsActive(bool isActive)
        {
            _isActive = isActive;
        }

        private CanvasGroup CanvasGroup
        {
            get
            {
                if (_canvasGroup == null)
                    _canvasGroup = GetComponent<CanvasGroup>();
                if (_canvasGroup == null)
                    _canvasGroup = gameObject.AddComponent<CanvasGroup>();
                return _canvasGroup;
            }
        }
    }

    public class Screen : BaseUI
    {
        public virtual void OnTransitionFinish() { }
        public virtual void OnBack() { }
        public virtual void OnCloseLastPopup() { }
    }

    public class Popup : BaseUI
    {
        public Action CloseCallback;
        public bool IsOk { get; set; }
        public virtual void OnTransitionFinish() { }

        public void Close()
        {
            UIManager.Instance.Back();
        }

        public void Close(Action callback)
        {
            CloseCallback = callback;
            Close();
        }

        public void OnOk()
        {
            IsOk = true;
            Close();
        }

        public void OnOk(Action callback)
        {
            IsOk = true;
            CloseCallback = callback;
            Close();
        }
    }

    public class Element : BaseUI
    {

    }

    public abstract class ScreenTransition : BaseUI
    {
        public abstract void OnCloseTransition(Action onSuccess);
        public abstract void OnOpenTransition(Action onSuccess);
    }

    public abstract class Loading : BaseUI
    {
        
    }

    [DisallowMultipleComponent]
    public abstract class PopupTransition : MonoBehaviour
    {
        public abstract void OnShowTransition(Action onSuccess);
        public abstract void OnHideTransition(Action onSuccess);
    }

    public class SwitchScreenData
    {
        public Type ScreenType;
        public object Data;
        public string SceneName;
        public bool IsSlide;
    }

    public class ScreenAction
    {
        private Action<Action> _mainAction;
        private Queue<Action<Action>> _actionQueue;

        public void SetMainAction(Action<Action> action)
        {
            _mainAction = action;
        }

        public void AddAction(Action<Action> action)
        {
            if (_actionQueue == null)
                _actionQueue = new Queue<Action<Action>>();
            _actionQueue.Enqueue(action);
        }

        public void ProcessActions(Action onSuccess = null)
        {
            ProcessAction(_mainAction, () => ProcessActionQueue(_actionQueue, onSuccess));
        }

        private static void ProcessAction(Action<Action> action, Action onSuccess)
        {
            if (action != null)
            {
                Action callback = () =>
                {
                    var lockId = UIManager.Instance.Lock(); // Prevent user click

                    Runner.Delay(UIConfig.ScreenActionDelay, true, () =>
                    {
                        UIManager.Instance.Unlock(lockId);
                        onSuccess.Execute();
                    });
                };

                action(callback);
            }
            else
            {
                onSuccess.Execute();
            }
        }

        private static void ProcessActionQueue(Queue<Action<Action>> queue, Action onSuccess)
        {
            if (queue != null && queue.Count > 0)
            {
                var action = queue.Dequeue();
                ProcessAction(action, () => ProcessActionQueue(queue, onSuccess));
            }
            else
            {
                onSuccess.Execute();
            }
        }
    }
}