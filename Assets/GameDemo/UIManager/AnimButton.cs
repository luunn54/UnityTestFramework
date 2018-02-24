using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

[DisallowMultipleComponent, ExecuteInEditMode]
public class AnimButton : ExposableMonoBehaviour, IPointerClickHandler, IPointerDownHandler
{
    private const float TweenDuration = 0.075f;

    public enum AnimTypeEnum
    {
        ScaleUp,
        ScaleDown
    }

    [ExposeProperty]
    public bool Interactable
    {
        get { return _interactable; }

        set
        {
            _interactable = value;
            UpdateColor();
        }
    }

    public Vector3 OriginalScale
    {
        get { return _originalScale; }
        set { _originalScale = value; }
    }

    public static Action<AnimButton> PlaySoundAction;
    public AnimTypeEnum AnimType;
    public float ScaleUpFactor = 1.2f;
    public float ScaleDownFactor = 0.8f;
    public GameObject Target;
    public Image DisableImage;
    public Color DisableColor = new Color(0.5f, 0.5f, 0.5f, 1);
    public Action BeforeOnClick;
    public UnityEvent OnClick;
    public bool DisableSound;
    [Header("Define button ID if need")]
    public string Id;
    private Vector3 _originalScale = Vector3.one;
    [SerializeField, HideInInspector] private bool _interactable = true;

    private void Start()
    {
        if (Target == null) Target = gameObject;
        if (Target != null) UpdateOriginalScale();
        UpdateColor();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (IsLock()) return;

        if (Interactable && !DisableSound)
        {
            PlaySoundAction.Execute(this);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsLock()) return;
        if (!Interactable) return;

        var lockId = Lock();
        OnAnimButton(Target, () => OnTweenComplete(lockId));
    }

    private void OnAnimButton(GameObject target, Action onComplete)
    {
        if (target == null)
        {
            onComplete();
        }
        else
        {
            var called = false;

            TweenCallback completeAction = () =>
            {
                if (!called)
                {
                    called = true;
                    Runner.DelayOneFrame(onComplete);
                }
            };

            var scaleFactor = AnimType == AnimTypeEnum.ScaleUp ? ScaleUpFactor : ScaleDownFactor;
            target.transform.DOScale(_originalScale*scaleFactor, TweenDuration)
                .SetLoops(2, LoopType.Yoyo).SetUpdate(true).OnComplete(completeAction);
        }
    }

    private void OnTweenComplete(long lockId)
    {
        Unlock(lockId);
        ActionUtil.ExecuteOnce(ref BeforeOnClick);
        if (OnClick != null) OnClick.Invoke();
    }

    private void UpdateColor()
    {
        if (DisableImage != null)
            DisableImage.color = _interactable ? Color.white : DisableColor;
    }

    public void UpdateOriginalScale()
    {
        _originalScale = Target.transform.localScale;
    }

    private bool IsLock()
    {
        return UIManager.Instance.IsLock();
    }

    private long Lock()
    {
        return UIManager.Instance.Lock();
    }

    private void Unlock(long lockId)
    {
        UIManager.Instance.Unlock(lockId);
    }
}