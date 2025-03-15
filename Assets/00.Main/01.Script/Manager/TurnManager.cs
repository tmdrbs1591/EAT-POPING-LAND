using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class TurnManager : MonoBehaviourPunCallbacks
{
    public static TurnManager instance;

    public int currentPlayerIndex = 0; // 현재 턴인 플레이어 인덱스
    public int totalPlayers; // 전체 플레이어 수

    [SerializeField] GameObject myTurnPanel;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        totalPlayers = PhotonNetwork.PlayerList.Length; // 플레이어 수 가져오기
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("StartTurn", RpcTarget.All, currentPlayerIndex);
        }
    }

    [PunRPC]
    public void StartTurn(int playerIndex)
    {
        currentPlayerIndex = playerIndex;

        // 내 차례일 때
        if (PhotonNetwork.LocalPlayer.ActorNumber == playerIndex + 1)
        {
            DiceManager.instance.enabled = true;
            myTurnPanel.SetActive(false);
            myTurnPanel.SetActive(true);
            Debug.Log("나의 턴!");
        }
        else
        {
            DiceManager.instance.enabled = false;
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
        // 플레이어 수 동기화
        totalPlayers = PhotonNetwork.PlayerList.Length;
    }
}
