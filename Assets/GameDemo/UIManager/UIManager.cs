using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UI;
using UnityEngine.SceneManagement;
using Screen = UI.Screen;

public interface IUIManager
{
    void SwitchScreen<T>(object data = null) where T : Screen;
    void SlideScreen<T>(object data = null) where T : Screen;
    void SwitchScreenAndScene<T>(string sceneName, object data = null) where T : Screen;
    void SlideScreenAndScene<T>(string sceneName, object data = null) where T : Screen;
    T GetScreen<T>() where T : Screen;
    Screen GetCurrentScreen();
    void PreloadScreen<T>(Action onSuccess = null) where T : Screen;
    void SetSwitchScreenCallback(Action<Screen> action);
    void SetScreenAction<T>(Action<Action> action) where T : Screen;
    void AddScreenAction<T>(Action<Action> action) where T : Screen;
    void SetDefaultScreenTransition<T>() where T : ScreenTransition;

    void ShowElement<T>(object data = null, Action onSuccess = null) where T : Element;
    void HideElement<T>() where T : Element;
    T GetElement<T>() where T : Element;
    bool IsElementActive<T>() where T : Element;
    void PreloadElement<T>(Action onSuccess = null) where T : Element;

    void ShowPopup<T>(object data = null) where T : Popup;
    void ShowPopup<T>(object data, Action<T> onOk, Action onCancel = null) where T : Popup;
    void Notify(string message, Action onClose = null);
    void Notify(string message, Action<NotifyPopup> onActive, Action onClose = null);
    void NotifyText(string message);
    void Confirm(string message, Action onYes, Action onNo = null);
    void Confirm(string message, Action<ConfirmPopup> onActive, Action onYes, Action onNo = null);
    T GetPopup<T>() where T : Popup;
    bool IsPopupActive<T>() where T : Popup;
    void PreloadPopup<T>(Action onSuccess = null) where T : Popup;

    void Back();
    bool HasBackScreen();
    void ClearBackHistory();
    bool EnableBackKey { get; set; }

    long Lock();
    void Unlock(long lockId);
    bool IsLock();
    long ShowLoading();
    long ShowLoading<T>() where T : Loading;
    void HideLoading(long loadingId);
    void HideLoading<T>(long loadingId) where T : Loading;
    T GetLoading<T>() where T : Loading;
    void SetDefaultLoading<T>() where T : Loading;
    bool IsLoading();

    void ClearCache();
}

public class UIManager : MonoBehaviour, IUIManager
{
    #region Instance
    public static IUIManager Instance
    {
        get { return _instance; }
    }

    private static IUIManager _instance;

    private void Awake()
    {
        StartLayer.SetActive(true);

        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }
    #endregion

    public GameObject ScreenParent;
    public GameObject ElementParent;
    public GameObject PopupParent;
    public GameObject SystemPopupParent;
    public GameObject PopupHolder;
    public GameObject ScreenTransitionParent;
    public GameObject LoadingLayer;
    public GameObject LockLayer;
    public GameObject StartLayer;
    private static long _idCounter;
    private readonly HashSet<long> _lockIds = new HashSet<long>();
    private readonly HashSet<long> _loadingIds = new HashSet<long>();
    private readonly Dictionary<Type, GameObject> _objCache = new Dictionary<Type, GameObject>();
    private readonly Stack<Action> _backPopups = new Stack<Action>();
    private readonly Stack<Action> _backSystemPopups = new Stack<Action>();
    private readonly Stack<SwitchScreenData> _screenHistory = new Stack<SwitchScreenData>();
    private readonly Dictionary<Type, ScreenAction> _screenActions = new Dictionary<Type, ScreenAction>();
    private Screen _currentScreen;
    private Action<Screen> _onSwitchScreen;
    private SwitchScreenData _lastScreenData;
    private Type _defaultLoading = typeof (DefaultLoading);
    private bool _enableBackKey = true;
    private Type _defaultScreenTransition = typeof (FadeScreenTransition);

