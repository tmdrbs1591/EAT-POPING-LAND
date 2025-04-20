using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using Photon.Realtime;

public class BattleManager : MonoBehaviourPun
{
    public static BattleManager instance;

    public GameObject battleCamera;
    public GameObject battleScreen;

    public Transform firstBattlePos;
    public Transform secondBattlePos;

    private Dictionary<int, Vector3> playerOriginalPositions = new Dictionary<int, Vector3>(); // 플레이어 원래 위치 저장

    [SerializeField] TMP_Text firstPlayerNameText;
    [SerializeField] TMP_Text secondPlayerNameText;
    [SerializeField] GameObject battlePanel;
    [SerializeField] public GameObject winnerPanel;
    [SerializeField] TMP_Text winnerNameText;


    private string opponentName = "";

    private int player1ID;
    private int player2ID;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.O))
        //{
        //    BattleWin();
        //}
        if (Input.GetKeyDown(KeyCode.L))
        {
            BattleLose();
        }
    }

    public void SetBattleInfo(string opponent)
    {
        opponentName = opponent;
    }

    public void BattleStart()
    {
        if (string.IsNullOrEmpty(opponentName))
            return;

        photonView.RPC("RPC_BattleStart", RpcTarget.All, PhotonNetwork.NickName, opponentName);
    }

    [PunRPC]
    private void RPC_BattleStart(string player1, string player2)
    {
        battleScreen.SetActive(true);
        battleCamera.SetActive(true);
        battlePanel.SetActive(false);
        battlePanel.SetActive(true);

        firstPlayerNameText.text = player1;
        secondPlayerNameText.text = player2;

        SavePlayerPositions(player1, player2);
        MovePlayerToBattlePos(player1, firstBattlePos.position);
        MovePlayerToBattlePos(player2, secondBattlePos.position);

        // ✅ 싸운 플레이어 ID 저장
        player1ID = PhotonNetwork.CurrentRoom.Players.Values.First(p => p.NickName == player1).ActorNumber;
        player2ID = PhotonNetwork.CurrentRoom.Players.Values.First(p => p.NickName == player2).ActorNumber;
    }

    private void SavePlayerPositions(string player1, string player2)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var player in players)
        {
            PhotonView pv = player.GetComponent<PhotonView>();
            if (pv != null)
            {
                int playerID = pv.Owner.ActorNumber;
                playerOriginalPositions[playerID] = player.transform.position; // 원래 위치 저장
            }
        }
    }

    private void MovePlayerToBattlePos(string playerName, Vector3 position)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var player in players)
        {
            PhotonView pv = player.GetComponent<PhotonView>();
            if (pv != null && pv.Owner.NickName == playerName)
            {
                pv.RPC("RPC_SetBattlePosition", RpcTarget.All, position);
                pv.RPC("RPC_SetRotation", RpcTarget.All, 0f,0f,0f);
                break;
            }
        }
    }

    //public void BattleWin()
    //{
    //    int winnerID = PhotonNetwork.LocalPlayer.ActorNumber;
    //    photonView.RPC("RPC_BattleResult", RpcTarget.All, winnerID);
    //}
    public void BattleLose()
    {
        int myID = PhotonNetwork.LocalPlayer.ActorNumber;
        int opponentID = (myID == player1ID) ? player2ID : player1ID;

        Player winner = PhotonNetwork.CurrentRoom.GetPlayer(opponentID);
        Player loser = PhotonNetwork.LocalPlayer;

        string winnerName = winner.NickName;

        // 승자와 패자에게만 승패 판단용
        photonView.RPC("RPC_BattleResult", winner, opponentID);
        photonView.RPC("RPC_BattleResult", loser, opponentID);

        // 모든 클라이언트에게 승자 이름 보여주기
        photonView.RPC("RPC_ShowWinner", RpcTarget.All, winnerName);
    }


    [PunRPC]
    private void RPC_BattleResult(int winnerID)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == winnerID)
        {
            Debug.Log("승리!");
        }
        else
        {
            Debug.Log("패배!");
        }
    }
    [PunRPC]
    public void RPC_ShowWinner(string winnerName)
    {
        winnerPanel.SetActive(true);
        winnerNameText.text = $"{winnerName} WIN";
    }

    [PunRPC]
    public void ResetPosPlayerRPC() //플레이어 위치 다시 제자리로
    {
        ResetPlayerPositions();
    }

    [PunRPC]
    private void RPC_BattlePanelFalse()
    {
        battlePanel.SetActive(false);
        battleScreen.SetActive(false);
    }
    private void ResetPlayerPositions()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var player in players)
        {
            PhotonView pv = player.GetComponent<PhotonView>();
            if (pv != null)
            {
                int playerID = pv.Owner.ActorNumber;
                if (playerOriginalPositions.ContainsKey(playerID))
                {
                    Vector3 originalPos = playerOriginalPositions[playerID];
                    pv.RPC("RPC_SetRePosition", RpcTarget.All, originalPos); // 원래 위치로 복귀
                    pv.RPC("RPC_SetRotation", RpcTarget.All, 31.7f, 42.8f, 0f);
                }
            }
        }

        playerOriginalPositions.Clear(); // 위치 데이터 초기화
    }
}
