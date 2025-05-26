using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopPanelButton : MonoBehaviour
{
    [System.Serializable]
    public class ButtonPanelPair
    {
        public Button button;               // ��ư
        public GameObject panel;            // ���� �г�
        public Sprite iconSprite;           // �⺻ ������
        public Sprite clickSprite;          // Ŭ�� ������
    }

    [SerializeField] private List<ButtonPanelPair> pairs;

    private void Start()
    {
        foreach (var pair in pairs)
        {
            pair.button.onClick.AddListener(() => OnButtonClick(pair));
        }

        // �ʱ� ����: ù ��°�� Ȱ��ȭ
        if (pairs.Count > 0)
            OnButtonClick(pairs[0]);
    }

    private void OnButtonClick(ButtonPanelPair selectedPair)
    {
        foreach (var pair in pairs)
        {
            bool isSelected = pair == selectedPair;

            // ��������Ʈ ��ü
            pair.button.GetComponent<Image>().sprite = isSelected ? pair.clickSprite : pair.iconSprite;

            // �г� Ȱ��ȭ ���� ����
            pair.panel.SetActive(isSelected);
        }
    }
}
