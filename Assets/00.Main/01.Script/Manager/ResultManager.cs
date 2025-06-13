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

    private void Awake()
    {
        StartCoroutine(PanelOpenCor());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Application.Quit();
        }
     

    }
    // 예: 결과 화면에서 버튼 누르면 호출
    public void OnLeaveRoomButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("00.Room"); // 여기에 실제 씬 이름 입력
    }

    IEnumerator PanelOpenCor()
    {
        yield return new WaitForSeconds(4f);
        resultPanel.SetActive(true);
    }
}
