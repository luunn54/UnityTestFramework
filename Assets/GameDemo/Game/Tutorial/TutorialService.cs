using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class TutorialService : Service<TutorialService>
{
    private Type _currentTutorial;

    public void ExecuteTutorial<T>(Action onSuccess = null) where T : TutorialAction, new()
    {
        UIManager.Instance.ShowElement<TutorialElement>(null, () =>
        {
            var tutorial = new T();
            _currentTutorial = tutorial.GetType();

            tutorial.Execute(() =>
            {
                _currentTutorial = null;
                onSuccess.Execute();
            });
        });
    }

    public void ExecuteTutorialIfNotFinish<T>(Action onFinish = null) where T : TutorialAction, new()
    {
        if (IsFinishTutorial<T>())
        {
            onFinish.Execute();
        }
        else
        {
            ExecuteTutorial<T>(onFinish);
        }
    }

    public bool IsFinishTutorial<T>() where T : TutorialAction
    {
        return false; // Todo: Check status of this tutorial
    }

    public bool IsRunningTutorial()
    {
        return _currentTutorial != null;
    }

    public bool IsRunningTutorial<T>() where T : TutorialAction
    {
        return _currentTutorial != null && _currentTutorial == typeof (T);
    }

    public void SetFinishTutorial(Type type, Action onSuccess)
    {
        onSuccess.Execute(); // Todo: Save status of this tutorial
    }

    public Action ChangeParent(Transform transform, Transform newParent)
    {
        var parent = transform.parent;
        var position = transform.position;
        var localScale = transform.localScale;
        var localEulerAngles = transform.localEulerAngles;
        var siblingIndex = transform.GetSiblingIndex();
        var rectTransform = transform.GetComponent<RectTransform>();
        Action restoreRectTransform = null;

        if (rectTransform != null)
        {
            var anchoredPosition3D = rectTransform.anchoredPosition3D;
            var anchorMax = rectTransform.anchorMax;
            var anchorMin = rectTransform.anchorMin;
            var offsetMax = rectTransform.offsetMax;
            var offsetMin = rectTransform.offsetMin;
            var pivot = rectTransform.pivot;
            var sizeDelta = rectTransform.sizeDelta;

            restoreRectTransform = () =>
            {
                if (transform == null) return;
                rectTransform.anchoredPosition3D = anchoredPosition3D;
                rectTransform.anchorMax = anchorMax;
                rectTransform.anchorMin = anchorMin;
                rectTransform.offsetMax = offsetMax;
                rectTransform.offsetMin = offsetMin;
                rectTransform.pivot = pivot;
                rectTransform.sizeDelta = sizeDelta;
            };
        }

        GridLayoutGroup gridLayout = null;
        var restoreGridLayout = false;
        HorizontalLayoutGroup horizontalLayout = null;
        var restoreHorizontalLayout = false;
        VerticalLayoutGroup verticalLayout = null;
        var restoreVerticalLayout = false;

        if (parent != null)
        {
            gridLayout = parent.GetComponent<GridLayoutGroup>();

            if (gridLayout != null && gridLayout.enabled)
            {
                gridLayout.enabled = false;
                restoreGridLayout = true;
            }

            horizontalLayout = parent.GetComponent<HorizontalLayoutGroup>();

            if (horizontalLayout != null && horizontalLayout.enabled)
            {
                horizontalLayout.enabled = false;
                restoreHorizontalLayout = true;
            }

            verticalLayout = parent.GetComponent<VerticalLayoutGroup>();

            if (verticalLayout != null && verticalLayout.enabled)
            {
                verticalLayout.enabled = false;
                restoreVerticalLayout = true;
            }
        }

        transform.SetParent(newParent);
        transform.position = position;

        Action restore = () =>
        {
            if (transform == null) return;
            transform.SetParent(parent);
            transform.position = position;
            transform.localScale = localScale;
            transform.localEulerAngles = localEulerAngles;
            transform.SetSiblingIndex(siblingIndex);
            ActionUtil.ExecuteOnce(ref restoreRectTransform);

            if (gridLayout != null && restoreGridLayout)
                gridLayout.enabled = true;

            if (horizontalLayout != null && restoreHorizontalLayout)
                horizontalLayout.enabled = true;

            if (verticalLayout != null && restoreVerticalLayout)
                verticalLayout.enabled = true;
        };

        return restore;
    }

    public List<T> DisableAllComponents<T>(T skipComponent = null) where T : Behaviour
    {
        var list = new List<T>();

        foreach (var component in Object.FindObjectsOfType<T>())
        {
            if (component != null && component != skipComponent && component.isActiveAndEnabled)
            {
                component.enabled = false;
                list.Add(component);
            }
        }

        return list;
    }

    public void RestoreAllComponents<T>(List<T> components) where T : Behaviour
    {
        foreach (var component in components)
        {
            if (component != null) component.enabled = true;
        }
    }
}