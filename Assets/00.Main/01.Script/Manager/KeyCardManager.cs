using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyCardManager : MonoBehaviour
{
    public static KeyCardManager instance;

    [SerializeField] private GameObject keyCardPanel;

    [SerializeField] private KeyCardData keyCardData; // ScriptableObject�� �ִ� KeyCardData
    [SerializeField] private List<KeyCardDataList> keyCardLists = new List<KeyCardDataList>(); // �������� ���õ� KeyCard�� ����

    public KeyCardType keyCardType;
    public TMP_Text  keyCardName;
    public TMP_Text keyCardInfo;
    public Image keyCardImage; // Image -> Sprite�� ����

    [Header("�̺�Ʈ��")]
   public KeyCard_UFO keycardUFO;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // KeyCardData�� �ִ� keyCards ����Ʈ���� �������� 10�� ����
        AddRandomKeyCards();
    }

    public void KeyCardPanelOpen()
    {
        if (keyCardLists.Count == 0)
        {
            Debug.Log("ī�� ����Ʈ�� ��� �ֽ��ϴ�.");
            return;
        }

        // ���� ī�� ���� UI�� ǥ��
        keyCardType = keyCardLists[0].keyCardType;
        keyCardName.text = keyCardLists[0].keyCardName;
        keyCardInfo.text = keyCardLists[0].keyCardInfo;
        keyCardImage.sprite = keyCardLists[0].keyCardImage;

        keyCardPanel.SetActive(true);

        // 0��° ī�� ����
        keyCardLists.RemoveAt(0);

        // ���ο� ī�� �������� �ϳ� �̾Ƽ� �߰� 
        List<KeyCardDataList> allKeyCards = new List<KeyCardDataList>(keyCardData.keyCards);
        System.Random rand = new System.Random();
        int randomIndex = rand.Next(allKeyCards.Count);
        keyCardLists.Add(allKeyCards[randomIndex]);
    }


    // KeyCardData�� keyCards���� �������� 10�� �����Ͽ� keyCardLists�� �߰��ϴ� �Լ�
    private void AddRandomKeyCards()
    {
        List<KeyCardDataList> allKeyCards = new List<KeyCardDataList>(keyCardData.keyCards);

        // �������� 10���� ����
        List<KeyCardDataList> randomKeyCards = new List<KeyCardDataList>();
        System.Random rand = new System.Random();

        while (randomKeyCards.Count < 10 && allKeyCards.Count > 0)
        {
            int randomIndex = rand.Next(allKeyCards.Count);
            randomKeyCards.Add(allKeyCards[randomIndex]);
        }

        keyCardLists = randomKeyCards; // ���������� ���õ� 10���� ī�带 keyCardLists�� ����
    }


    public void KeyEventStart()
    {
        switch (keyCardType)
        {
            case KeyCardType.UFO:
                keycardUFO.EventStart();
                break;
            case KeyCardType.ShopOpen:
                WeaponManager.instance.ShopPanelOpen(); 
                break;
            case KeyCardType.HoldLevelUp:
                break;
            case KeyCardType.WeaponUpgrade:
                break;
            case KeyCardType.Prison:

                break;



        }
    }
}
