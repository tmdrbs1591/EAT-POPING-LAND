using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    public static DiceManager instance;

    public Dice dice1;
    public Dice dice2;

    public int diceResult;

    [SerializeField] GameObject dicePanel;
    public TMP_Text diceResultText;
    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
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
        dicePanel.SetActive(true);
        diceResultText.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        diceResultText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        dicePanel.SetActive(false);

    }
}