    private void Update()
    {
        if (IsLock() || IsLoading())
            return;
        if (_enableBackKey && Input.GetKeyUp(KeyCode.Escape)) Back();
    }

    public void SwitchScreen<T>(object data) where T : Screen
    {
        SwitchScreen(typeof (T), null, data, false);
    }

    public void SlideScreen<T>(object data) where T : Screen
    {
        SlideScreen(typeof (T), data, false);
    }

    private void SwitchScreen(Type screen, Type screenTransition, object data, bool isBack)
    {
        var skipCloseTransition = _currentScreen == null;
        var lockId = Lock();

        ShowScreenTransition(screenTransition, nextTransition =>
        {
            ActiveScreen(screen, data, string.Empty, isBack, nextTransition);
        }, skipCloseTransition, () =>
        {
            Unlock(lockId);
            ScreenTransitionFinish(screen);
        });
    }

    private void SlideScreen(Type screen, object data, bool isBack)
    {
        var lockId = Lock();

        SlideScreen(screen, data, string.Empty, isBack, () =>
        {
            Unlock(lockId);
            ScreenTransitionFinish(screen);
        });
    }

    public void SwitchScreenAndScene<T>(string sceneName, object data) where T : Screen
    {
        SwitchScreenAndScene(typeof (T), null, sceneName, data, false);
    }

    public void SlideScreenAndScene<T>(string sceneName, object data) where T : Screen
    {
        SlideScreenAndScene(typeof(T), sceneName, data, false);
    }

    private void SwitchScreenAndScene(Type screen, Type screenTransition, string sceneName, object data, bool isBack)
    {
        var skipCloseTransition = _currentScreen == null;
        var lockId = Lock();

        ShowScreenTransition(screenTransition, nextTransition =>
        {
            ActiveScreen(screen, data, sceneName, isBack, () =>
            {
                LoadScene(sceneName, nextTransition);
            });
        }, skipCloseTransition, () =>
        {
            Unlock(lockId);
            ScreenTransitionFinish(screen);
        });
    }

    private void SlideScreenAndScene(Type screen, string sceneName, object data, bool isBack)
    {
        var lockId = Lock();

        SlideScreen(screen, data, sceneName, isBack, () =>
        {
            LoadScene(sceneName, () =>
            {
                Unlock(lockId);
                ScreenTransitionFinish(screen);
            });
        });
    }

    private void ScreenTransitionFinish(Type screenType)
    {
        if (_currentScreen != null)
        {
            _currentScreen.OnTransitionFinish();
        }

        CheckScreenActions(screenType);
    }

    public T GetScreen<T>() where T : Screen
    {
        var obj = CacheGet(typeof (T));
        return obj != null ? obj.GetComponent<T>() : default(T);
    }

    public Screen GetCurrentScreen()
    {
        return _currentScreen;
    }

    public void PreloadScreen<T>(Action onSuccess) where T : Screen
    {
        Preload(typeof (T), ScreenParent, onSuccess);
    }

    private void Preload(Type type, GameObject parent, Action onSuccess)
    {
        var objCache = CacheGet(type);

        if (objCache != null)
        {
            onSuccess.Execute();
        }
        else
        {
            LoadObject(type, parent, obj =>
            {
                obj.GetComponent<BaseUI>().Active(false);
                onSuccess.Execute();
            });
        }
    }

    public void SetSwitchScreenCallback(Action<Screen> action)
    {
        _onSwitchScreen = action;
    }

    public void SetScreenAction<T>(Action<Action> action) where T : Screen
    {
        var screenType = typeof (T);
        var screenAction = GetScreenAction(screenType);
        screenAction.SetMainAction(action);
        CheckScreenActions(screenType);
    }

    public void AddScreenAction<T>(Action<Action> action) where T : Screen
    {
        var screenType = typeof (T);
        var screenAction = GetScreenAction(screenType);
        screenAction.AddAction(action);
        CheckScreenActions(screenType);
    }

    private void CheckScreenActions(Type screenType)
    {
        if (_currentScreen != null && _currentScreen.GetType() == screenType)
            GetScreenAction(screenType).ProcessActions();
    }

