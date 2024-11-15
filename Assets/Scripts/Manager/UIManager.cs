using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager
{
    private int _order = -20;

    private Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();

    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("UI_Root");
            if (root == null)
                root = new GameObject { name = "UI_Root" };

            return root;
        }
    }

    public void Init()
    {
        EnsureEventSystem();
    }

    public void EnsureEventSystem()
    {
        if (EventSystem.current != null)
            return;

        EventSystem eventSystem = Manager.Resource.Instantiate<EventSystem>("EventSystem");
    }

    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = Utils.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        if (sort)
        {
            canvas.sortingOrder = _order++;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }

    public T ShowPopupUI<T>(string path = null, Transform parent = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(path))
        {
            path = typeof(T).Name;
        }

        GameObject go = Manager.Resource.Instantiate(path);
        T popup = Utils.GetOrAddComponent<T>(go);
        _popupStack.Push(popup);

        if (parent != null)
            go.transform.SetParent(parent);
        else
            go.transform.SetParent(Root.transform);

        // 위치설정 추가

        return popup;
    }

    public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject prefab = Manager.Resource.Load<GameObject>(name);
        T subItem = Manager.Resource.Instantiate(prefab, parent).GetOrAddComponent<T>();

        subItem.transform.localScale = Vector3.one;
        subItem.transform.localPosition = prefab.transform.position;

        return subItem;
    }

    public void ClosePopupUI()
    {
        if (_popupStack.Count == 0)
            return;

        UI_Popup popup = _popupStack.Pop();
        Manager.Resource.Destroy(popup.gameObject);
        popup = null;
        --_order;
    }

    public void ClosePopupUI(UI_Popup popup)
    {
        if (_popupStack.Count == 0)
            return;

        if (_popupStack.Peek() != popup)
        {
            Debug.Log("가장 최근에 열린 팝업이 아닙니다");
            return;
        }

        ClosePopupUI();
    }

    public T PeekPopupUI<T>() where T : UI_Popup
    {
        if (_popupStack.Count == 0)
            return null;

        return _popupStack.Peek() as T;
    }

    public void ClearPopupUI()
    {
        while(_popupStack.Count > 0)
        {
            ClosePopupUI();
        }
    }
}
