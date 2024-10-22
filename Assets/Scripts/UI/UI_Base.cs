using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Base : MonoBehaviour
{
    public enum UIEvent
    {
        Click,
        Pressed,
        PointerDown,
        PointerUp,
    }

    private Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

    protected bool _init = false;

    protected virtual bool Init()
    {
        if (_init)
            return false;

        // 초기화

        return _init = true;
    }

    private void Start()
    {
        Init();
    }

    /// <summary>
    /// Enum을 사용하여 바인딩
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumType"></param>
    protected void Bind<T>(Type enumType) where T : UnityEngine.Object
    {
        if (enumType.IsEnum == false)
            return;

        string[] names = Enum.GetNames(enumType);
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
        _objects.Add(typeof(T), objects);

        for (int i = 0; i < names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
                objects[i] = Utils.FindChild(gameObject, names[i], true);
            else
                objects[i] = Utils.FindChild<T>(gameObject, names[i], true);

            if (objects[i] == null)
                Debug.Log($"{names[i]} : 바인딩 실패");
        }
    }

    /// <summary>
    /// Enum을 통해 인덱스로 접근
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="idx"></param>
    /// <returns></returns>
    protected T Get<T>(int idx) where T : UnityEngine.Object
    {
        if (_objects.TryGetValue(typeof(T), out UnityEngine.Object[] objects) == false)
            return null;

        return objects[idx] as T;
    }

    public static void BindEvent(GameObject go, Action action, UIEvent type = UIEvent.Click)
    {
        UI_EventHandler evt = Utils.GetOrAddComponent<UI_EventHandler>(go);

        switch (type)
        {
            case UIEvent.Click:
                evt.onClickHandler -= action;
                evt.onClickHandler += action;
                break;
            case UIEvent.Pressed:
                evt.onPressedHandler -= action;
                evt.onPressedHandler += action;
                break;
            case UIEvent.PointerDown:
                evt.onPointerDownHandler -= action;
                evt.onPointerDownHandler += action;
                break;
            case UIEvent.PointerUp:
                evt.onPointerUpHandler -= action;
                evt.onPointerUpHandler += action;
                break;
            default:
                Debug.Log("이벤트핸들러 매칭 안됨");
                break;
        }
    }
}
