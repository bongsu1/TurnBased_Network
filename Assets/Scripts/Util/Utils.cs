using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static T GetOrAddComponent<T>(GameObject go) where T : Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();

        return component;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : Object
    {
        if (go == null)
            return null;

        if (recursive)
        {
            foreach(T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }
        else
        {
            Transform transform = go.transform.Find(name);
            if (transform != null)
                return transform.GetComponent<T>();
        }

        return null;
    }

    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        GameObject gameObject = FindChild<Transform>(go, name, recursive).gameObject;
        return gameObject;
    }
}
