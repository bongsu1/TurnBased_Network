using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        MatchQuitButton,
    }

    private enum Images
    {
        My1stPicked,
        My2ndPicked,
        My3rdPicked,
        My4thPicked,
        My5thPicked,
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

    private bool first = false;
    private int[] banInfo = { -1, -1 };

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
        for (int i = 0; i < 11; i++)
            Get<Image>((int)Images.My1stPickedImage + i).gameObject.SetActive(false);
        for (int i = 0; i < 11; i++)
            Get<TMP_Text>((int)Texts.My1stPickedText + i).gameObject.SetActive(false);

        int[] charIDs = Manager.Game.CharacterDictionary.GetIDs();
        for (int i = 0; i < charIDs.Length; i++)
        {
            UI_PickItem pickItem = Manager.UI.MakeSubItem<UI_PickItem>(pickContent);
            pickItems.Add(charIDs[i], pickItem);
            pickItem.SetInfo(Manager.Game.CharacterDictionary[charIDs[i]]);

            int id = charIDs[i];
            pickItem.gameObject.BindEvent(() => OnClickPickItem(id));
        }

        Get<TMP_Text>((int)Texts.MyNickNameText).text = Manager.Game.MyInfo.nickName;
        Get<TMP_Text>((int)Texts.MyRankScoreText).text = $"{Manager.Game.MyInfo.rankPoint} 점";

        // 방에 먼저 들어온사람이 선픽
        first = Manager.Game.IsFirst;

        PlayerManager.Instance.OnSelectPickUp -= OnSelectPick;
        PlayerManager.Instance.OnSelectPickUp += OnSelectPick;
        PlayerManager.Instance.OnSelectItem -= ShowPickItem;
        PlayerManager.Instance.OnSelectItem += ShowPickItem;

        Get<Button>((int)Buttons.SelectButton).gameObject.BindEvent(OnClickSelectButton);
        ActiveSelectButton(first);

        Get<Button>((int)Buttons.MatchQuitButton).gameObject.BindEvent(OnClickQuitMatchButton);

        return true;
    }

    // 구현부

    // 교차선택
    private readonly bool[] firstTurns = { true, false, false, true, true, false, false, true, true, false };
    private Dictionary<int, UI_PickItem> pickItems = new Dictionary<int, UI_PickItem>();
    private List<int> picked = new List<int>();
    private List<int> firstPickList = new List<int>();
    private List<int> secondPickList = new List<int>();

    private int currentPickID = -1;
    private int turnCount = 0; // 지금 몇번째 선택중인지
    private int myPickCount = 0;
    private int otherPickCount = 0;

    private void OnClickPickItem(int selectID)
    {
        // 밴픽이 종료되면 클릭이 안되도록
        if (turnCount > 9)
            return;

        // 자신의 턴에만 캐릭터를 선택할 수 있도록
        if (firstTurns[turnCount] != first)
            return;

        // 이미 선택되어 있으면 쓸데없는 연산을 피하기 위해 리턴
        if (currentPickID == selectID)
            return;

        currentPickID = selectID;
        ShowPickItem(selectID);

        C_BanPick pickItem = new C_BanPick { banId = (short)selectID };
        Manager.Network.Send(pickItem.Write());
    }

    private void OnClickSelectButton()
    {
        if (currentPickID == -1)
            return;

        // 이미 픽 되어있다면
        if (picked.Contains(currentPickID))
        {
            pickItems[currentPickID].GetComponent<Button>().interactable = false;
            pickItems[currentPickID].gameObject.EventActive(false);
            return;
        }

        // 내 턴이 아닐 때
        if (firstTurns[turnCount] != first)
            return;

        C_PickUp pickUp = new C_PickUp { pickIdx = (short)currentPickID };
        Manager.Network.Send(pickUp.Write());

        // 선택
        Image myPickedImage = Get<Image>((int)Images.My1stPickedImage + myPickCount);
        TMP_Text myPickedname = Get<TMP_Text>((int)Texts.My1stPickedText + myPickCount);
        ++myPickCount;

        PickUp(myPickedImage, myPickedname, currentPickID, first);
        currentPickID = -1;
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

    private void OnClickBanButton(int selectIdx)
    {
        // 테스트
        Get<Button>((int)Buttons.Other1stPickedButton + selectIdx).GetComponent<Image>().color = Color.red;

        ActiveBanButton(false);

        int team = first ? 1 : 0;
        banInfo[team] = selectIdx;

        C_BanPick banPick = new C_BanPick { banId = (short)selectIdx };
        Manager.Network.Send(banPick.Write());

        EndBan();
    }

    private void OnClickQuitMatchButton()
    {
        Manager.UI.ClosePopupUI(this);
        Manager.UI.ShowPopupUI<UI_LobbyPopup>();

        Manager.Network.Disconnect();
    }

    private void ActiveSelectButton(bool active)
    {
        Get<Button>((int)Buttons.SelectButton).interactable = active;
        Get<Button>((int)Buttons.SelectButton).gameObject.EventActive(active);
    }

    private void ShowPickItem(int id)
    {
        if (Manager.Game.CharacterDictionary[id] == null)
            return;

        TMP_Text selectedName = Get<TMP_Text>((int)Texts.SelectedNameText);
        Image selectedImage = Get<Image>((int)Images.SelectedImage);

        if (selectedImage.gameObject.activeSelf == false)
        {
            selectedImage.gameObject.SetActive(true);
            selectedName.gameObject.SetActive(true);
        }

        selectedImage.sprite = Manager.Game.CharacterDictionary[id].frontImage;
        selectedName.text = Manager.Game.CharacterDictionary[id].charName;
    }

    private void OnSelectPick(int pickID)
    {
        if (turnCount == 10)
            return;

        ShowPickItem(pickID);

        Image otherPickedImage = Get<Image>((int)Images.Other1stPickedImage + otherPickCount);
        TMP_Text otherPickedname = Get<TMP_Text>((int)Texts.Other1stPickedText + otherPickCount);
        ++otherPickCount;

        PickUp(otherPickedImage, otherPickedname, pickID, first == false);
    }

    private void OnSelectBan(int banIdx)
    {
        int team = first ? 0 : 1;
        banInfo[team] = banIdx;

        EndBan();
    }

    private void EndBan()
    {
        // 양쪽이 모두 끝나야 진행
        if (banInfo[0] == -1 || banInfo[1] == -1)
            return;

        firstPickList.RemoveAt(banInfo[0]);
        secondPickList.RemoveAt(banInfo[1]);

        int ban = first ? banInfo[0] : banInfo[1];

        // 테스트
        Get<Image>((int)Images.My1stPicked + ban).color = Color.red;

        PlayerManager.Instance.OnSelectBan -= OnSelectBan;
        Manager.Game.SetPickList(firstPickList, secondPickList);

        StartCoroutine(ToBattleRoutine());
    }

    private IEnumerator ToBattleRoutine()
    {
        yield return new WaitForSeconds(2f);
        Manager.UI.ClosePopupUI(this);
        Manager.UI.ShowPopupUI<UI_BattlePopup>();
    }

    private void PickUp(Image characterImage, TMP_Text characterNameText, int characterID, bool first)
    {
        characterImage.gameObject.SetActive(true);
        characterNameText.gameObject.SetActive(true);
        characterImage.sprite = Manager.Game.CharacterDictionary[characterID].frontImage;
        characterNameText.text = Manager.Game.CharacterDictionary[characterID].charName;

        pickItems[characterID].GetComponent<Button>().interactable = false;
        pickItems[characterID].gameObject.EventActive(false);

        picked.Add(characterID);

        // 자신이 첫 번째 플레이어라면 자신이 골랐을때는 첫 팀리스트에, 상대가 고른 경우에는 두 번째 팀 리스트에 추가
        // 두 번째 플레이어라면 반대로
        if (first)
        {
            firstPickList.Add(characterID);
        }
        else
        {
            secondPickList.Add(characterID);
        }

        ++turnCount;
        if (turnCount < 10)
        {
            ActiveSelectButton(firstTurns[turnCount] == this.first);
        }
        else
        {
            // 이벤트 제거
            PlayerManager.Instance.OnSelectPickUp -= OnSelectPick;
            PlayerManager.Instance.OnSelectItem -= ShowPickItem;

            PlayerManager.Instance.OnSelectBan -= OnSelectBan;
            PlayerManager.Instance.OnSelectBan += OnSelectBan;
            ActiveSelectButton(false);
            ActiveBanButton(true);
        }
    }

    private void OnDestroy()
    {
        PlayerManager.Instance.OnSelectPickUp -= OnSelectPick;
        PlayerManager.Instance.OnSelectItem -= ShowPickItem;
        PlayerManager.Instance.OnSelectBan -= OnSelectBan;
    }
}