using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MenuCameraMove : MonoBehaviour
{
    [SerializeField] private Transform charSelectCameraPos;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    [SerializeField] private GameObject charSelectBtn;
    [SerializeField] private RectTransform charSelectPanel;
    [SerializeField] private RectTransform charImfoPanel;

    private Vector2 charSelectPanelOriginalPos;
    private Vector2 charImfoPanelOriginalPos;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        charSelectPanelOriginalPos = charSelectPanel.anchoredPosition;
        charImfoPanelOriginalPos = charImfoPanel.anchoredPosition;

        // 처음에 패널 화면 밖으로
        charSelectPanel.anchoredPosition = charSelectPanelOriginalPos + new Vector2(0, -Screen.height);
        charImfoPanel.anchoredPosition = charImfoPanelOriginalPos + new Vector2(-Screen.width, 0);

        charSelectPanel.gameObject.SetActive(false);
        charImfoPanel.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MoveToOriginalPos();
        }
    }

    public void MoveToSelectPos()
    {
        AudioManager.instance.PlaySound(transform.position, 4, 1f, 1f);

        transform.DOMove(charSelectCameraPos.position, 0.3f).SetEase(Ease.InOutQuad);
        transform.DORotate(charSelectCameraPos.rotation.eulerAngles, 0.3f).SetEase(Ease.InOutQuad);

        charSelectBtn.SetActive(false);

        // UI 슬라이드 인
        charSelectPanel.gameObject.SetActive(true);
        charSelectPanel.DOAnchorPos(charSelectPanelOriginalPos, 0.3f).SetEase(Ease.InOutQuad);

        charImfoPanel.gameObject.SetActive(true);
        charImfoPanel.DOAnchorPos(charImfoPanelOriginalPos, 0.3f).SetEase(Ease.InOutQuad);

        CharacterManager.instance.isCharSelect = true;
    }

    public void MoveToOriginalPos()
    {
        transform.DOMove(originalPosition, 0.3f).SetEase(Ease.InOutQuad);
        transform.DORotate(originalRotation.eulerAngles, 0.3f).SetEase(Ease.InOutQuad);

        charSelectBtn.SetActive(true);

        // UI 슬라이드 아웃
        charSelectPanel.DOAnchorPos(charSelectPanelOriginalPos + new Vector2(0, -Screen.height), 0.3f)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => charSelectPanel.gameObject.SetActive(false));

        charImfoPanel.DOAnchorPos(charImfoPanelOriginalPos + new Vector2(-Screen.width, 0), 0.3f)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => charImfoPanel.gameObject.SetActive(false));

        CharacterManager.instance.isCharSelect = false;
    }
}
