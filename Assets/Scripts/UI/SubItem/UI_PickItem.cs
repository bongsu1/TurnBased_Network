using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PickItem : UI_Base
{
    private enum Images
    {
        CharacterImage,
    }

    private enum Texts
    {
        CharacterNameText,
    }

    // 테스트
    // character data 필요
    private string characterName = null;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Image>(typeof(Images));
        Bind<TMP_Text>(typeof(Texts));

        gameObject.BindEvent(OnClickPickItem);

        RefreshUI();

        return true;
    }

    // 테스트
    // 데이터를 받을 수 있도록 수정
    public void SetInfo(string name)
    {
        characterName = name;
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (_init == false)
            return;

        Get<TMP_Text>((int)Texts.CharacterNameText).text = characterName;
    }

    private void OnClickPickItem() { Debug.Log($"pickItem : {characterName}"); }
}
