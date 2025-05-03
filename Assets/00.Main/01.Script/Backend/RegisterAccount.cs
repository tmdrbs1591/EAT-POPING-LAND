using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BackEnd;
using Unity.VisualScripting;

public class RegisterAccount : LoginBase
{
    [SerializeField]
    private Image imageID;
    [SerializeField]
    private TMP_InputField inputFieldID;
    [SerializeField]
    private Image imagePW;
    [SerializeField]
    private TMP_InputField inputFieldPW;
    [SerializeField]
    private Image imageConfirmPW;
    [SerializeField]
    private TMP_InputField inputFieldConfirmPW;
    [SerializeField]
    private Image imageEmail;
    [SerializeField]
    private TMP_InputField inputFieldEmail;

    [SerializeField]
    private Button btnRegisterAccount;

    /// <summary>
    ///   �������� ��ư�� �������� ȣ��
    /// </summary>
    public void OnClickRegisterAccount()
    {
        ResetUI(imageID,imagePW,imageConfirmPW,imageEmail);

        if (IsFieldDataEmpty(imageID, inputFieldID.text, "���̵�")) return;
        if (IsFieldDataEmpty(imagePW, inputFieldPW.text, "��й�ȣ")) return;
        if (IsFieldDataEmpty(imageConfirmPW, inputFieldConfirmPW.text, "��й�ȣ Ȯ��")) return;
        if (IsFieldDataEmpty(imageEmail, inputFieldEmail.text, "���� �ּ�")) return;

        //��й�ȣ�� ��й�ȣ Ȯ�� �����̴ٸ���
        if (inputFieldPW.text != inputFieldConfirmPW.text)
        {
            GuideForIncorretlyEnterData(imageConfirmPW, "��й�ȣ�� ��ġ���� �ʽ��ϴ�");
            return;
        }


        if (!inputFieldEmail.text.Contains("@"))
        {
            GuideForIncorretlyEnterData(imageEmail, "���� ������ �߸��Ǿ����ϴ�. (ex. address@xx.xx)");
            return;
        }

        btnRegisterAccount.interactable = false;
        SetMessage("���� �������Դϴ�..");

        CustomSignUp();

    }

    private void CustomSignUp()
    {
        Backend.BMember.CustomSignUp(inputFieldID.text, inputFieldPW.text, callback =>
        {

            btnRegisterAccount.interactable = true;

            if (callback.IsSuccess())
            {
                SetMessage($"���� ���� ����. {inputFieldID.text}�� ȯ���մϴ�.");
            }
            else
            {
                string message = string.Empty;

                switch (int.Parse(callback.GetStatusCode()))
                {

                    case 409:
                        message = "�̹� �����ϴ� ���̵��Դϴ�";
                        break;
                    case 403:
                    case 401:
                    case 400:
                    default:
                        message = callback.GetMessage();
                        break;
                }
                if (message.Contains("���̵�"))
                {
                    GuideForIncorretlyEnterData(imageID, message);
                }
                else
                {
                    SetMessage(message);
                }
            }
        });
    }
}
