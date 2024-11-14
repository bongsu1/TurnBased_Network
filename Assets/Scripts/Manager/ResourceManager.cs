using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    private Dictionary<string, Object> _resources = new Dictionary<string, Object>(); // 필요시 분류

    private List<AssetBundle> _bundles = new List<AssetBundle>();

    private bool _assetVaild = false;
    public bool AssetVaild { get { return _assetVaild; } }

    public void Init()
    {
        LoadAssetBundle();
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
            #region 리소스 버전
            /*T resource = Resources.Load<T>(path);
            if (resource == null)
            {
                Debug.Log($"잘못된 경로 : {path}");
                return null;
            }
            _resources.Add(key, resource);
            return resource;*/
            #endregion
            #region 에셋번들 버전
            T resource = null;
            for (int i = 0; i < _bundles.Count; i++)
            {
                Debug.Log(_bundles[i]);
                resource = _bundles[i].LoadAsset<T>(path);
                if (resource != null)
                    break;
            }
            if (resource == null)
            {
                Debug.Log($"잘못된 이름 : {path}");
                return null;
            }

            _resources.Add(key, resource);
            return resource;
            #endregion
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

    #region AssetBundleLoad
    private void LoadAssetBundle()
    {
        AssetBundle prefabs = AssetBundle.LoadFromFile("Bundle/prefabs");
        AssetBundle scriptable_objects = AssetBundle.LoadFromFile("Bundle/scriptable_objects");
        AssetBundle images = AssetBundle.LoadFromFile("Bundle/images");

        _bundles.Add(prefabs);
        _bundles.Add(scriptable_objects);
        _bundles.Add(images);

        _assetVaild = true;
    }
    #endregion
}
