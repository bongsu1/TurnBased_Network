using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScene : MonoBehaviour
{
    protected bool _init = false;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => Manager.Data.IsVaild && Manager.Resource.AssetVaild);
        Init();
    }

    protected virtual bool Init()
    {
        if (_init)
            return false;

        Manager.UI.EnsureEventSystem();

        return true;
    }

    public virtual void Clear() { }
}
