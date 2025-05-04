using UnityEngine;
using BackEnd;

public class LoginSample : MonoBehaviour
{
    private void Awake()
    {
        string ID = "user01";
        string PW = "1234";
        string email = "user01@gmail.com";
        string nickname = "ù��°����";

        Backend.BMember.CustomSignUp(ID, PW);

        Backend.BMember.UpdateCustomEmail(email);

        Backend.BMember.CustomLogin(ID, PW);

        Backend.BMember.FindCustomID(email);

        Backend.BMember.ResetPassword(ID, email);

        Backend.BMember.CreateNickname(nickname);

        Backend.BMember.UpdateNickname(nickname);

    }

}
