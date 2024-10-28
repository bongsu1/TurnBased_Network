using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BattlePopup : UI_Popup
{
    private enum Buttons
    {
        FirstSkillButton,
        SecondSkillButton,
        ThirdSkillButton,
        My1stCharacterButton,
        My2ndCharacterButton,
        My3rdCharacterButton,
        My4thCharacterButton,
        Other1stCharacterButton,
        Other2ndCharacterButton,
        Other3rdCharacterButton,
        Other4thCharacterButton,
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Button>(typeof(Buttons));

        return true;
    }
}
