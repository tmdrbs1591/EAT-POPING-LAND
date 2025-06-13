using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // 꼭 추가!

public class SettingPanel : MonoBehaviour
{
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private float moveDuration = 0.5f; // 애니메이션 시간
    [SerializeField] private float moveDistance = 500f; // 왼쪽으로 이동할 거리

    public bool isPanelOpen { get; private set; }
    private Vector3 originalPosition;

    public UIManager uiManager;
    void Start()
    {
        originalPosition = settingPanel.transform.localPosition;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && uiManager.uiStack.Count == 0)
        {
            TogglePanel();
        }
    }

    public void TogglePanel()
    {
        if (isPanelOpen)
        {
            // 닫기
            settingPanel.transform.DOLocalMove(originalPosition, moveDuration);
            isPanelOpen = false;
        }
        else
        {
            // 열기 (왼쪽으로 moveDistance만큼 이동)
            Vector3 targetPosition = originalPosition + new Vector3(-moveDistance, 0, 0);
            settingPanel.transform.DOLocalMove(targetPosition, moveDuration);
            isPanelOpen = true;
        }
    }
}
