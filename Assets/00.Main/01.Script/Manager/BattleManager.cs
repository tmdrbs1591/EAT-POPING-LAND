using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;

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

    private string opponentName = "";

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            BattleWin();
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
                pv.RPC("RPC_SetPosition", RpcTarget.All, position);
                pv.RPC("RPC_SetRotation", RpcTarget.All, -20f, -45f, 20f);
                break;
            }
        }
    }

    public void BattleWin()
    {
        int winnerID = PhotonNetwork.LocalPlayer.ActorNumber;
        photonView.RPC("RPC_BattleResult", RpcTarget.All, winnerID);
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
            Debug.Log(" 패배!");
        }

        battlePanel.SetActive(false);
        battleScreen.SetActive(false);

        ResetPlayerPositions();
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
                    pv.RPC("RPC_SetPosition", RpcTarget.All, originalPos); // 원래 위치로 복귀
                    pv.RPC("RPC_SetRotation", RpcTarget.All, 0f, 0f, 0f);
                }
            }
        }

        playerOriginalPositions.Clear(); // 위치 데이터 초기화
    }
}
