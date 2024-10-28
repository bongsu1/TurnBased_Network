using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Manager.UI.ShowPopupUI<UI_TitlePopup>();
        Debug.Log($"{name} : Init");
        return true;
    }
}
