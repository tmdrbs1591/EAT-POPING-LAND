using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    public static DiceManager instance;

    private void Awake()
    {
        instance = this;
    }
    public int diceResult;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RollDice();
        }
    }

    void RollDice()
    {
        int dice1 = Random.Range(1, 7); // 주사위 1: 1~6 사이의 랜덤 값
        int dice2 = Random.Range(1, 7); // 주사위 2: 1~6 사이의 랜덤 값

        int total = dice1 + dice2;
        diceResult = total;

        Debug.Log($"주사위1: {dice1}, 주사위2: {dice2}, 합계: {total}");

    }
}
