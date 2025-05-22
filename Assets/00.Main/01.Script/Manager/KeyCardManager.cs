using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyCardManager : MonoBehaviour
{
    public static KeyCardManager instance;

    [SerializeField] private GameObject keyCardPanel;

    [SerializeField] private KeyCardData keyCardData; // ScriptableObject에 있는 KeyCardData
    [SerializeField] private List<KeyCardDataList> keyCardLists = new List<KeyCardDataList>(); // 랜덤으로 선택된 KeyCard들 저장

    public KeyCardType keyCardType;
    public TMP_Text  keyCardName;
    public TMP_Text keyCardInfo;
    public Image keyCardImage; // Image -> Sprite로 변경

    [Header("이벤트들")]
   public KeyCard_UFO keycardUFO;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // KeyCardData에 있는 keyCards 리스트에서 랜덤으로 10개 선택
        AddRandomKeyCards();
    }

    public void KeyCardPanelOpen()
    {
        if (keyCardLists.Count == 0)
        {
            Debug.Log("카드 리스트가 비어 있습니다.");
            return;
        }

        // 현재 카드 정보 UI에 표시
        keyCardType = keyCardLists[0].keyCardType;
        keyCardName.text = keyCardLists[0].keyCardName;
        keyCardInfo.text = keyCardLists[0].keyCardInfo;
        keyCardImage.sprite = keyCardLists[0].keyCardImage;

        keyCardPanel.SetActive(true);

        // 0번째 카드 삭제
        keyCardLists.RemoveAt(0);

        // 새로운 카드 랜덤으로 하나 뽑아서 추가 
        List<KeyCardDataList> allKeyCards = new List<KeyCardDataList>(keyCardData.keyCards);
        System.Random rand = new System.Random();
        int randomIndex = rand.Next(allKeyCards.Count);
        keyCardLists.Add(allKeyCards[randomIndex]);
    }


    // KeyCardData의 keyCards에서 랜덤으로 10개 선택하여 keyCardLists에 추가하는 함수
    private void AddRandomKeyCards()
    {
        List<KeyCardDataList> allKeyCards = new List<KeyCardDataList>(keyCardData.keyCards);

        // 랜덤으로 10개를 선택
        List<KeyCardDataList> randomKeyCards = new List<KeyCardDataList>();
        System.Random rand = new System.Random();

        while (randomKeyCards.Count < 10 && allKeyCards.Count > 0)
        {
            int randomIndex = rand.Next(allKeyCards.Count);
            randomKeyCards.Add(allKeyCards[randomIndex]);
        }

        keyCardLists = randomKeyCards; // 최종적으로 선택된 10개의 카드를 keyCardLists에 저장
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