    private ScreenAction GetScreenAction(Type screenType)
    {
        ScreenAction screenAction;
        _screenActions.TryGetValue(screenType, out screenAction);

        if (screenAction == null)
        {
            screenAction = new ScreenAction();
            _screenActions[screenType] = screenAction;
        }

        return screenAction;
    }

    public void SetDefaultScreenTransition<T>() where T : ScreenTransition
    {
        _defaultScreenTransition = typeof (T);
    }

    public void ShowElement<T>(object data, Action onSuccess) where T : Element
    {
        if (!IsElementActive<T>())
        {
            LoadObject(typeof (T), ElementParent, element =>
            {
                ActiveObject(element, data, null, onSuccess);
            });
        }
    }

    public void HideElement<T>() where T : Element
    {
        if (IsElementActive<T>())
        {
            DeactivateObject(typeof(T), null);
        }
    }

    public T GetElement<T>() where T : Element
    {
        var obj = CacheGet(typeof(T));
        return obj != null ? obj.GetComponent<T>() : default(T);
    }

    public bool IsElementActive<T>() where T : Element
    {
        var element = GetElement<T>();
        return element != null && element.IsActive();
    }

    public void PreloadElement<T>(Action onSuccess) where T : Element
    {
        Preload(typeof (T), ElementParent, onSuccess);
    }

    public void ShowPopup<T>(object data) where T : Popup
    {
        ShowPopup<T>(PopupParent, data, null, null, null);
    }

    public void ShowPopup<T>(object data, Action<T> onOk, Action onCancel) where T : Popup
    {
        ShowPopup(PopupParent, data, null, onOk, onCancel);
    }

    public void Notify(string message, Action onClose = null)
    {
        Notify(message, null, onClose);
    }

    public void Notify(string message, Action<NotifyPopup> onActive, Action onClose = null)
    {
        var data = new NotifyPopupData
        {
            Message = message,
            OnClose = onClose
        };

        ShowPopup<NotifyPopup>(SystemPopupParent, data, obj =>
        {
            var popupHolder = (PopupHolder) obj;
            onActive.Execute((NotifyPopup) popupHolder.Popup);
        }, null, null);
    }

    public void NotifyText(string message)
    {
        ShowElement<NotifyTextElement>(message, null);
    }

    public void Confirm(string message, Action onYes, Action onNo = null)
    {
        Confirm(message, null, onYes, onNo);
    }

    public void Confirm(string message, Action<ConfirmPopup> onActive, Action onYes, Action onNo = null)
    {
        var data = new ConfirmPopupData
        {
            Message = message,

            OnClose = isOk =>
            {
                if (isOk) onYes.Execute();
                else onNo.Execute();
            }
        };

        ShowPopup<ConfirmPopup>(SystemPopupParent, data, obj =>
        {
            var popupHolder = (PopupHolder) obj;
            onActive.Execute((ConfirmPopup) popupHolder.Popup);
        }, null, null);
    }

    public T GetPopup<T>() where T : Popup
    {
        var holder = CacheGet(typeof (T));
        return holder != null ? holder.GetComponent<PopupHolder>().Popup as T : default(T);
    }

    public bool IsPopupActive<T>() where T : Popup
    {
        var popup = GetPopup<T>();
        return popup != null && popup.IsActive();
    }

    public void PreloadPopup<T>(Action onSuccess) where T : Popup
    {
        Preload(typeof (T), PopupParent, onSuccess);
    }

