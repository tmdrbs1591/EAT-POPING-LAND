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
        // �� ���ھ� ���
        for (int i = 0; i < text.Length; i++)
        {
            systemText.text += text[i];
            yield return new WaitForSeconds(0.05f); // �M�M�M ���� (���ϴ� �ӵ� ���� ����)
        }

        // ��ü �ؽ�Ʈ�� ��µ� �� 5�� ��ٸ��� ��
        yield return new WaitForSeconds(3.5f);
        systemMegapon.SetActive(false);
    }
}
