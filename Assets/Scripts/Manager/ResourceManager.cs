using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    private Dictionary<string, Object> _resources = new Dictionary<string, Object>(); // 필요시 분류

    public void Init()
    {
        // 초기화
    }

    public T Load<T>(string path) where T : Object
    {
        string key = $"{path}_{typeof(T)}";

        if (_resources.TryGetValue(key, out Object obj))
        {
            return obj as T;
        }
        else
        {
            T resource = Resources.Load<T>(path);
            if (resource == null)
            {
                Debug.Log($"잘못된 경로 : {path}");
                return null;
            }
            _resources.Add(key, resource);
            return resource;
        }
    }

    public GameObject Instantiate(GameObject prefab, Transform parent = null)
    {
        GameObject go = Object.Instantiate(prefab, parent);
        go.name = prefab.name;
        return go;
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject prefab = Load<GameObject>(path);
        if (prefab == null)
        {
            Debug.Log($"없는 프리팹이거나 {path} 경로가 잘못됨");
            return null;
        }

        return Instantiate(prefab, parent);
    }

    public T Instantiate<T>(T prefab, Transform parent = null) where T : Object
    {
        T obj = Object.Instantiate<T>(prefab, parent);
        return obj;
    }

    public T Instantiate<T>(string path, Transform parent = null) where T : Object
    {
        T prefab = Load<T>(path);
        if (prefab == null)
        {
            Debug.Log($"없는 프리팹이거나 {path} 경로가 잘못됨");
            return null;
        }

        return Instantiate<T>(prefab, parent);
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        Object.Destroy(go);
    }

    // 추후에 어드에서블 또는 에셋번들로 변경
}
