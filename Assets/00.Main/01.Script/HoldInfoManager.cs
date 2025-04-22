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
    [SerializeField] RectTransform infoPanelRect; // infoPanel의 RectTransform
    float originalCamSize;
    float zoomOutSize; // 확대될 크기

    Vector2 infoPanelOriginalPos;
    Vector2 infoPanelHiddenPos;

    private void Awake()
    {
        instance = this;
        originalCamSize = mainCam.orthographicSize;
        zoomOutSize = originalCamSize + 1f; // 확대될 크기

        infoPanelOriginalPos = infoPanelRect.anchoredPosition; // 원래 위치
        infoPanelHiddenPos = infoPanelOriginalPos + new Vector2(400f, 0f); // 오른쪽으로 숨겨놓을 위치 (400f는 UI 크기에 맞게 조절)
        infoPanelRect.anchoredPosition = infoPanelHiddenPos; // 시작할 때 오른쪽으로 숨겨놓기
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
        priceText.text = "땅값 : " + FormatKoreanCurrency(prices);
        infoPanel.SetActive(true);
        infoPanelRect.DOAnchorPos(infoPanelOriginalPos, 0.4f).SetEase(Ease.OutExpo); // 오른쪽에서 왼쪽으로 슬라이드 인
    }

    private string FormatKoreanCurrency(int money)
    {
        if (money < 10000)
        {
            return money.ToString("N0") + "원";
        }

        int man = money / 10000;
        int rest = money % 10000;

        if (rest == 0)
        {
            return $"{man}만원";
        }
        else
        {
            return $"{man}만{rest.ToString("N0")}원";
        }
    }

    public void HoldInfoClose()
    {
        // 오른쪽으로 슬라이드 아웃한 후 SetActive(false)
        infoPanelRect.DOAnchorPos(infoPanelHiddenPos, 0.4f).SetEase(Ease.InExpo).OnComplete(() =>
        {
            infoPanel.SetActive(false);
        });
    }
}
