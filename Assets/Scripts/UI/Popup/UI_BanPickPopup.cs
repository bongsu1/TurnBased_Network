using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    private const string pick = "pick";
    private const string count = "count";
    private const string pickUp = "pickUp";
    // 선픽은 항상 플레이어 1
    private bool first = false;

    DatabaseReference pickRef = null;

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

        Get<Button>((int)Buttons.SelectButton).gameObject.BindEvent(OnClickSelectButton);
        ActiveSelectButton(false);

        #region 전투방 만들기
        string roomKey = Manager.Game.RoomInfo.key;
        string p1 = Manager.Game.RoomInfo.uids[0];
        string p2 = Manager.Game.RoomInfo.uids[1];

        string myUID = Manager.Data.Auth.CurrentUser.UserId;
        first = myUID == p1;

        pickRef = Manager.Data.DB.GetReference($"Battles/{roomKey}");
        // 선픽
        if (first)
        {
            string json = JsonUtility.ToJson(new PickInfo());
            pickRef.SetRawJsonValueAsync(json);
        }
        pickRef.ValueChanged += OnSelectPick;
        #endregion

        Get<TMP_Text>((int)Texts.MyNickNameText).text = Manager.Game.MyInfo.nickName;
        Get<TMP_Text>((int)Texts.MyRankScoreText).text = $"{Manager.Game.MyInfo.rankPoint} 점";
        Manager.Data.DB.GetReference(UserInfo.Root).Child(first ? p2 : p1)
            .GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                string json = task.Result.GetRawJsonValue();
                UserInfo otherInfo = JsonUtility.FromJson<UserInfo>(json);
                if (otherInfo == null)
                    return;

                Get<TMP_Text>((int)Texts.OtherNickNameText).text = otherInfo.nickName;
                Get<TMP_Text>((int)Texts.OtherRankScoreText).text = $"{otherInfo.rankPoint} 점";
            }
        });

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

        var send = new Dictionary<string, object> { { pick, currentPickID }, { pickUp, false } };
        pickRef.UpdateChildrenAsync(send);
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

        // 내가 선택중일 때
        if (firstTurns[turnCount] != first)
            return;

        // 선택
        Image myPickedImage = Get<Image>((int)Images.My1stPickedImage + myPickCount);
        TMP_Text myPickedname = Get<TMP_Text>((int)Texts.My1stPickedText + myPickCount);
        ++myPickCount;

        myPickedImage.gameObject.SetActive(true);
        myPickedname.gameObject.SetActive(true);
        myPickedImage.sprite = Manager.Game.CharacterDictionary[currentPickID].frontImage;
        myPickedname.text = Manager.Game.CharacterDictionary[currentPickID].charName;

        pickItems[currentPickID].GetComponent<Button>().interactable = false;
        pickItems[currentPickID].gameObject.EventActive(false);

        picked.Add(currentPickID);
        if (first)
        {
            firstPickList.Add(currentPickID);
        }
        else
        {
            secondPickList.Add(currentPickID);
        }

        var send = new Dictionary<string, object> { { count, turnCount }, { pickUp, true } };
        pickRef.UpdateChildrenAsync(send);

        ++turnCount;
        currentPickID = -1;

        if (turnCount == 10) // 모두 선택했으면
        {
            pickRef.ValueChanged -= OnSelectPick;
            ActiveSelectButton(false);
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

    private void OnClickBanButton(int selectIdx)
    {
        // 테스트
        Get<Button>((int)Buttons.Other1stPickedButton + selectIdx).GetComponent<Image>().color = Color.red;

        ActiveBanButton(false);

        // 상대팀을 밴하는 것이기 때문에 반대로 넣어줌
        string player = first ? "second" : "first";
        var send = new Dictionary<string, object> { { player, selectIdx } };
        pickRef.ValueChanged += OnSelectBan;
        pickRef.UpdateChildrenAsync(send);
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
        // 테스트 아직 스프라이트가 없는 캐릭터들이 있음
        if (Manager.Game.CharacterDictionary[id].frontImage != null)
            selectedImage.sprite = Manager.Game.CharacterDictionary[id].frontImage;
        selectedName.text = Manager.Game.CharacterDictionary[id].charName;
    }

    private void OnSelectPick(object obj, ValueChangedEventArgs args)
    {
        if (turnCount == 10)
            return;

        string json = args.Snapshot.GetRawJsonValue();
        var pickInfo = JsonUtility.FromJson<PickInfo>(json);

        // 현재 자신의 턴 일때만 선택버튼 활성화
        ActiveSelectButton(firstTurns[turnCount] == first);

        // 상대 턴일 때만 들어옴
        if (firstTurns[pickInfo.count] == first)
            return;

        ShowPickItem(pickInfo.pick);

        // 상대가 선택을 확정한 경우
        if (pickInfo.pickUp)
        {
            Image otherPickedImage = Get<Image>((int)Images.Other1stPickedImage + otherPickCount);
            TMP_Text otherPickedname = Get<TMP_Text>((int)Texts.Other1stPickedText + otherPickCount);
            ++otherPickCount;

            otherPickedImage.gameObject.SetActive(true);
            otherPickedname.gameObject.SetActive(true);
            otherPickedImage.sprite = Manager.Game.CharacterDictionary[pickInfo.pick].frontImage;
            otherPickedname.text = Manager.Game.CharacterDictionary[pickInfo.pick].charName;

            pickItems[pickInfo.pick].GetComponent<Button>().interactable = false;
            pickItems[pickInfo.pick].gameObject.EventActive(false);

            picked.Add(pickInfo.pick);

            // 자신이 첫번째 플레이어일때
            if (first)
            {
                secondPickList.Add(pickInfo.pick);
            }
            // 두번째 플레이어일때
            else
            {
                firstPickList.Add(pickInfo.pick);
            }

            ++turnCount;
            var send = new Dictionary<string, object> { { count, turnCount }, { pickUp, false } };
            pickRef.UpdateChildrenAsync(send);

            // 모두 완료된 상황
            if (turnCount == 10)
            {
                pickRef.ValueChanged -= OnSelectPick;
                ActiveSelectButton(false);
                ActiveBanButton(true);
            }
        }
    }

    private void OnSelectBan(object obj, ValueChangedEventArgs args)
    {
        string json = args.Snapshot.GetRawJsonValue();
        BanInfo banInfo = JsonUtility.FromJson<BanInfo>(json);

        if (banInfo.first == -1 || banInfo.second == -1)
            return;

        firstPickList.RemoveAt(banInfo.first);
        secondPickList.RemoveAt(banInfo.second);

        int ban = first ? banInfo.first : banInfo.second;

        // 테스트
        Get<Image>((int)Images.My1stPicked + ban).color = Color.red;

        pickRef.ValueChanged -= OnSelectBan;

        Manager.Game.SetPickList(firstPickList, secondPickList);

        StartCoroutine(ToBattleRoutine());
        // 지연 추가
    }

    private IEnumerator ToBattleRoutine()
    {
        yield return new WaitForSeconds(2f);
        Manager.UI.ClosePopupUI(this);
        Manager.UI.ShowPopupUI<UI_BattlePopup>();
    }
}

[Serializable]
public struct PickInfo
{
    public int pick;
    public int count;
    public bool pickUp;
}

[Serializable]
public class BanInfo
{
    public int first = -1;
    public int second = -1;
}