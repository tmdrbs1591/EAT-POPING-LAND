using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopPanelButton : MonoBehaviour
{
    [System.Serializable]
    public class ButtonPanelPair
    {
        public Button button;               // 버튼
        public GameObject panel;            // 열릴 패널
        public Sprite iconSprite;           // 기본 아이콘
        public Sprite clickSprite;          // 클릭 아이콘
    }

    [SerializeField] private List<ButtonPanelPair> pairs;

    private void Start()
    {
        foreach (var pair in pairs)
        {
            pair.button.onClick.AddListener(() => OnButtonClick(pair));
        }

        // 초기 상태: 첫 번째만 활성화
        if (pairs.Count > 0)
            OnButtonClick(pairs[0]);
    }

    private void OnButtonClick(ButtonPanelPair selectedPair)
    {
        foreach (var pair in pairs)
        {
            bool isSelected = pair == selectedPair;

            // 스프라이트 교체
            pair.button.GetComponent<Image>().sprite = isSelected ? pair.clickSprite : pair.iconSprite;

            // 패널 활성화 여부 설정
            pair.panel.SetActive(isSelected);
        }
    }
}
