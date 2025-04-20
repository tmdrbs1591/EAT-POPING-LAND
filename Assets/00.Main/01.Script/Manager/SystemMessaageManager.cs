using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
public class SystemMessaageManager : MonoBehaviourPunCallbacks
{
    public static SystemMessaageManager instance;

    public GameObject systemMegapon;
    public TMP_Text systemText;

    private void Awake()
    {
        instance = this;
    }

    public void MessageTextStart(string text)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("MessageTextStartRPC",RpcTarget.All,text);
        }
    }

    [PunRPC]
    public void MessageTextStartRPC(string text)
    {
        StartCoroutine(MessageTextStartCor(text));
    }
    IEnumerator MessageTextStartCor(string text)
    {
        systemMegapon.SetActive(true);
        systemText.text = "";

        yield return new WaitForSeconds(1f);
        // 한 글자씩 출력
        for (int i = 0; i < text.Length; i++)
        {
            systemText.text += text[i];
            yield return new WaitForSeconds(0.05f); // 딲딲딲 템포 (원하는 속도 조절 가능)
        }

        // 전체 텍스트가 출력된 후 5초 기다리고 끔
        yield return new WaitForSeconds(3.5f);
        systemMegapon.SetActive(false);
    }
}
