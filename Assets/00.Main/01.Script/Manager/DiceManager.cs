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

    public bool isDice; // 주사위를 굴렸는가

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        // 자신의 턴이고, 아직 주사위를 안 굴렸으며, 전투 중이 아닐 때
        Player[] players = PhotonNetwork.PlayerList;

        if (Input.GetKeyDown(KeyCode.Space)
            && players[TurnManager.instance.currentPlayerIndex] == PhotonNetwork.LocalPlayer
            && !isDice
            && !BattleManager.instance.isBattle)
        {
            isDice = true;
            photonView.RPC(nameof(RPC_PlayDiceSound), RpcTarget.All);
            TurnManager.instance.diceUI.SetActive(false);

            // 주사위 값 생성 및 모든 클라이언트에게 전송
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

        Debug.Log($"[RPC] 주사위1: {value1}, 주사위2: {value2}, 합계: {diceResult}");
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
