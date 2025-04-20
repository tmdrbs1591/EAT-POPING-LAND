using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SystemMessaageManager : MonoBehaviour
{
    public static SystemMessaageManager instance;

    public GameObject systemMegapon;
    public TMP_Text systemText;

    public Animator megaponAnim;
    private void Awake()
    {
        instance = this;
    }

    public void MessageTextStart(string text)
    {
        StartCoroutine(MessageTextStartCor(text));
    }

    IEnumerator MessageTextStartCor(string text)
    {
        systemMegapon.SetActive(true);
        systemText.text = "";

        yield return new WaitForSeconds(1f);
        // �� ���ھ� ���
        for (int i = 0; i < text.Length; i++)
        {
            systemText.text += text[i];
            yield return new WaitForSeconds(0.05f); // �M�M�M ���� (���ϴ� �ӵ� ���� ����)
        }

        // ��ü �ؽ�Ʈ�� ��µ� �� 5�� ��ٸ��� ��
        yield return new WaitForSeconds(3.5f);
        megaponAnim.SetTrigger("End");
        yield return new WaitForSeconds(2f);
        systemMegapon.SetActive(false);
    }
}
