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

    private string myRoomKey = null;
    private Room myRoom = null;
    private bool isMatch = false; // 매칭이 되었는지

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
        // 테스트 일단 취소는 안됨
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
        DatabaseReference matchingRef = Manager.Data.DB.GetReference(Matching.Root);

        if (task.IsCanceled)
        {
            // 취소
            Debug.Log("불러오기 취소됨");
            Get<Button>((int)Buttons.RankMatchButton).gameObject.EventActive(true);
            return;
        }
        else if (task.IsFaulted)
        {
            // 실패
            Debug.Log("불러오기 실패함");
            Get<Button>((int)Buttons.RankMatchButton).gameObject.EventActive(true);
            return;
        }

        DataSnapshot snapshot = task.Result;
        string json = snapshot.GetRawJsonValue();
        Matching matching = JsonUtility.FromJson<Matching>(json);

        if (matching == null)
        {
            Debug.Log("데이터 자체가 없음");
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

            Debug.Log("방이 없음");
            Room newRoom = new Room(myRoomKey, uid);
            matching.rooms.Add(newRoom);

            myRoom = newRoom;
        }
        // 방이 있다
        else
        {
            Debug.Log("방이 있음");
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
        DatabaseReference matchingRef = Manager.Data.DB.GetReference(Matching.Root);

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
        matchingRef.ValueChanged -= OnValueChange;
        matchingRef.ValueChanged += OnValueChange;
    }

    private void OnValueChange(object obj, ValueChangedEventArgs args)
    {
        if (isMatch)
            return;

        DatabaseReference matchingRef = Manager.Data.DB.GetReference(Matching.Root);
        string json = args.Snapshot.GetRawJsonValue();

        Matching matching = JsonUtility.FromJson<Matching>(json);
        matching.ChangeStructure(true);

        // 방이 사라졌을 때
        if (matching.krp.TryGetValue(myRoomKey, out Room room) == false)
        {
            myRoom = null;
            myRoomKey = null;

            Get<Button>((int)Buttons.RankMatchButton).gameObject.EventActive(true);
            return;
        }

        // 2P만 들어감
        if (matching.krp[myRoomKey].canStart)
        {
            // 두명 전부 완료된 상황
            // 1P는 이미 전투 진입완료
            // 2P가 방정리하고 전투 진입하면 됨

            matching.krp.Remove(myRoomKey);
            matching.ChangeStructure(false);
        }
        // 1P만 들어감
        else if (matching.krp[myRoomKey].isFull && matching.krp[myRoomKey].canStart == false)
        {
            // 2P가 방에 들어온 상황
            // 1P는 2P가 들어온 것을 확인하고 밴픽화면으로 진입

            matching.krp[myRoomKey].canStart = true;
            matching.ChangeStructure(false);

            // 나의 방 정보 최신화
            myRoom = matching.krp[myRoomKey];
        }
        // 다른 방이 변동되었다면
        else
        {
            return;
        }

        isMatch = true;
        string newJson = JsonUtility.ToJson(matching);
        matchingRef.SetRawJsonValueAsync(newJson).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                // 취소
                isMatch = false;
                // 한명이라도 접속을 못 했을 시 로직
                return;
            }
            else if (task.IsFaulted)
            {
                // 실패
                isMatch = false;
                // 한명이라도 접속을 못 했을 시 로직
                return;
            }

            Debug.Log($"매칭 완료됨, 1P : {myRoom.uids[0]}, 2P : {myRoom.uids[1]}");

            Manager.UI.ClosePopupUI(this);
            Manager.UI.ShowPopupUI<UI_BanPickPopup>();
        });
    }
}

[Serializable]
public class Matching
{
    public const string Root = "Matching/";

    public List<Room> rooms;
    /// <summary>
    /// key room pair
    /// </summary>
    public Dictionary<string, Room> krp;

    /// <summary>
    /// true : list => dictionary, false : dictionary => list
    /// </summary>
    /// <param name="listToDictionary"></param>
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
}
