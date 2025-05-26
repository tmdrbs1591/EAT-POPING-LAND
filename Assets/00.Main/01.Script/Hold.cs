using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public enum ColorType
{
    Red,
    Green,
    Blue,
    Yellow,
    Default,
    Key,
    Shop,
    Money,
    Prison

}

public class Hold : MonoBehaviour
{
    public Renderer rend;


    public ColorType holdType = ColorType.Default;
    public int holdPrice;
    public int level; // �ܰ�
    public string holdPlayerName; // �� ���� �̸�


    public Material[] materialList; // �ν����Ϳ��� ���

    [SerializeField] private GameObject level1Effect;
    [SerializeField] private GameObject level2Effect;
    [SerializeField] private GameObject level3Effect;
    [SerializeField] private GameObject level4Effect;
    [SerializeField] private GameObject level5Effect;

    void Start()
    {
        rend = GetComponent<Renderer>();

        if (holdType == ColorType.Default)
        {
            // 2��° �ڽ�
            Transform secondChild = transform.GetChild(1); // �ε��� 1 = �� ��°
       
            level1Effect = secondChild.GetChild(0).gameObject;
            level2Effect = secondChild.GetChild(1).gameObject;
            level3Effect = secondChild.GetChild(2).gameObject;
            level4Effect = secondChild.GetChild(3).gameObject;
            level5Effect = secondChild.GetChild(4).gameObject;
        }
            // �� �ڽ��� 1~5��° �ڽ� �Ҵ�
         
    }

    // Update is called once per frame
    void Update()
    {

    }

    [PunRPC]
    public void HoldColorChange(int materialIndex, int holdTypeInt)
    {
        if (materialIndex >= 0 && materialIndex < materialList.Length)
        {
            rend.material = materialList[materialIndex];
        }
        holdType = (ColorType)holdTypeInt;
    }
    private void UpdateLevelEffect()
    {
        // ��� ����Ʈ ��Ȱ��ȭ
        level1Effect.SetActive(false);
        level2Effect.SetActive(false);
        level3Effect.SetActive(false);
        level4Effect.SetActive(false);
        level5Effect.SetActive(false);

        // ���� ������ �´� ����Ʈ�� Ȱ��ȭ
        switch (level)
        {
            case 1:
                break;
            case 2:
                level1Effect.SetActive(true);
                break;
            case 3:
                level2Effect.SetActive(true);
                break;
            case 4:
                level3Effect.SetActive(true);
                break;
            case 5:
                level4Effect.SetActive(true);
                break;
            case 6:
                level5Effect.SetActive(true);
                break;
        }
    }

    [PunRPC]
    public void HoldPriceUp(int price, string playerName)
    {
        if(level >= 6)
        {
            GameManager.instance.playerScript.playerColorBoxScript.AddHoldMoney();
            return;
        }
        if(level <= 0)
        {
            holdPrice += price;
        }
        else
        {
            holdPrice *= 2;
        }
        level++;
        holdPlayerName = playerName;

        UpdateLevelEffect(); // ����Ʈ ����

        Instantiate(HoldInfoManager.instance.upgradeEffect, transform.position + new Vector3(0, 5, 0), Quaternion.identity);
        AudioManager.instance.PlaySound(transform.position, 7, Random.Range(1f, 1f), 1f);
        Debug.Log("������!");
    }

    public void PlaySteppedAnimation()
    {
        // ó�� ��ġ
        Vector3 originalPos = transform.localPosition;

        // �������ٰ� �ö���� ������
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOLocalMoveY(originalPos.y - 1f, 0.1f).SetEase(Ease.OutQuad)); // �Ʒ���
        seq.Append(transform.DOLocalMoveY(originalPos.y, 0.13f).SetEase(Ease.InQuad));         // �ٽ� ����
    }

}
