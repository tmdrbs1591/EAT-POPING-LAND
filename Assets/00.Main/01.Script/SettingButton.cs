using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class SettingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TMP_Text buttonText; // �ؽ�Ʈ
    [SerializeField] private Color normalColor = Color.white; // �⺻ ��
    [SerializeField] private Color hoverColor = Color.yellow; // ȣ�� ��
    [SerializeField] private GameObject backgroundObject; // ��� ������Ʈ

    // ���콺 �÷��� ��
    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.color = hoverColor;
        backgroundObject.SetActive(true);
    }

    // ���콺 ���� ��
    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.color = normalColor;
        backgroundObject.SetActive(false);
    }
    public void GameExit()
    {
        Application.Quit();
    }
}
