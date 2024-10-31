using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_BanPickPopup : UI_Popup
{
    private enum GameObjects
    {
        PickContent,
    }

    private enum Buttons
    {
        SelectButton,
        Other1stPickedButton, // 밴 버튼
        Other2ndPickedButton,
        Other3rdPickedButton,
        Other4thPickedButton,
        Other5thPickedButton,
    }

    private enum Images
    {
        My1stPickedImage,
        My2ndPickedImage,
        My3rdPickedImage,
        My4thPickedImage,
        My5thPickedImage,
        Other1stPickedImage,
        Other2ndPickedImage,
        Other3rdPickedImage,
        Other4thPickedImage,
        Other5thPickedImage,
        SelectedImage,
        MyProfileImage,
        OtherProfileImage,
    }

    private enum Texts
    {
        My1stPickedText,
        My2ndPickedText,
        My3rdPickedText,
        My4thPickedText,
        My5thPickedText,
        Other1stPickedText,
        Other2ndPickedText,
        Other3rdPickedText,
        Other4thPickedText,
        Other5thPickedText,
        SelectedNameText,
        MyNickNameText,
        MyRankScoreText,
        OtherNickNameText,
        OtherRankScoreText,
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));
        Bind<TMP_Text>(typeof(Texts));

        Transform pickContent = Get<GameObject>((int)GameObjects.PickContent).transform;

        Get<Button>((int)Buttons.Other1stPickedButton).gameObject.BindEvent(() => OnClickBanButton(0));
        Get<Button>((int)Buttons.Other2ndPickedButton).gameObject.BindEvent(() => OnClickBanButton(1));
        Get<Button>((int)Buttons.Other3rdPickedButton).gameObject.BindEvent(() => OnClickBanButton(2));
        Get<Button>((int)Buttons.Other4thPickedButton).gameObject.BindEvent(() => OnClickBanButton(3));
        Get<Button>((int)Buttons.Other5thPickedButton).gameObject.BindEvent(() => OnClickBanButton(4));
        ActiveBanButton(false);

        // 선택이 되기 전까지 비활성화
        for (int i = 0; i < Enum.GetNames(typeof(Images)).Length; i++)
            Get<Image>(i).gameObject.SetActive(false);
        for (int i = 0; i < Enum.GetNames(typeof(Texts)).Length; i++)
            Get<TMP_Text>(i).gameObject.SetActive(false);

        int[] charIDs = Manager.Game.CharacterDictionary.GetIDs();
        for (int i = 0; i < charIDs.Length; i++)
        {
            UI_PickItem pickItem = Manager.UI.MakeSubItem<UI_PickItem>(pickContent);
            pickItemDic.Add(charIDs[i], pickItem);
            pickItem.SetInfo(Manager.Game.CharacterDictionary[charIDs[i]]);

            int id = charIDs[i];
            pickItem.gameObject.BindEvent(() => OnClickPickItem(id));
        }

        Get<Button>((int)Buttons.SelectButton).gameObject.BindEvent(OnClickSelectButton);

        return true;
    }

    // 구현부

    private bool[] myTurns = { true, false, false, true, true, false, false, true, true, false }; // 교차선택
    private Dictionary<int, UI_PickItem> pickItemDic = new Dictionary<int, UI_PickItem>();
    private List<int> picked = new List<int>();
    private List<int> myPickList = new List<int>();
    private List<int> otherPickList = new List<int>();

    private int currentPickID = -1; // 데이터로 교체 고려
    private int turnCount; // 지금 몇번째 선택중인지
    private int myPickCount = 0;
    private int otherPickCount = 0;

    private void OnClickPickItem(int selectID)
    {
        currentPickID = selectID;

        TMP_Text selectedName = Get<TMP_Text>((int)Texts.SelectedNameText);
        Image selectedImage = Get<Image>((int)Images.SelectedImage);
        if (selectedImage.gameObject.activeSelf == false)
        {
            selectedImage.gameObject.SetActive(true);
            selectedName.gameObject.SetActive(true);
        }

        selectedName.text = Manager.Game.CharacterDictionary[selectID].charName;
    }

    private void OnClickSelectButton()
    {
        if (currentPickID == -1)
            return;

        // 이미 픽 되어있다면
        if (picked.Contains(currentPickID))
        {
            pickItemDic[currentPickID].GetComponent<Button>().interactable = false;
            pickItemDic[currentPickID].gameObject.EventActive(false);
            return;
        }

        // 내가 선택중일 때
        if (myTurns[turnCount])
        {
            // 선택
            Image myPickedImage = Get<Image>((int)Images.My1stPickedImage + myPickCount);
            TMP_Text myPickedname = Get<TMP_Text>((int)Texts.My1stPickedText + myPickCount);
            ++turnCount;
            ++myPickCount;

            myPickedImage.gameObject.SetActive(true);
            myPickedname.gameObject.SetActive(true);
            myPickedname.text = Manager.Game.CharacterDictionary[currentPickID].charName;

            pickItemDic[currentPickID].GetComponent<Button>().interactable = false;
            pickItemDic[currentPickID].gameObject.EventActive(false);

            myPickList.Add(currentPickID);
            picked.Add(currentPickID);
            currentPickID = -1;
        }
        else
        {
            Image otherPickedImage = Get<Image>((int)Images.Other1stPickedImage + otherPickCount);
            TMP_Text otherPickedname = Get<TMP_Text>((int)Texts.Other1stPickedText + otherPickCount);
            ++turnCount;
            ++otherPickCount;

            otherPickedImage.gameObject.SetActive(true);
            otherPickedname.gameObject.SetActive(true);
            otherPickedname.text = Manager.Game.CharacterDictionary[currentPickID].charName;

            pickItemDic[currentPickID].GetComponent<Button>().interactable = false;
            pickItemDic[currentPickID].gameObject.EventActive(false);

            otherPickList.Add(currentPickID);
            picked.Add(currentPickID);
            currentPickID = -1;
        }

        if (turnCount == 10) // 모두 선택했으면
        {
            Button selectButton = Get<Button>((int)Buttons.SelectButton);
            selectButton.interactable = false;
            selectButton.gameObject.EventActive(false);

            ActiveBanButton(true);
        }
    }

    private void ActiveBanButton(bool active)
    {
        Get<Button>((int)Buttons.Other1stPickedButton).interactable = active;
        Get<Button>((int)Buttons.Other2ndPickedButton).interactable = active;
        Get<Button>((int)Buttons.Other3rdPickedButton).interactable = active;
        Get<Button>((int)Buttons.Other4thPickedButton).interactable = active;
        Get<Button>((int)Buttons.Other5thPickedButton).interactable = active;

        Get<Button>((int)Buttons.Other1stPickedButton).gameObject.EventActive(active);
        Get<Button>((int)Buttons.Other2ndPickedButton).gameObject.EventActive(active);
        Get<Button>((int)Buttons.Other3rdPickedButton).gameObject.EventActive(active);
        Get<Button>((int)Buttons.Other4thPickedButton).gameObject.EventActive(active);
        Get<Button>((int)Buttons.Other5thPickedButton).gameObject.EventActive(active);

        // 테스트
        if (active)
        {
            for (int i = 0; i < 5; i++)
            {
                Image pickImage = Get<Image>((int)Images.My1stPickedImage + i);
                int idx = i;
                pickImage.gameObject.BindEvent(() =>
                {
                    pickImage.color = Color.red;
                    myPickList.RemoveAt(idx);
                    for (int i = 0; i < 5; i++)
                    {
                        Image pickImage = Get<Image>((int)Images.My1stPickedImage + i);
                        pickImage.gameObject.EventActive(false);
                    }
                    otherBan = true;
                    EndBanPick();
                });
            }
        }
    }

    private void OnClickBanButton(int selectIdx)
    {
        // 테스트
        Get<Button>((int)Buttons.Other1stPickedButton + selectIdx).GetComponent<Image>().color = Color.red;
        otherPickList.RemoveAt(selectIdx);

        ActiveBanButton(false);
        myBan = true;
        EndBanPick();
    }

    bool myBan = false;
    bool otherBan = false;
    private void EndBanPick()
    {
        if (myBan && otherBan)
        {
            Manager.Game.SetPickList(myPickList, true);
            Manager.Game.SetPickList(otherPickList, false);

            Manager.UI.ClosePopupUI(this);
            Manager.UI.ShowPopupUI<UI_BattlePopup>();
        }
    }
}
