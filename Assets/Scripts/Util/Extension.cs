using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        return Utils.GetOrAddComponent<T>(go);
    }

    public static void BindEvent(this GameObject go, Action action, UI_Base.UIEvent type = UI_Base.UIEvent.Click)
    {
        UI_Base.BindEvent(go, action, type);
    }
}
