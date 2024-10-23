using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Extensions;
using Firebase.Auth;

public class UI_TitlePopup : UI_Popup
{
    private enum InputFields
    {
        IDInputField,
        PasswordInputField,
    }

    private enum Buttons
    {
        LoginButton,
        GuestButton,
        SignupButton,
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<TMP_InputField>(typeof(InputFields));
        Bind<Button>(typeof(Buttons));

        Get<Button>((int)Buttons.LoginButton).gameObject.BindEvent(OnClickLoginButton);
        Get<Button>((int)Buttons.GuestButton).gameObject.BindEvent(OnGuestButtonClick);
        Get<Button>((int)Buttons.SignupButton).gameObject.BindEvent(OnClickSignupButton);

        return true;
    }

    private void OnClickLoginButton()
    {
        /*string email = Get<TMP_InputField>((int)InputFields.IDInputField).text;
        string password = Get<TMP_InputField>((int)InputFields.PasswordInputField).text;

        if (Manager.Data.IsVaild == false)
            return;

        Manager.Data.Auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
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
            Debug.Log($"로그인 성공: {result.User.DisplayName} ({result.User.UserId})");
        });*/
    }

    private void OnGuestButtonClick()
    {
        /*if (Manager.Data.IsVaild == false)
            return;

        Manager.Data.Auth.SignInAnonymouslyAsync().ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("익명 로그인 취소");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError($"익명 로그인 실패: {task.Exception}");
                return;
            }

            AuthResult result = task.Result;
            Debug.LogFormat($"익명 로그인 성공: {result.User.DisplayName} ({result.User.UserId})");
        });*/
    }

    private void OnClickSignupButton()
    {
        // 회원 가입 창 띄우기로 변경해야함
        
        /*string email = Get<TMP_InputField>((int)InputFields.IDInputField).text;
        string password = Get<TMP_InputField>((int)InputFields.PasswordInputField).text;

        if (Manager.Data.IsVaild == false)
            return;

        Manager.Data.Auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("가입 취소");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError($"가입 실패: {task.Exception}");
                return;
            }

            // Firebase user has been created.
            AuthResult result = task.Result;
            Debug.Log($"가입 성공: {result.User.DisplayName} ({result.User.UserId})");
        });*/
    }
}
