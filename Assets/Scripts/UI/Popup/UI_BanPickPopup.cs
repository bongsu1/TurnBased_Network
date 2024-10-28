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

    private Dictionary<int, UI_PickItem> pickDic = new Dictionary<int, UI_PickItem>();

    private int currentPickID = -1; // 데이터로 교체 고려
    private int pickCount; // 지금 몇번째 선택중인지
    private int myBan = 0; // 내 몇번째 캐릭터가 밴 될지
    private int otherBan = 0; // 상대 몇번째 캐릭터가 밴 될지

    // 테스트 데이터
    string[] names = { "이상해씨", "파이리", "꼬부기", "피카츄", "이브이", "치코리타", "브케인", "리아코", "나무지기", "아차모", "물짱이" };

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

        // 테스트
        for (int i = 0; i < names.Length; i++)
        {
            UI_PickItem pickItem = Manager.UI.MakeSubItem<UI_PickItem>(pickContent);
            pickDic.Add(i, pickItem);
            pickItem.SetInfo(names[i]);

            int id = i;
            pickItem.gameObject.BindEvent(() => OnClickPickItem(id));
        }

        Get<Button>((int)Buttons.SelectButton).gameObject.BindEvent(OnClickSelectButton);

        return true;
    }

    private void OnClickPickItem(int selectID)
    {
        currentPickID = selectID; // 데이터 에서 불러와서 사용하도록 수정

        TMP_Text selectedName = Get<TMP_Text>((int)Texts.SelectedNameText);
        Image selectedImage = Get<Image>((int)Images.SelectedImage);
        if (selectedImage.gameObject.activeSelf == false)
        {
            selectedImage.gameObject.SetActive(true);
            selectedName.gameObject.SetActive(true);
        }

        selectedName.text = names[selectID];
    }

    private void OnClickSelectButton()
    {
        if (currentPickID == -1) 
            return;
        // 이미 픽된것이면 리턴되도록 추가

        // 선택
        Image myPickedImage = Get<Image>((int)Images.My1stPickedImage + pickCount);
        TMP_Text myPickedname = Get<TMP_Text>((int)Texts.My1stPickedText + pickCount);
        ++pickCount;

        myPickedImage.gameObject.SetActive(true);
        myPickedname.gameObject.SetActive(true);
        myPickedname.text = names[currentPickID];

        pickDic[currentPickID].GetComponent<Button>().interactable = false;
        pickDic[currentPickID].gameObject.EventActive(false);

        currentPickID = -1;

        if (pickCount == 5) // 모두 선택했으면
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
    }

    private void OnClickBanButton(int select)
    {
        otherBan = select;

        // 테스트
        Get<Button>((int)Buttons.Other1stPickedButton + select).GetComponent<Image>().color = Color.red;

        ActiveBanButton(false);
    }
}
