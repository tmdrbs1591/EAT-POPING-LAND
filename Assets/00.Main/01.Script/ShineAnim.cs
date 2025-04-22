using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ShineAnim : MonoBehaviour
{
    public Color color;
    public Renderer[] images;
    private float temp = 0; // 클래스 레벨에서 temp 변수를 정의

    public float restartTime = 0.2f;

    private void OnEnable()
    {
        StartCoroutine(DoShine());
    }

    private void OnDisable()
    {
        temp = 0; // 코루틴이 중지될 때 temp 변수를 0으로 초기화
        for (int i = 0; i < images.Length; i++)
            images[i].material.SetFloat("_ShineLocation", temp);
    }

    public float shineSpeed = 0.5f; // 이걸 0.2로 하면 더 느려지고, 1.0 이상이면 빨라짐

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