    private void ShowPopup<T>(GameObject popupParent, object data, Action<BaseUI> onActive, Action<T> onOk, Action onCancel) where T : Popup
    {
        if (IsPopupActive<T>())
            return;

        var popupType = typeof (T);
        var showId = Lock();

        LoadObject(popupType, popupParent, holderObj =>
        {
            ActiveObject(holderObj, data, onActive, () =>
            {
                var popupHolder = holderObj.GetComponent<PopupHolder>();
                var popup = popupHolder.Popup.GetComponent<T>();
                var transition = popupHolder.Popup.GetComponent<PopupTransition>();
                popup.IsOk = false;

                Action hide = () =>
                {
                    var hideId = Lock();

                    if (transition != null)
                    {
                        transition.OnHideTransition(() => DeactivateObject(popupType, () =>
                        {
                            Unlock(hideId);
                            
                            if (popup.IsOk)
                            {
                                onOk.Execute(popup);
                            }
                            else
                            {
                                onCancel.Execute();
                            }

                            ActionUtil.ExecuteOnce(ref popup.CloseCallback);
                            CheckCloseLastPopup();
                        }));
                    }
                    else
                    {
                        DeactivateObject(popupType, () =>
                        {
                            Unlock(hideId);

                            if (popup.IsOk)
                            {
                                onOk.Execute(popup);
                            }
                            else
                            {
                                onCancel.Execute();
                            }

                            ActionUtil.ExecuteOnce(ref popup.CloseCallback);
                            CheckCloseLastPopup();
                        });
                    }
                };

                if (IsSystemPopup(popup))
                {
                    _backSystemPopups.Push(hide);
                }
                else
                {
                    _backPopups.Push(hide);
                }

                if (transition != null)
                {
                    transition.OnShowTransition(() =>
                    {
                        Unlock(showId);
                        popup.OnTransitionFinish();
                    });
                }
                else
                {
                    Unlock(showId);
                    popup.OnTransitionFinish();
                }
            });
        });
    }

    private void CheckCloseLastPopup()
    {
        if (_backSystemPopups.Count == 0 && _backPopups.Count == 0 && _currentScreen != null)
            _currentScreen.OnCloseLastPopup();
    }

    public void Back()
    {
        if (_backSystemPopups.Count > 0)
        {
            _backSystemPopups.Pop().Execute();
        }
        else if (_backPopups.Count > 0)
        {
            _backPopups.Pop().Execute();
        }
        else
        {
            CheckBackScreen();
        }
    }

    private bool IsSystemPopup(Popup popup)
    {
        return popup is NotifyPopup || popup is ConfirmPopup;
    }

    private void CheckBackScreen()
    {
        var backScreenData = GetBackScreenData();

        if (backScreenData != null)
        {
            var screenType = backScreenData.ScreenType;
            var data = backScreenData.Data;
            var sceneName = backScreenData.SceneName;

            if (!string.IsNullOrEmpty(sceneName) && sceneName != SceneManager.GetActiveScene().name)
            {
                if (backScreenData.IsSlide)
                {
                    SlideScreenAndScene(screenType, sceneName, data, true);
                }
                else
                {
                    SwitchScreenAndScene(screenType, null, sceneName, data, true);
                }
            }
            else
            {
                if (backScreenData.IsSlide)
                {
                    SlideScreen(screenType, data, true);
                }
                else
                {
                    SwitchScreen(screenType, null, data, true);
                }
            }
        }
        else
        {
            Confirm("Do you want to quit?", () => Runner.DelayOneFrame(Application.Quit));
        }
    }

    public bool HasBackScreen()
    {
        return _screenHistory.Count > 0;
    }

    public void ClearBackHistory()
    {
        _screenHistory.Clear();
    }

    public bool EnableBackKey
    {
        get { return _enableBackKey; }
        set { _enableBackKey = value; }
    }

    private SwitchScreenData GetBackScreenData()
    {
        return _screenHistory.Count > 0 ? _screenHistory.Pop() : null;
    }

    private void ActiveScreen(Type screenType, object data, string sceneName, bool isBack, Action onSuccess)
    {
        Action active = () =>
        {
            LoadObject(screenType, ScreenParent, obj =>
            {
                ActiveObject(obj, data, null, () =>
                {
                    _currentScreen = obj.GetComponent<Screen>();

                    if (isBack)
                    {
                        _currentScreen.OnBack();
                    }
                    else
                    {
                        if (_lastScreenData != null)
                            _screenHistory.Push(_lastScreenData);
                    }

                    _lastScreenData = new SwitchScreenData
                    {
                        ScreenType = screenType,
                        Data = data,
                        SceneName = sceneName
                    };

                    _onSwitchScreen.Execute(_currentScreen);
                    onSuccess.Execute();
                });
            });
        };

        if (_currentScreen != null)
        {
            DeactivateObject(_currentScreen.GetType(), active);
        }
        else
        {
            active();
        }
    }

