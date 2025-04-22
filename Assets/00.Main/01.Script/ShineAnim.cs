using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ShineAnim : MonoBehaviour
{
    public Color color;
    public Renderer[] images;
    private float temp = 0; // Ŭ���� �������� temp ������ ����

    public float restartTime = 0.2f;

    private void OnEnable()
    {
        StartCoroutine(DoShine());
    }

    private void OnDisable()
    {
        temp = 0; // �ڷ�ƾ�� ������ �� temp ������ 0���� �ʱ�ȭ
        for (int i = 0; i < images.Length; i++)
            images[i].material.SetFloat("_ShineLocation", temp);
    }

    public float shineSpeed = 0.5f; // �̰� 0.2�� �ϸ� �� ��������, 1.0 �̻��̸� ������

    IEnumerator DoShine()
    {
        float startDelay = Random.Range(1f, 1.3f);
        yield return new WaitForSeconds(startDelay);
        while (true)
        {
            temp += Time.deltaTime * shineSpeed;

            if (temp > 1.0f)
            {
                temp = 0.0f;
                yield return new WaitForSeconds(restartTime);
            }

            for (int i = 0; i < images.Length; i++)
                images[i].material.SetFloat("_ShineLocation", temp);

            yield return null;
        }
    }

}