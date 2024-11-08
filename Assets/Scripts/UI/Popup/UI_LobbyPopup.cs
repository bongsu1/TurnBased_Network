using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_LobbyPopup : UI_Popup
{
    private enum Buttons
    {
        MenuButton,
        RankBattleButton,
        MockBattleButton,
        BattleLogButton,
        RankingButton,
        RankMatchButton,
        CreateRoomButton,
        LogoutButton,
        QuitButton,
    }

    private enum GameObjects
    {
        RankBattleDisplay,
        MockBattleDisplay,
        BattleLogDisplay,
        RankingDisplay,
        Menu,
        Matching,
    }

    private enum Images
    {
        ProfileImage,
        RankPointImage,
    }

    private enum Texts
    {
        ProfileImageText,
        RankPointText,
        LogInfoText, // 전적 텍스트
    }

    private string myRoomKey = null;
    private Room myRoom = null;

    private DatabaseReference matchingRef = null;
    private DatabaseReference userInfoRef = null;

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
        Get<Button>((int)Buttons.LogoutButton).gameObject.BindEvent(SignOut);
        Get<Button>((int)Buttons.QuitButton).gameObject.BindEvent(OnClickQuitButton);

        Get<GameObject>((int)GameObjects.Menu).SetActive(false);
        Get<GameObject>((int)GameObjects.Matching).SetActive(false);

        matchingRef = Manager.Data.DB.GetReference(Matching.Root);
        userInfoRef = Manager.Data.DB.GetReference(UserInfo.Root);

        InformationInit();

        return true;
    }

    private void OnClickMenuButton()
    {
        GameObject menu = Get<GameObject>((int)GameObjects.Menu);
        menu.SetActive(menu.activeSelf == false);
    }

    private void OnClickQuitButton() { Application.Quit(); }

    private void OnClickRankBattleButton() { ActiveDisplay(GameObjects.RankBattleDisplay); }

    private void OnClickMockBattleButton() { ActiveDisplay(GameObjects.MockBattleDisplay); }

    private void OnClickBattleLogButton() { ActiveDisplay(GameObjects.BattleLogDisplay); }

    private void OnClickRankingButton() { ActiveDisplay(GameObjects.RankingDisplay); }

    private void OnClickRankMatchButton()
    {
        // 테스트 일단 취소는 안됨
        Get<GameObject>((int)GameObjects.Matching).SetActive(true);
        Get<Button>((int)Buttons.RankMatchButton).gameObject.EventActive(false);

        Manager.Data.DB.GetReference(Matching.Root).GetValueAsync().ContinueWithOnMainThread(FindMatch);

        // 매칭중일 때 매칭 버튼을 다시 클릭하면 매칭 취소하는 기능 추가
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

    private void FindMatch(Task<DataSnapshot> task)
    {
        string uid = Manager.Data.Auth.CurrentUser.UserId;

        if (task.IsCanceled)
        {
            // 취소
            Debug.Log("불러오기 취소됨");
            Get<GameObject>((int)GameObjects.Matching).SetActive(false);
            Get<Button>((int)Buttons.RankMatchButton).gameObject.EventActive(true);
            return;
        }
        else if (task.IsFaulted)
        {
            // 실패
            Debug.Log("불러오기 실패함");
            Get<GameObject>((int)GameObjects.Matching).SetActive(false);
            Get<Button>((int)Buttons.RankMatchButton).gameObject.EventActive(true);
            return;
        }

        string json = task.Result.GetRawJsonValue();
        Matching matching = JsonUtility.FromJson<Matching>(json);

        if (matching == null)
        {
            matching = new Matching();
        }

        matching.ChangeStructure(true);

        for (int i = 0; i < matching.rooms.Count; i++)
        {
            if (matching.rooms[i].isFull == false)
            {
                myRoomKey = matching.rooms[i].key;
                break;
            }
        }

        // 남아 있는 방이 없다
        if (string.IsNullOrEmpty(myRoomKey))
        {
            myRoomKey = matchingRef.Push().Key;

            Room newRoom = new Room(myRoomKey, uid);
            matching.rooms.Add(newRoom);

            myRoom = newRoom;
        }
        // 방이 있다
        else
        {
            matching.krp[myRoomKey].uids[1] = uid;
            matching.krp[myRoomKey].isFull = true;

            myRoom = matching.krp[myRoomKey];
            // 매칭완료
        }

        string newjson = JsonUtility.ToJson(matching);
        matchingRef.SetRawJsonValueAsync(newjson).ContinueWithOnMainThread(SubscribeValueChange);
    }

    private void SubscribeValueChange(Task task)
    {
        if (task.IsCanceled)
        {
            // 취소
            Debug.Log("참가 취소");
            myRoomKey = null;
            myRoom = null;

            Get<Button>((int)Buttons.RankMatchButton).gameObject.EventActive(true);
            return;
        }
        else if (task.IsFaulted)
        {
            // 실패
            Debug.Log("참가 실패");
            myRoomKey = null;
            myRoom = null;

            Get<Button>((int)Buttons.RankMatchButton).gameObject.EventActive(true);
            return;
        }

        Debug.Log("참가 완료");
        matchingRef.ValueChanged -= OnMatch;
        matchingRef.ValueChanged += OnMatch;
    }

    private void OnMatch(object obj, ValueChangedEventArgs args)
    {
        string json = args.Snapshot.GetRawJsonValue();

        Matching matching = JsonUtility.FromJson<Matching>(json);
        matching.ChangeStructure(true);

        // 방이 사라졌을 때
        if (matching.krp.ContainsKey(myRoomKey) == false)
        {
            myRoom = null;
            myRoomKey = null;

            matchingRef.ValueChanged -= OnMatch;
            Get<GameObject>((int)GameObjects.Matching).SetActive(false);
            Get<Button>((int)Buttons.RankMatchButton).gameObject.EventActive(true);
            return;
        }

        if (matching.krp[myRoomKey].canStart && Manager.Data.Auth.CurrentUser.UserId == myRoom.uids[1])
        {
            // 두명 전부 완료된 상황
            // 1P는 이미 전투 진입완료
            // 2P가 방정리하고 전투 진입하면 됨

            matching.krp.Remove(myRoomKey);
            matching.ChangeStructure(false);
        }
        // 1P만 들어감
        else if (matching.krp[myRoomKey].isFull &&
            matching.krp[myRoomKey].canStart == false &&
            Manager.Data.Auth.CurrentUser.UserId == myRoom.uids[0])
        {
            // 2P가 방에 들어온 상황
            // 1P는 2P가 들어온 것을 확인하고 밴픽화면으로 진입

            matching.krp[myRoomKey].canStart = true;

            // 나의 방 정보 최신화
            myRoom = matching.krp[myRoomKey];
        }
        // 다른 방이 변동되었다면
        else
        {
            return;
        }

        matchingRef.ValueChanged -= OnMatch;
        string newJson = JsonUtility.ToJson(matching);
        matchingRef.SetRawJsonValueAsync(newJson).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                // 취소
                // 한명이라도 접속을 못 했을 시 실행할 로직
                return;
            }
            else if (task.IsFaulted)
            {
                // 실패
                // 한명이라도 접속을 못 했을 시 실행할 로직
                return;
            }

            Debug.Log($"매칭 완료됨, 1P : {myRoom.uids[0]}, 2P : {myRoom.uids[1]}");

            Manager.Game.SetRoomInfo(myRoom);
            StartCoroutine(ToBanPickRoutine());
        });
    }

    private IEnumerator ToBanPickRoutine()
    {
        yield return new WaitForSeconds(2f);
        Manager.UI.ClosePopupUI(this);
        Manager.UI.ShowPopupUI<UI_BanPickPopup>();
    }

    private void InformationInit()
    {
        string userID = Manager.Data.Auth.CurrentUser.UserId;
        userInfoRef.Child(userID).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                // 불러오기 취소 처음 화면으로
                SignOut();
                return;
            }
            else if (task.IsFaulted)
            {
                // 불러오기 실패 처음 화면으로
                SignOut();
                return;
            }
            DataSnapshot snapshot = task.Result;
            string json = snapshot.GetRawJsonValue();

            UserInfo userInfo = JsonUtility.FromJson<UserInfo>(json);
            if (userInfo == null)
            {
                int[] newLog = { 0, 0 };
                int rand = UnityEngine.Random.Range(0, 1000);
                userInfo = new UserInfo() { nickName = $"new{rand}", rankPoint = 0, log = newLog, isConnect = true };

                string newJson = JsonUtility.ToJson(userInfo);
                userInfoRef.Child(userID).SetRawJsonValueAsync(newJson);
            }

            Get<TMP_Text>((int)Texts.ProfileImageText).text = userInfo.nickName;
            Get<TMP_Text>((int)Texts.LogInfoText).text = $"{userInfo.log[0]}승 / {userInfo.log[1]}패";
            Get<TMP_Text>((int)Texts.RankPointText).text = $"점수 : {userInfo.rankPoint}";

            Manager.Game.SetMyInfo(userInfo);

            var send = new Dictionary<string, object> { { "isConnect", true } };
            userInfoRef.Child(userID).UpdateChildrenAsync(send);
        });
    }

    private void SignOut()
    {
        Manager.Data.Auth.SignOut();
        Manager.UI.ClosePopupUI(this);
        Manager.UI.ShowPopupUI<UI_TitlePopup>();
    }
}

[Serializable]
public class Matching
{
    public const string Root = "Matching/";

    public bool exist;
    public List<Room> rooms;
    /// <summary>
    /// key room pair
    /// </summary>
    public Dictionary<string, Room> krp;

    /// <summary>
    /// true : list => dictionary, false : dictionary => list
    /// </summary>
    /// <param nickName="listToDictionary"></param>
    public void ChangeStructure(bool listToDictionary)
    {
        if (listToDictionary)
        {
            krp = rooms.ToDictionary(item => item.key);
        }
        else
        {
            rooms = krp.Values.ToList();
        }
    }

    public Matching()
    {
        rooms = new List<Room>();
        exist = true;
    }
}

[Serializable]
public class Room
{
    public string key;
    public string[] uids;
    public bool isFull;
    public bool canStart;

    public Room(string key, string uid)
    {
        this.key = key;
        uids = new string[2];
        uids[0] = uid;
        isFull = false;
        canStart = false;
    }

    public Room() { /*테스트용*/}
}

[Serializable]
public class UserInfo
{
    public const string Root = "UserInfo/";

    public string nickName;
    public int rankPoint;
    /// <summary>
    /// idx : 0 = win, 1 = defeat
    /// </summary>
    public int[] log;
    public bool isConnect;
}