    private void SlideScreen(Type screenType, object data, string sceneName, bool isBack, Action onSuccess)
    {
        var currentScreen = GetCurrentScreen();

        LoadObject(screenType, ScreenParent, obj =>
        {
            var uiRect = GetComponent<RectTransform>();
            var screenRect = obj.GetComponent<RectTransform>();
            var screenParentRect = ScreenParent.GetComponent<RectTransform>();

            ActiveObject(obj, data, null, () =>
            {
                screenRect.anchoredPosition = new Vector2(isBack ? -uiRect.sizeDelta.x : uiRect.sizeDelta.x, 0);
                var to = new Vector3(isBack ? uiRect.sizeDelta.x : -uiRect.sizeDelta.x, 0, 0);

                TweenUtil.MoveLocal(ScreenParent.gameObject, Vector3.zero, to, TweenType.Linear, 0.5f, () =>
                {
                    if (currentScreen != null)
                        DeactivateObject(currentScreen.GetType(), null);
                    screenParentRect.anchoredPosition = Vector2.zero;
                    screenRect.anchoredPosition = Vector2.zero;
                    _currentScreen = obj.GetComponent<Screen>();

                    if (isBack)
                    {
                        _currentScreen.OnBack();
                    }
                    else
                    {
                        if (_lastScreenData != null)
                        {
                            _lastScreenData.IsSlide = true;
                            _screenHistory.Push(_lastScreenData);
                        }
                    }

                    _lastScreenData = new SwitchScreenData
                    {
                        ScreenType = screenType,
                        Data = data,
                        SceneName = sceneName
                    };

                    _onSwitchScreen.Execute(_currentScreen);
                    onSuccess.Execute();
                });
            });
        });
    }

    private void ShowScreenTransition(Type type, Action<Action> transitionAction, bool skipCloseTransition,
        Action transitionFinish)
    {
        if (type == null) type = _defaultScreenTransition;

        LoadObject(type, ScreenTransitionParent, transitionObj =>
        {
            ActiveObject(transitionObj, null, null, () =>
            {
                if (StartLayer.activeSelf) StartLayer.SetActive(false);
                var transition = transitionObj.GetComponent<ScreenTransition>();

                if (skipCloseTransition)
                {
                    transitionAction.Execute(() => OpenScreenTransition(transition, transitionFinish));
                }
                else
                {
                    transition.OnCloseTransition(() =>
                    {
                        transitionAction.Execute(() => OpenScreenTransition(transition, transitionFinish));
                    });
                }
            });
        });
    }

    private void OpenScreenTransition(ScreenTransition transition, Action transitionFinish)
    {
        transition.OnOpenTransition(() => DeactivateObject(transition.GetType(), transitionFinish));
    }

    private void LoadObject(Type type, GameObject parent, Action<GameObject> onSuccess)
    {
        var objCache = CacheGet(type);

        if (objCache != null)
        {
            onSuccess.Execute(objCache);
        }
        else
        {
            LoadResource("UI/" + type, prefab =>
            {
                if (_objCache.ContainsKey(type))
                    return; // Prevent duplicate object

                GameObject obj;

                if (type.IsSubclassOf(typeof (Popup)))
                {
                    obj = UIUtil.Clone(PopupHolder, parent);
                    var popup = UIUtil.Clone(prefab, obj).GetComponent<Popup>();
                    obj.GetComponent<PopupHolder>().SetPopup(popup);
                }
                else
                {
                    obj = UIUtil.Clone(prefab, parent);
                }

                obj.GetComponent<BaseUI>().OnCreate();
                if (!_objCache.ContainsKey(type))
                    _objCache.Add(type, obj);
                onSuccess.Execute(obj);
            });
        }
    }

