using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.Diagnostics;

public class ResultManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject resultPanel;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            resultPanel.SetActive(true); 
        }
    }
    // ��: ��� ȭ�鿡�� ��ư ������ ȣ��
    public void OnLeaveRoomButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
        OnLeftRoom();
    }

    public override void OnLeftRoom()
    {
        // ���ϴ� ������ �̵� (��: �κ� ��)
        SceneManager.LoadScene("00.Room"); // ���⿡ ���� �� �̸� �Է�
    }

}
