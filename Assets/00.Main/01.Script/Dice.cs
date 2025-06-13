using System.Collections;
using UnityEngine;

public class Dice : MonoBehaviour
{
    public int diceValue { get; private set; } // 주사위 값 (1~6)

    public void SetValue(int value)
    {
        diceValue = value;
    }

    public IEnumerator RollDiceAnimation()
    {
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.Rotate(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            elapsed += Time.deltaTime;
            yield return null;
        }

        SetDiceFace(diceValue);
    }
    void SetDiceFace(int value)
    {
        // **회전 초기화**
        transform.rotation = Quaternion.identity;

        // **주사위 값에 따라 회전**
        switch (value)
        {
            case 1: transform.Rotate(-90, 0, 0); break;   // 1번 면 (윗면)
            case 2: transform.Rotate(0, 0, 0); break;    // 2번 면 (정면)
            case 3: transform.Rotate(0, 0, -90); break;  // 3번 면 (오른쪽)
            case 4: transform.Rotate(0, 0, 90); break;   // 4번 면 (왼쪽)
            case 5: transform.Rotate(180, 0, 0); break;  // **5번 면 (바닥)**
            case 6: transform.Rotate(90, 0, 0); break;   // **6번 면 (뒷면)**
        }

        Debug.DrawRay(transform.position, transform.up * 2, Color.red, 2f); // 디버그용
    }

}
