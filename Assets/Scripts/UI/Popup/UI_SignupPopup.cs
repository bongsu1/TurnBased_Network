using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SignupPopup : UI_Popup
{
    private enum Images
    {
        Outside,
        Notification,
    }

    private enum InputFields
    {
        IDInputField,
        PasswordInputField,
    }

    private enum Buttons
    {
        SignupButton,
        NotificationButton,
    }

    private enum Texts
    {
        NotificationText,
    }

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
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Image>(typeof(Images));
        Bind<TMP_InputField>(typeof(InputFields));
        Bind<Button>(typeof(Buttons));
        Bind<TMP_Text>(typeof(Texts));

        Get<Image>((int)Images.Outside).gameObject.BindEvent(CloseThisUI);
        Get<Button>((int)Buttons.SignupButton).gameObject.BindEvent(OnClickSignupButton);
        Get<Button>((int)Buttons.NotificationButton).gameObject.BindEvent(CloseNotification);

        CloseNotification();

        return true;
    }

    private void OnClickSignupButton()
    {
        Get<Button>((int)Buttons.SignupButton).interactable = false;
        Get<Button>((int)Buttons.SignupButton).gameObject.EventActive(false);

        string email = Get<TMP_InputField>((int)InputFields.IDInputField).text;
        string password = Get<TMP_InputField>((int)InputFields.PasswordInputField).text;

        Manager.Data.Auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Get<Button>((int)Buttons.SignupButton).interactable = true;
                Get<Button>((int)Buttons.SignupButton).gameObject.EventActive(true);

                Get<TMP_Text>((int)Texts.NotificationText).text = "회원가입 취소";
                Get<Image>((int)Images.Notification).gameObject.SetActive(true);
                return;
            }
            if (task.IsFaulted)
            {
                Get<Button>((int)Buttons.SignupButton).interactable = true;
                Get<Button>((int)Buttons.SignupButton).gameObject.EventActive(true);

                Get<TMP_Text>((int)Texts.NotificationText).text = "회원가입 실패";
                Get<Image>((int)Images.Notification).gameObject.SetActive(true);
                return;
            }

            Get<TMP_Text>((int)Texts.NotificationText).text = "회원가입 완료";
            Get<Button>((int)Buttons.NotificationButton).gameObject.BindEvent(CloseThisUI);
            Get<Image>((int)Images.Notification).gameObject.SetActive(true);
        });
    }

    private void CloseThisUI()
    {
        Manager.UI.ClosePopupUI(this);
    }

    private void CloseNotification()
    {
        Get<Image>((int)Images.Notification).gameObject.SetActive(false);
    }
}
