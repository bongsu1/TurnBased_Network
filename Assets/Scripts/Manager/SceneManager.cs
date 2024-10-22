using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager
{
    private BaseScene _currentScene = null;

    public void Init()
    {

    }

    public BaseScene GetCurScene()
    {
        if (_currentScene == null)
        {
            _currentScene = UnityEngine.Object.FindObjectOfType<BaseScene>();
        }

        return _currentScene;
    }

    public T GetCurScene<T>() where T : BaseScene
    {
        return GetCurScene() as T;
    }
}
