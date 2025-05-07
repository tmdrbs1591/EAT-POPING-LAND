using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class TurnManager : MonoBehaviourPunCallbacks
{
    public static TurnManager instance;

    public int currentPlayerIndex = 0; // ���� ���� �÷��̾� �ε���
    public int totalPlayers; // ��ü �÷��̾� ��
    public int countdown = 15;

    [SerializeField] GameObject myTurnPanel;
    [SerializeField] TMP_Text countdownText; // ī��Ʈ�ٿ� ǥ�� UI

    public bool isCountingDown = false; // ī��Ʈ�ٿ� ������ Ȯ��
    private bool isFirstTurn = true; // ù ������ Ȯ��
    public bool isPrison = true; // ��������Ȯ��

    public GameObject diceUI;
    public GameObject otherDiceUI;
    public GameObject prisonUI;
    private void Awake()
    {
        instance = this;
        isPrison = false;
    }

    private void Start()
    {
        totalPlayers = PhotonNetwork.PlayerList.Length; // �÷��̾� �� ��������
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
        isCountingDown = true; // ī��Ʈ�ٿ� ����
        DiceManager.instance.enabled = false; // �ֻ��� ��Ȱ��ȭ

   
        while (countdown > 0)
        {
            countdownText.text = $"{countdown}�� �ڿ� ������ �����մϴ�.";
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        countdownText.text = "�� ����!";
        yield return new WaitForSeconds(1f);
        if (PhotonNetwork.IsMasterClient)
        {
            SystemMessaageManager.instance.MessageTextStart("������ ���۵Ǿ����ϴ�!");
        }
        countdownText.text = "";

        isCountingDown = false; // ī��Ʈ�ٿ� ����
        photonView.RPC("StartTurn", RpcTarget.All, playerIndex);
    }

    [PunRPC]
    public void StartTurn(int playerIndex)
    {
        currentPlayerIndex = playerIndex;

        // �� ������ ��
        if (PhotonNetwork.LocalPlayer.ActorNumber == playerIndex + 1)
        {
            if (isPrison)
            {
                prisonUI.SetActive(false);
                prisonUI.SetActive(true);
                EndTurn();
                isPrison = false;
                return;
            }
            else
            {
                diceUI.SetActive(true);
                DiceManager.instance.enabled = true; // �ֻ��� Ȱ��ȭ
                myTurnPanel.SetActive(true);
                Debug.Log("���� ��!");
            }
           


        }
        else
        {
            DiceManager.instance.enabled = false;
            myTurnPanel.SetActive(false);
            otherDiceUI.SetActive(true);


        }
    }

    public void EndTurn()
    {

        int nextPlayerIndex = (currentPlayerIndex + 1) % totalPlayers;
        photonView.RPC("StartTurn", RpcTarget.All, nextPlayerIndex);

        Debug.Log("�� ��");
        DiceManager.instance.isDice = false;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        totalPlayers = PhotonNetwork.PlayerList.Length;
    }

}
