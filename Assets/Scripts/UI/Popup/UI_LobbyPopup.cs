using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
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
        MockBattleDisplay, // 원래는 모의전으로 만들려고 했으나 지금은 소통게시판
        BattleLogDisplay,
        RankingDisplay,
        Menu,
        Matching,
        PostContent,
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
        RankMatchButtonText,
    }

    private enum InputFields
    {
        PostInputField,
    }

    private DatabaseReference userInfoRef = null;
    private bool isMatching = false;

    private DatabaseReference communityRef = null;
    private Queue<UI_Post> postQue = new Queue<UI_Post>(10);

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) &&
            Get<GameObject>((int)GameObjects.MockBattleDisplay).activeSelf)
        {
            OnClickCreateRoomButton();
        }
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));
        Bind<TMP_Text>(typeof(Texts));
        Bind<TMP_InputField>(typeof(InputFields));

        ActiveDisplay(GameObjects.RankBattleDisplay);

        Get<Button>((int)Buttons.MenuButton).gameObject.BindEvent(OnClickMenuButton);
        Get<Button>((int)Buttons.RankBattleButton).gameObject.BindEvent(OnClickRankBattleButton);
        Get<Button>((int)Buttons.MockBattleButton).gameObject.BindEvent(OnClickMockBattleButton);
        Get<Button>((int)Buttons.BattleLogButton).gameObject.BindEvent(OnClickBattleLogButton);
        Get<Button>((int)Buttons.RankingButton).gameObject.BindEvent(OnClickRankingButton);
        Get<Button>((int)Buttons.RankMatchButton).gameObject.BindEvent(OnClickRankMatchButton);
        Get<Button>((int)Buttons.CreateRoomButton).gameObject.BindEvent(OnClickCreateRoomButton);
        Get<Button>((int)Buttons.LogoutButton).gameObject.BindEvent(OnClickLogoutButton);
        Get<Button>((int)Buttons.QuitButton).gameObject.BindEvent(OnClickQuitButton);

        Get<GameObject>((int)GameObjects.Menu).SetActive(false);
        Get<GameObject>((int)GameObjects.Matching).SetActive(false);

        userInfoRef = Manager.Data.DB.GetReference(UserInfo.Root)
            .Child(Manager.Data.Auth.CurrentUser.UserId);
        communityRef = Manager.Data.DB.GetReference(Community.Root);

        InformationInit();

        PlayerManager.Instance.OnEnterRoom -= OnEnterRoom;
        PlayerManager.Instance.OnEnterRoom += OnEnterRoom;
        PlayerManager.Instance.OnLeaveRoom -= OnLeaveRoom;
        PlayerManager.Instance.OnLeaveRoom += OnLeaveRoom;

        communityRef.ValueChanged -= OnUpdatePost;
        communityRef.ValueChanged += OnUpdatePost;

        return true;
    }

    private void OnClickMenuButton()
    {
        GameObject menu = Get<GameObject>((int)GameObjects.Menu);
        menu.SetActive(menu.activeSelf == false);
    }

    private void OnClickQuitButton() { Application.Quit(); }

    private void OnClickRankBattleButton() { ActiveDisplay(GameObjects.RankBattleDisplay); }

    private void OnClickMockBattleButton()
    {
        ActiveDisplay(GameObjects.MockBattleDisplay);
        Get<TMP_InputField>((int)InputFields.PostInputField).ActivateInputField();
    }

    private void OnClickBattleLogButton() { ActiveDisplay(GameObjects.BattleLogDisplay); }

    private void OnClickRankingButton() { ActiveDisplay(GameObjects.RankingDisplay); }

    private void OnClickRankMatchButton()
    {
        Get<GameObject>((int)GameObjects.Matching).SetActive(isMatching == false);
        Get<Button>((int)Buttons.RankMatchButton).gameObject.EventActive(false);

        if (isMatching == false)
        {
            Debug.Log("Match Start");
            Manager.Network.Connect();
        }
        else
        {
            Debug.Log("Match Cancel");
            Manager.Network.Disconnect();
        }

        isMatching = isMatching == false;
        StartCoroutine(WaitConnectRoutine());
    }

    private IEnumerator WaitConnectRoutine()
    {
        string buttonText = isMatching ? "취소" : "전투 신청";
        Get<TMP_Text>((int)Texts.RankMatchButtonText).text = buttonText;
        yield return new WaitForSeconds(2f);
        Get<Button>((int)Buttons.RankMatchButton).gameObject.EventActive(true);
    }

    private void OnClickCreateRoomButton()
    {
        TMP_InputField postInput = Get<TMP_InputField>((int)InputFields.PostInputField);
        if (string.IsNullOrEmpty(postInput.text))
            return;

        Button button = Get<Button>((int)Buttons.CreateRoomButton);
        button.gameObject.EventActive(false);

        string nick = Manager.Game.MyInfo.nickName;
        string post = $"{nick} : {postInput.text}";

        var send = new Dictionary<string, object>
        { { "post", post } ,{$"logs/{DateTime.Now}", post } };
        communityRef.UpdateChildrenAsync(send).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
                postInput.text = string.Empty;

            button.gameObject.EventActive(true);
            Get<TMP_InputField>((int)InputFields.PostInputField).ActivateInputField();
        });
    }

    private void OnClickLogoutButton()
    {
        var send = new Dictionary<string, object> { { "isConnect", false } };
        userInfoRef.UpdateChildrenAsync(send);
    }

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

    private void InformationInit()
    {
        userInfoRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted == false)
            {
                // 불러오기 실패 처음 화면으로
                Manager.Data.SignOut();
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
                userInfoRef.SetRawJsonValueAsync(newJson);
            }

            Get<TMP_Text>((int)Texts.ProfileImageText).text = userInfo.nickName;
            Get<TMP_Text>((int)Texts.LogInfoText).text = $"{userInfo.log[0]}승 / {userInfo.log[1]}패";
            Get<TMP_Text>((int)Texts.RankPointText).text = $"점수 : {userInfo.rankPoint}";

            Manager.Game.SetMyInfo(userInfo);
        });
    }

    private void OnEnterRoom(int playerId)
    {
        // Player ID : 상대 ID
        Manager.Game.IsFirst = playerId == 1;
        Manager.UI.ClearPopupUI();
        Manager.UI.ShowPopupUI<UI_BanPickPopup>();
    }

    private void OnLeaveRoom()
    {
        Manager.UI.ShowPopupUI<UI_NetworkNotification>();
    }

    private void OnUpdatePost(object obj, ValueChangedEventArgs args)
    {
        string json = args.Snapshot.GetRawJsonValue();
        Community community = JsonUtility.FromJson<Community>(json);
        if (community == null)
            return;

        if (postQue.Count == 10)
        {
            UI_Post oldPost = postQue.Dequeue();
            oldPost.SetInfo(community.post);
            oldPost.gameObject.transform.SetAsLastSibling();
            postQue.Enqueue(oldPost);
        }
        else
        {
            Transform postContent = Get<GameObject>((int)GameObjects.PostContent).transform;
            UI_Post newPost = Manager.UI.MakeSubItem<UI_Post>(postContent);
            newPost.SetInfo(community.post);
            postQue.Enqueue(newPost);
        }
    }

    private void OnDestroy()
    {
        PlayerManager.Instance.OnEnterRoom -= OnEnterRoom;
        communityRef.ValueChanged -= OnUpdatePost;
    }
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
    public bool isConnect; // 중복로그인 확인용 변수
}

public class Community
{
    public const string Root = "Community/";

    public string post;
}
