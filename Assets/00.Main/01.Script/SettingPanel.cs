using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // �� �߰�!

public class SettingPanel : MonoBehaviour
{
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private float moveDuration = 0.5f; // �ִϸ��̼� �ð�
    [SerializeField] private float moveDistance = 500f; // �������� �̵��� �Ÿ�

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
            // �ݱ�
            settingPanel.transform.DOLocalMove(originalPosition, moveDuration);
            isPanelOpen = false;
        }
        else
        {
            // ���� (�������� moveDistance��ŭ �̵�)
            Vector3 targetPosition = originalPosition + new Vector3(-moveDistance, 0, 0);
            settingPanel.transform.DOLocalMove(targetPosition, moveDuration);
            isPanelOpen = true;
        }
    }
}
