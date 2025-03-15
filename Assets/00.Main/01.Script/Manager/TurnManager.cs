using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class TurnManager : MonoBehaviourPunCallbacks
{
    public static TurnManager instance;

    public int currentPlayerIndex = 0; // ���� ���� �÷��̾� �ε���
    public int totalPlayers; // ��ü �÷��̾� ��

    [SerializeField] GameObject myTurnPanel;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        totalPlayers = PhotonNetwork.PlayerList.Length; // �÷��̾� �� ��������
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("StartTurn", RpcTarget.All, currentPlayerIndex);
        }
    }

    [PunRPC]
    public void StartTurn(int playerIndex)
    {
        currentPlayerIndex = playerIndex;

        // �� ������ ��
        if (PhotonNetwork.LocalPlayer.ActorNumber == playerIndex + 1)
        {
            DiceManager.instance.enabled = true;
            myTurnPanel.SetActive(false);
            myTurnPanel.SetActive(true);
            Debug.Log("���� ��!");
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
        // �÷��̾� �� ����ȭ
        totalPlayers = PhotonNetwork.PlayerList.Length;
    }
}
