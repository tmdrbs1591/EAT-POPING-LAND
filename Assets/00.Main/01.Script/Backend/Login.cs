using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BackEnd;
using UnityEngine.UI;
public class Login : LoginBase
{
    [SerializeField] private Image imageID;
    [SerializeField] private TMP_InputField inputFieldID;
    [SerializeField] private Image imagePW;
    [SerializeField] private TMP_InputField inputFieldPW;

    [SerializeField] private Button btnLogin;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            {
                OnClickLogin();
            }
        }
    }
    public void OnClickLogin()
    {
        ResetUI(imageID, imagePW);
        if (IsFieldDataEmpty(imageID, inputFieldID.text, "아이디")) return;
        if (IsFieldDataEmpty(imagePW, inputFieldPW.text, "비밀번호")) return;

        btnLogin.interactable = false;

        StartCoroutine(nameof(LoginProcess));

        ResponseToLogin(inputFieldID.text, inputFieldPW.text);

    }

    private void ResponseToLogin(string ID, string PW)
    {
        Backend.BMember.CustomLogin(ID, PW, callback =>
        {
            StopCoroutine(nameof(LoginProcess));

            if (callback.IsSuccess())
            {
                SetMessage($"{inputFieldID.text}님 환영합니다.");
            }
            else
            {
                //로그인 실패
                btnLogin.interactable = true;
                string message = string.Empty;

                switch (int.Parse(callback.GetStatusCode()))
                { 
                    case 401:
                        message = callback.GetMessage().Contains("customId") ? "존재하지 않는 아이디입니다." : "잘못된 비밀번호입니다.";
                        break;
                    case 403:
                        message = callback.GetMessage().Contains("user") ? "차단당한 유저입니다." : "차단당한 디바이스 입니다.";
                        break;
                    case 410:
                         message = "탈퇴가 진행중인 유저입니다.";
                        break;
                    default:
                        message = callback.GetMessage();
                        break;
                }

                if (message.Contains("비밀번호"))
                {
                    GuideForIncorretlyEnterData(imagePW, message);
                }
                else
                {
                    GuideForIncorretlyEnterData(imageID, message);
                }
            }
        });
    }
    private IEnumerator LoginProcess()
    {
        float time = 0;

        while (true)
        {
            time += Time.deltaTime;

            SetMessage($"로그인 중입니다 ... {time:F1}");
            yield return null;
        }
    }
}
