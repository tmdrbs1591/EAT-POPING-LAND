using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.Diagnostics;

public class ResultManager : MonoBehaviourPunCallbacks
{
    // 예: 결과 화면에서 버튼 누르면 호출
    public void OnLeaveRoomButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
        OnLeftRoom();
    }

    public override void OnLeftRoom()
    {
        // 원하는 씬으로 이동 (예: 로비 씬)
        SceneManager.LoadScene("00.Room"); // 여기에 실제 씬 이름 입력
    }

  
}
