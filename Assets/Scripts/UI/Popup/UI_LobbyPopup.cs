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
        GameObject rbDisplay = Get<GameObject>((int)GameObjects.RankBattleDisplay);
        GameObject mbDisplay = Get<GameObject>((int)GameObjects.MockBattleDisplay);
        GameObject blDisplay = Get<GameObject>((int)GameObjects.BattleLogDisplay);
        GameObject rkDisplay = Get<GameObject>((int)GameObjects.RankingDisplay);

        Button rbButton = Get<Button>((int)Buttons.RankBattleButton);
        Button mbButton = Get<Button>((int)Buttons.MockBattleButton);
        Button blButton = Get<Button>((int)Buttons.BattleLogButton);
        Button rkButton = Get<Button>((int)Buttons.RankingButton);

        switch (objects)
        {
            case GameObjects.RankBattleDisplay:
                rbDisplay.SetActive(true);
                mbDisplay.SetActive(false);
                blDisplay.SetActive(false);
                rkDisplay.SetActive(false);

                rbButton.interactable = false;
                mbButton.interactable = true;
                blButton.interactable = true;
                rkButton.interactable = true;

                rbButton.gameObject.EventActive(false);
                mbButton.gameObject.EventActive(true);
                blButton.gameObject.EventActive(true);
                rkButton.gameObject.EventActive(true);
                break;

            case GameObjects.MockBattleDisplay:
                rbDisplay.SetActive(false);
                mbDisplay.SetActive(true);
                blDisplay.SetActive(false);
                rkDisplay.SetActive(false);

                rbButton.interactable = true;
                mbButton.interactable = false;
                blButton.interactable = true;
                rkButton.interactable = true;

                rbButton.gameObject.EventActive(true);
                mbButton.gameObject.EventActive(false);
                blButton.gameObject.EventActive(true);
                rkButton.gameObject.EventActive(true);
                break;

            case GameObjects.BattleLogDisplay:
                rbDisplay.SetActive(false);
                mbDisplay.SetActive(false);
                blDisplay.SetActive(true);
                rkDisplay.SetActive(false);

                rbButton.interactable = true;
                mbButton.interactable = true;
                blButton.interactable = false;
                rkButton.interactable = true;

                rbButton.gameObject.EventActive(true);
                mbButton.gameObject.EventActive(true);
                blButton.gameObject.EventActive(false);
                rkButton.gameObject.EventActive(true);
                break;

            case GameObjects.RankingDisplay:
                rbDisplay.SetActive(false);
                mbDisplay.SetActive(false);
                blDisplay.SetActive(false);
                rkDisplay.SetActive(true);

                rbButton.interactable = true;
                mbButton.interactable = true;
                blButton.interactable = true;
                rkButton.interactable = false;

                rbButton.gameObject.EventActive(true);
                mbButton.gameObject.EventActive(true);
                blButton.gameObject.EventActive(true);
                rkButton.gameObject.EventActive(false);
                break;

            default:
                // nothing..
                break;
        }
    }
}
