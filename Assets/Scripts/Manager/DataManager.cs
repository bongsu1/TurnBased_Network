using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Firebase.Auth;
using Firebase.Database;

public class DataManager
{
    // 데이터 파일 관리 (세이브&로드)

    public void Init()
    {
        CheckFirebase();
    }

    #region Firebase

    private bool isVaild = false;
    public bool IsVaild { get { return isVaild; } }

    private FirebaseApp _app = null;
    private FirebaseAuth _auth = null;
    private FirebaseDatabase _db = null;

    public FirebaseApp App { get { return _app; } }
    public FirebaseAuth Auth { get { return _auth; } }
    public FirebaseDatabase DB { get { return _db; } }

    private void CheckFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                _app = FirebaseApp.DefaultInstance;
                _auth = FirebaseAuth.DefaultInstance;
                _db = FirebaseDatabase.DefaultInstance;

                Debug.Log("파이어베이스 체크 완료");

                isVaild = true;
            }
            else
            {
                Debug.LogError($"파이어베이스 체크 실패: {dependencyStatus}");
            }
        });
    }

    public void SignOut()
    {
        Manager.Game.SetMyInfo(null);

        _auth.SignOut();
        Manager.UI.ClearPopupUI();
        Manager.UI.ShowPopupUI<UI_TitlePopup>();
    }
    #endregion
}
