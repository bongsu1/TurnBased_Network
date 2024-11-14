using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 디버깅용도 모바일내에서 확인이 어려워서 사용, USB 디버깅이 안되는 상황에서 사용
/// </summary>
public class UI_LogError : UI_Base
{
    enum Images { Image,}
    enum Texts { LogText,}

    string log = null;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Image>(typeof(Images));
        Bind<TMP_Text>(typeof(Texts));

        Get<Image>((int)Images.Image).gameObject.BindEvent(OnClick);
        RefreshUI();

        return true;
    }

    void OnClick()
    {
        Manager.Resource.Destroy(gameObject);
    }

    public void SetInfo(string log)
    {
        this.log = log;

        RefreshUI();
    }

    private void RefreshUI()
    {
        if (_init == false)
            return;

        Get<TMP_Text>((int)Texts.LogText).text = log;
    }
}
