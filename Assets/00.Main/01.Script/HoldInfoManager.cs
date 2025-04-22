using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HoldInfoManager : MonoBehaviour
{
    public static HoldInfoManager instance;

    public List<GameObject> holdInfoImage;

    [SerializeField] GameObject infoPanel;
    [SerializeField] GameObject infoVolume;
    [SerializeField] TMP_Text priceText;

    [SerializeField] Camera mainCam;
    [SerializeField] RectTransform infoPanelRect; // infoPanel�� RectTransform
    float originalCamSize;
    float zoomOutSize; // Ȯ��� ũ��

    Vector2 infoPanelOriginalPos;
    Vector2 infoPanelHiddenPos;

    private void Awake()
    {
        instance = this;
        originalCamSize = mainCam.orthographicSize;
        zoomOutSize = originalCamSize + 1f; // Ȯ��� ũ��

        infoPanelOriginalPos = infoPanelRect.anchoredPosition; // ���� ��ġ
        infoPanelHiddenPos = infoPanelOriginalPos + new Vector2(400f, 0f); // ���������� ���ܳ��� ��ġ (400f�� UI ũ�⿡ �°� ����)
        infoPanelRect.anchoredPosition = infoPanelHiddenPos; // ������ �� ���������� ���ܳ���
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            HoldInfoIconOpen();
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            HoldInfoIconClose();
        }
    }

    void HoldInfoIconOpen()
    {
        foreach (GameObject img in holdInfoImage)
        {
            img.SetActive(true);
        }

        mainCam.DOOrthoSize(zoomOutSize, 0.5f);

        infoVolume.SetActive(true);
        int playerLayerMask = 1 << LayerMask.NameToLayer("Player");
        mainCam.cullingMask &= ~playerLayerMask;
    }

    void HoldInfoIconClose()
    {
        foreach (GameObject img in holdInfoImage)
        {
            img.SetActive(false);
        }
        HoldInfoClose();
        mainCam.DOOrthoSize(originalCamSize, 0.5f);

        infoVolume.SetActive(false);
        int playerLayerMask = 1 << LayerMask.NameToLayer("Player");
        mainCam.cullingMask |= playerLayerMask;
    }

    public void HoldInfoOpen(int prices)
    {
        priceText.text = "���� : " + FormatKoreanCurrency(prices);
        infoPanel.SetActive(true);
        infoPanelRect.DOAnchorPos(infoPanelOriginalPos, 0.4f).SetEase(Ease.OutExpo); // �����ʿ��� �������� �����̵� ��
    }

    private string FormatKoreanCurrency(int money)
    {
        if (money < 10000)
        {
            return money.ToString("N0") + "��";
        }

        int man = money / 10000;
        int rest = money % 10000;

        if (rest == 0)
        {
            return $"{man}����";
        }
        else
        {
            return $"{man}��{rest.ToString("N0")}��";
        }
    }

    public void HoldInfoClose()
    {
        // ���������� �����̵� �ƿ��� �� SetActive(false)
        infoPanelRect.DOAnchorPos(infoPanelHiddenPos, 0.4f).SetEase(Ease.InExpo).OnComplete(() =>
        {
            infoPanel.SetActive(false);
        });
    }
}