    private GameObject CacheGet(Type type)
    {
        GameObject obj;
        _objCache.TryGetValue(type, out obj);
        return obj;
    }

    private void ActiveObject(GameObject obj, object data, Action<BaseUI> onActive, Action onSuccess)
    {
        var baseUi = obj.GetComponent<BaseUI>();
        baseUi.Active(true);
        baseUi.OnActive(data);
        onActive.Execute(baseUi);
        onSuccess.Execute();
    }

    private void DeactivateObject(Type type, Action onSuccess)
    {
        var obj = CacheGet(type);

        if (obj != null)
        {
            var baseUi = obj.GetComponent<BaseUI>();
            baseUi.Active(false);
            baseUi.OnDeactivate();
            onSuccess.Execute();
        }
    }

    private void LoadScene(string sceneName, Action onSuccess)
    {
        StartCoroutine(LoadSceneAsyncCo(sceneName, onSuccess));
    }

    private void LoadResource(string path, Action<GameObject> onSuccess)
    {
        StartCoroutine(LoadResourceAsyncCo(path, onSuccess));
    }

    private IEnumerator LoadResourceAsyncCo(string path, Action<GameObject> onSuccess)
    {
        var request = Resources.LoadAsync<GameObject>(path);
        yield return request;
        onSuccess.Execute(request.asset as GameObject);
    }

    private IEnumerator LoadSceneAsyncCo(string sceneName, Action onSuccess)
    {
        var request = SceneManager.LoadSceneAsync(sceneName);
        yield return request;
        onSuccess.Execute();
    }

    private long NewId()
    {
        _idCounter++;
        return _idCounter;
    }

    public long Lock()
    {
        var lockId = NewId();
        _lockIds.Add(lockId);
        UpdateLockLayer();
        return lockId;
    }

    public void Unlock(long lockId)
    {
        _lockIds.Remove(lockId);
        UpdateLockLayer();
    }

    public bool IsLock()
    {
        return _lockIds.Count > 0;
    }

    private void UpdateLockLayer()
    {
        LockLayer.SetActive(IsLock());
    }

    public long ShowLoading()
    {
        return ShowLoading(_defaultLoading);
    }

    public long ShowLoading<T>() where T : Loading
    {
        return ShowLoading(typeof (T));
    }

    public long ShowLoading(Type loadingType)
    {
        var loadingId = NewId();
        _loadingIds.Add(loadingId);
        UpdateLoadingLayer();

        LoadObject(loadingType, LoadingLayer, loading =>
        {
            ActiveObject(loading, null, null, null);
        });

        return loadingId;
    }

    public void HideLoading(long loadingId)
    {
        HideLoading(_defaultLoading, loadingId);
    }

    public void HideLoading<T>(long loadingId) where T : Loading
    {
        HideLoading(typeof (T), loadingId);
    }

    public T GetLoading<T>() where T : Loading
    {
        var obj = CacheGet(typeof(T));
        return obj != null ? obj.GetComponent<T>() : default(T);
    }

    public void SetDefaultLoading<T>() where T : Loading
    {
        _defaultLoading = typeof (T);
    }

    private void HideLoading(Type loadingType, long loadingId)
    {
        _loadingIds.Remove(loadingId);
        UpdateLoadingLayer();
        DeactivateObject(loadingType, null);
    }

    private void UpdateLoadingLayer()
    {
        LoadingLayer.SetActive(IsLoading());
    }

    public bool IsLoading()
    {
        return _loadingIds.Count > 0;
    }

    public void ClearCache()
    {
        var clearDict = new Dictionary<Type, GameObject>();

        foreach (var item in _objCache)
        {
            var obj = item.Value;

            if (obj != null)
            {
                var baseUi = obj.GetComponent<BaseUI>();
                if (baseUi != null && !baseUi.IsActive())
                    clearDict.Add(item.Key, item.Value);
            }
        }

        foreach (var item in clearDict)
        {
            _objCache.Remove(item.Key);
            Destroy(item.Value);
        }
    }
}