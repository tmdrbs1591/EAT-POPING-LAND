using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;

public class DiceManager : MonoBehaviour
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
        Player[] players = PhotonNetwork.PlayerList;

        if (Input.GetKeyDown(KeyCode.Space)
            && players[TurnManager.instance.currentPlayerIndex] == PhotonNetwork.LocalPlayer
            && !isDice
            && !BattleManager.instance.isBattle)
        {
            AudioManager.instance.PlaySound(transform.position, 0, Random.Range(1f, 1.1f), 1); // 오디오 재생

            TurnManager.instance.diceUI.SetActive(false);
            isDice = true;
            RollDice();
            StartCoroutine(ResultTextCor());
        }

        diceResultText.text = diceResult.ToString();
    }


    void RollDice()
    {
        int diceValue1 = Random.Range(1, 7);
        int diceValue2 = Random.Range(1, 7);

        diceResult = diceValue1 + diceValue2;

        dice1.SetValue(diceValue1);
        dice2.SetValue(diceValue2);

        StartCoroutine(dice1.RollDiceAnimation());
        StartCoroutine(dice2.RollDiceAnimation());

        Debug.Log($"주사위1: {diceValue1}, 주사위2: {diceValue2}, 합계: {diceResult}");
    }

    IEnumerator ResultTextCor()
    {
        DiceCamera.SetActive(true);
        dicePanel.SetActive(true);
        diceResultText.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        diceResultText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        dicePanel.SetActive(false);
        DiceCamera.SetActive(false);


    }
}
