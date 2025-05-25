using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;

public class DiceManager : MonoBehaviourPunCallbacks
{
    public static DiceManager instance;

    public GameObject DiceCamera;
    public Dice dice1;
    public Dice dice2;

    public int diceResult;

    [SerializeField] GameObject dicePanel;
    public TMP_Text diceResultText;

    public bool isDice; // �ֻ����� ���ȴ°�

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        // �ڽ��� ���̰�, ���� �ֻ����� �� ��������, ���� ���� �ƴ� ��
        Player[] players = PhotonNetwork.PlayerList;

        if (Input.GetKeyDown(KeyCode.Space)
            && players[TurnManager.instance.currentPlayerIndex] == PhotonNetwork.LocalPlayer
            && !isDice
            && !BattleManager.instance.isBattle)
        {
            isDice = true;
            photonView.RPC(nameof(RPC_PlayDiceSound), RpcTarget.All);
            TurnManager.instance.diceUI.SetActive(false);

            // �ֻ��� �� ���� �� ��� Ŭ���̾�Ʈ���� ����
            int diceValue1 = Random.Range(1, 7);
            int diceValue2 = Random.Range(1, 7);
            photonView.RPC(nameof(RPC_SyncDice), RpcTarget.All, diceValue1, diceValue2);
        }
    }

    [PunRPC]
    void RPC_SyncDice(int value1, int value2)
    {
        diceResult = value1 + value2;

        dice1.SetValue(value1);
        dice2.SetValue(value2);

        StartCoroutine(dice1.RollDiceAnimation());
        StartCoroutine(dice2.RollDiceAnimation());

        StartCoroutine(ResultTextCor());

        Debug.Log($"[RPC] �ֻ���1: {value1}, �ֻ���2: {value2}, �հ�: {diceResult}");
    }

    [PunRPC]
    void RPC_PlayDiceSound()
    {
        AudioManager.instance.PlaySound(transform.position, 0, Random.Range(1f, 1.1f), 1f);
    }

    IEnumerator ResultTextCor()
    {
        DiceCamera.SetActive(true);
        dicePanel.SetActive(true);

        diceResultText.text = diceResult.ToString();
        diceResultText.gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);

        diceResultText.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);

        dicePanel.SetActive(false);
        DiceCamera.SetActive(false);
    }
}
