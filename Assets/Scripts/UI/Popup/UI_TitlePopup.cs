using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        ShowPWButton,
    }

    private DatabaseReference userInfoRef = null;

    private void Update()
    {
        if (_init == false)
            return;

        if (Get<TMP_InputField>((int)InputFields.IDInputField).isFocused)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Get<TMP_InputField>((int)InputFields.PasswordInputField).ActivateInputField();
            }
        }
        else if (Get<TMP_InputField>((int)InputFields.PasswordInputField).isFocused)
        {
            if (Input.GetKey(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
            {
                Get<TMP_InputField>((int)InputFields.IDInputField).ActivateInputField();
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnClickLoginButton();
        }
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
        Get<Button>((int)Buttons.ShowPWButton).gameObject.BindEvent(OnClickShowPWButton);

        Get<TMP_InputField>((int)InputFields.IDInputField).gameObject.BindEvent(LoadPrefs);
        Get<TMP_InputField>((int)InputFields.PasswordInputField).gameObject.BindEvent(LoadPrefs);

        return true;
    }

    private void OnClickLoginButton()
    {
        string email = Get<TMP_InputField>((int)InputFields.IDInputField).text;
        string password = Get<TMP_InputField>((int)InputFields.PasswordInputField).text;

        if (Manager.Data.IsVaild == false)
            return;

        ActiveButton(false);
        Manager.Data.Auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(ToLobby);
    }

    private void OnGuestButtonClick()
    {
        if (Manager.Data.IsVaild == false)
            return;

        ActiveButton(false);
        Manager.Data.Auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(ToLobby);
    }

    private void OnClickSignupButton()
    {
        if (Manager.Data.IsVaild == false)
            return;

        Manager.UI.ShowPopupUI<UI_SignupPopup>();
    }

    private void OnClickShowPWButton()
    {
        TMP_InputField passwordInput = Get<TMP_InputField>((int)InputFields.PasswordInputField);
        switch (passwordInput.contentType)
        {
            case TMP_InputField.ContentType.Standard:
                passwordInput.contentType = TMP_InputField.ContentType.Password;
                break;
            case TMP_InputField.ContentType.Password:
                passwordInput.contentType = TMP_InputField.ContentType.Standard;
                break;
            default:
                passwordInput.contentType = TMP_InputField.ContentType.Standard;
                break;
        }

        passwordInput.Select();
    }

    private void ToLobby(Task<AuthResult> task)
    {
        if (task.IsCanceled)
        {
            Debug.LogError($"로그인 취소");
            ActiveButton(true);
            return;
        }
        else if (task.IsFaulted)
        {
            Debug.LogError($"로그인 실패 {task.Exception}");
            ActiveButton(true);
            return;
        }

        string email = Get<TMP_InputField>((int)InputFields.IDInputField).text;
        string password = Get<TMP_InputField>((int)InputFields.PasswordInputField).text;

        PlayerPrefs.SetString("email", email);
        PlayerPrefs.SetString("password", password);

        string userId = Manager.Data.Auth.CurrentUser.UserId;
        userInfoRef = Manager.Data.DB.GetReference(UserInfo.Root).Child(userId);

        SignInCheck().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Manager.Data.Auth.SignOut();
                Debug.LogError($"로그인 체크 취소");
                return;
            }
            else if (task.IsFaulted)
            {
                Manager.Data.Auth.SignOut();
                Debug.LogError($"로그인 체크 실패 {task.Exception}");
                return;
            }

            userInfoRef.ValueChanged -= OnDuplicationSignIn;
            userInfoRef.ValueChanged += OnDuplicationSignIn;

            Manager.UI.ClosePopupUI(this);
            Manager.UI.ShowPopupUI<UI_LobbyPopup>();
        });
    }

    private void ActiveButton(bool active)
    {
        Get<Button>((int)Buttons.LoginButton).interactable = active;
        Get<Button>((int)Buttons.LoginButton).gameObject.EventActive(active);

        Get<Button>((int)Buttons.GuestButton).interactable = active;
        Get<Button>((int)Buttons.GuestButton).gameObject.EventActive(active);

        Get<Button>((int)Buttons.SignupButton).interactable = active;
        Get<Button>((int)Buttons.SignupButton).gameObject.EventActive(active);
    }

    private Task SignInCheck()
    {
        var send = new Dictionary<string, object> { { "isConnect", false } };
        userInfoRef.UpdateChildrenAsync(send);

        send = new Dictionary<string, object> { { "isConnect", true } };
        return userInfoRef.UpdateChildrenAsync(send);
    }

    private void OnDuplicationSignIn(object obj, ValueChangedEventArgs args)
    {
        string json = args.Snapshot.GetRawJsonValue();
        UserInfo userInfo = JsonUtility.FromJson<UserInfo>(json);
        if (userInfo == null)
            return;

        if (userInfo.isConnect)
            return;

        userInfoRef.ValueChanged -= OnDuplicationSignIn;
        Manager.Data.SignOut();
    }

    bool isLoaded = false; // 한번만 실행
    private void LoadPrefs() // 인풋필드를 클릭하면 실행
    {
        if (isLoaded)
            return;

        string email = PlayerPrefs.GetString("email", null);
        if (string.IsNullOrEmpty(email))
            return;

        string password = PlayerPrefs.GetString("password", null);
        if (string.IsNullOrEmpty(password))
            return;

        Get<TMP_InputField>((int)InputFields.IDInputField).text = email;
        Get<TMP_InputField>((int)InputFields.PasswordInputField).text = password;
        isLoaded = true;
    }
}
