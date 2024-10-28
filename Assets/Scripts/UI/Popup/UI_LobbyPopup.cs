using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_LobbyPopup : UI_Popup
{
    [SerializeField] GameObject[] displays;

    private enum Buttons
    {
        MenuButton,
        RankBattleButton,
        MockBattleButton,
        BattleLogButton,
        RankingButton,
        RankMatchButton,
        CreateRoomButton,
    }

    private enum GameObjects
    {
        RankBattleDisplay,
        MockBattleDisplay,
        BattleLogDisplay,
        RankingDisplay,
    }

    private enum Images
    {
        ProfileImage,
        RankPointImage,
    }

    private enum Texts
    {
        RankPointText,
        LogInfoText, // 전적 텍스트
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));
        Bind<TMP_Text>(typeof(Texts));

        ActiveDisplay(GameObjects.RankBattleDisplay);

        Get<Button>((int)Buttons.MenuButton).gameObject.BindEvent(OnClickMenuButton);
        Get<Button>((int)Buttons.RankBattleButton).gameObject.BindEvent(OnClickRankBattleButton);
        Get<Button>((int)Buttons.MockBattleButton).gameObject.BindEvent(OnClickMockBattleButton);
        Get<Button>((int)Buttons.BattleLogButton).gameObject.BindEvent(OnClickBattleLogButton);
        Get<Button>((int)Buttons.RankingButton).gameObject.BindEvent(OnClickRankingButton);
        Get<Button>((int)Buttons.RankMatchButton).gameObject.BindEvent(OnClickRankMatchButton);
        Get<Button>((int)Buttons.CreateRoomButton).gameObject.BindEvent(OnClickCreateRoomButton);

        return true;
    }

    private void OnClickMenuButton() { Debug.Log("메뉴버튼"); }

    private void OnClickRankBattleButton() { ActiveDisplay(GameObjects.RankBattleDisplay); }

    private void OnClickMockBattleButton() { ActiveDisplay(GameObjects.MockBattleDisplay); }

    private void OnClickBattleLogButton() { ActiveDisplay(GameObjects.BattleLogDisplay); }

    private void OnClickRankingButton() { ActiveDisplay(GameObjects.RankingDisplay); }

    private void OnClickRankMatchButton()
    { 
        // 매칭 로직
        // 나머지 버튼들 비활성화
        // 매칭중일 때 매칭 버튼을 다시 클릭하면 매칭 취소
        Debug.Log("전투신청버튼");
    }

    private void OnClickCreateRoomButton() { Debug.Log("방만들기버튼"); }

    private void ActiveDisplay(GameObjects objects)
    {
        switch (objects)
        {
            case GameObjects.RankBattleDisplay:
                Get<GameObject>((int)GameObjects.RankBattleDisplay).SetActive(true);
                Get<GameObject>((int)GameObjects.MockBattleDisplay).SetActive(false);
                Get<GameObject>((int)GameObjects.BattleLogDisplay).SetActive(false);
                Get<GameObject>((int)GameObjects.RankingDisplay).SetActive(false);

                Get<Button>((int)Buttons.RankBattleButton).interactable = false;
                Get<Button>((int)Buttons.MockBattleButton).interactable = true;
                Get<Button>((int)Buttons.BattleLogButton).interactable = true;
                Get<Button>((int)Buttons.RankingButton).interactable = true;
                break;

            case GameObjects.MockBattleDisplay:
                Get<GameObject>((int)GameObjects.RankBattleDisplay).SetActive(false);
                Get<GameObject>((int)GameObjects.MockBattleDisplay).SetActive(true);
                Get<GameObject>((int)GameObjects.BattleLogDisplay).SetActive(false);
                Get<GameObject>((int)GameObjects.RankingDisplay).SetActive(false);

                Get<Button>((int)Buttons.RankBattleButton).interactable = true;
                Get<Button>((int)Buttons.MockBattleButton).interactable = false;
                Get<Button>((int)Buttons.BattleLogButton).interactable = true;
                Get<Button>((int)Buttons.RankingButton).interactable = true;
                break;

            case GameObjects.BattleLogDisplay:
                Get<GameObject>((int)GameObjects.RankBattleDisplay).SetActive(false);
                Get<GameObject>((int)GameObjects.MockBattleDisplay).SetActive(false);
                Get<GameObject>((int)GameObjects.BattleLogDisplay).SetActive(true);
                Get<GameObject>((int)GameObjects.RankingDisplay).SetActive(false);

                Get<Button>((int)Buttons.RankBattleButton).interactable = true;
                Get<Button>((int)Buttons.MockBattleButton).interactable = true;
                Get<Button>((int)Buttons.BattleLogButton).interactable = false;
                Get<Button>((int)Buttons.RankingButton).interactable = true;
                break;

            case GameObjects.RankingDisplay:
                Get<GameObject>((int)GameObjects.RankBattleDisplay).SetActive(false);
                Get<GameObject>((int)GameObjects.MockBattleDisplay).SetActive(false);
                Get<GameObject>((int)GameObjects.BattleLogDisplay).SetActive(false);
                Get<GameObject>((int)GameObjects.RankingDisplay).SetActive(true);

                Get<Button>((int)Buttons.RankBattleButton).interactable = true;
                Get<Button>((int)Buttons.MockBattleButton).interactable = true;
                Get<Button>((int)Buttons.BattleLogButton).interactable = true;
                Get<Button>((int)Buttons.RankingButton).interactable = false;
                break;

            default:
                // nothing..
                break;
        }
    }
}
