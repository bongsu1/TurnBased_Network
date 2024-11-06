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

                /*// 테스트 디버그용
                Manager.Data.Auth.SignInWithEmailAndPasswordAsync("rlaqhdtn22@gmail.com", "qwe123").ContinueWith(task =>
                {
                    if (task.IsCanceled)
                    {
                        Debug.LogError("로그인 취소");
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        Debug.LogError($"로그인 실패: {task.Exception}");
                        return;
                    }

                    AuthResult result = task.Result;
                    Debug.LogFormat($"로그인 성공: {result.User.DisplayName} ({result.User.UserId})");
                });*/
                isVaild = true; // 테스트하느라 옮김 다시 옮겨 주세요
            }
            else
            {
                Debug.LogError($"파이어베이스 체크 실패: {dependencyStatus}");
            }
        });
    }
    #endregion
}
