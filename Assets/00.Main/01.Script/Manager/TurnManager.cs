using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class TurnManager : MonoBehaviourPunCallbacks
{
    public static TurnManager instance;

    public int currentPlayerIndex = 0; // 현재 턴인 플레이어 인덱스
    public int totalPlayers; // 전체 플레이어 수
    public int countdown = 15;

    [SerializeField] GameObject myTurnPanel;
    [SerializeField] TMP_Text countdownText; // 카운트다운 표시 UI

    public bool isCountingDown = false; // 카운트다운 중인지 확인
    private bool isFirstTurn = true; // 첫 턴인지 확인
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        totalPlayers = PhotonNetwork.PlayerList.Length; // 플레이어 수 가져오기
        if (PhotonNetwork.IsMasterClient && isFirstTurn)
        {
            isFirstTurn = false;
            photonView.RPC("StartCountdown", RpcTarget.All, currentPlayerIndex);
        }
    }

    [PunRPC]
    public void StartCountdown(int playerIndex)
    {
        StartCoroutine(CountdownCoroutine(playerIndex));
    }

    private IEnumerator CountdownCoroutine(int playerIndex)
    {
        isCountingDown = true; // 카운트다운 시작
        DiceManager.instance.enabled = false; // 주사위 비활성화

   
        while (countdown > 0)
        {
            countdownText.text = $"{countdown}초 뒤에 시작합니다.";
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        countdownText.text = "턴 시작!";
        yield return new WaitForSeconds(1f);
        countdownText.text = "";

        isCountingDown = false; // 카운트다운 종료
        photonView.RPC("StartTurn", RpcTarget.All, playerIndex);
    }

    [PunRPC]
    public void StartTurn(int playerIndex)
    {
        currentPlayerIndex = playerIndex;

        // 내 차례일 때
        if (PhotonNetwork.LocalPlayer.ActorNumber == playerIndex + 1)
        {
            DiceManager.instance.enabled = true; // 주사위 활성화
            myTurnPanel.SetActive(true);
            Debug.Log("나의 턴!");
        }
        else
        {
            DiceManager.instance.enabled = false;
            myTurnPanel.SetActive(false);
        }
    }

    public void EndTurn()
    {

        int nextPlayerIndex = (currentPlayerIndex + 1) % totalPlayers;
        photonView.RPC("StartTurn", RpcTarget.All, nextPlayerIndex);

        DiceManager.instance.isDice = false;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        totalPlayers = PhotonNetwork.PlayerList.Length;
    }
}
