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

    private CharacterData data = null;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Image>(typeof(Images));
        Bind<TMP_Text>(typeof(Texts));

        RefreshUI();

        return true;
    }

    // 테스트
    // 데이터를 받을 수 있도록 수정
    public void SetInfo(CharacterData data)
    {
        this.data = data;
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (_init == false)
            return;

        Get<TMP_Text>((int)Texts.CharacterNameText).text = data.charName;

        // 테스트
        if (data.frontImage != null)
        {
            Get<Image>((int)Images.CharacterImage).sprite = data.frontImage;
        }
    }
}
