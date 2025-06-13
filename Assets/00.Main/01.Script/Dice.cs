using System.Collections;
using UnityEngine;

public class Dice : MonoBehaviour
{
    public int diceValue { get; private set; } // �ֻ��� �� (1~6)

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
        // **ȸ�� �ʱ�ȭ**
        transform.rotation = Quaternion.identity;

        // **�ֻ��� ���� ���� ȸ��**
        switch (value)
        {
            case 1: transform.Rotate(-90, 0, 0); break;   // 1�� �� (����)
            case 2: transform.Rotate(0, 0, 0); break;    // 2�� �� (����)
            case 3: transform.Rotate(0, 0, -90); break;  // 3�� �� (������)
            case 4: transform.Rotate(0, 0, 90); break;   // 4�� �� (����)
            case 5: transform.Rotate(180, 0, 0); break;  // **5�� �� (�ٴ�)**
            case 6: transform.Rotate(90, 0, 0); break;   // **6�� �� (�޸�)**
        }

        Debug.DrawRay(transform.position, transform.up * 2, Color.red, 2f); // ����׿�
    }

}
