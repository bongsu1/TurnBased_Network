using UnityEngine;
using UnityEngine.UI;

public class UI_NetworkNotification : UI_Popup
{
    private enum Images
    {
        BG,
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Image>(typeof(Images));

        Get<Image>((int)Images.BG).gameObject.BindEvent(OnClickBG);

        return true;
    }

    private void OnClickBG()
    {
        Manager.UI.ClearPopupUI();
        Manager.UI.ShowPopupUI<UI_LobbyPopup>();
    }
}
