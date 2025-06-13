using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class SettingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TMP_Text buttonText; // 텍스트
    [SerializeField] private Color normalColor = Color.white; // 기본 색
    [SerializeField] private Color hoverColor = Color.yellow; // 호버 색
    [SerializeField] private GameObject backgroundObject; // 배경 오브젝트

    // 마우스 올렸을 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.color = hoverColor;
        backgroundObject.SetActive(true);
    }

    // 마우스 뗐을 때
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
