using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using DG.Tweening;
using Unity.VisualScripting;

public class KeyCard_HoldUpgrade : MonoBehaviour, KeyCardEvents
{

    [SerializeField] List<GameObject> holdMoveButtons;

    [SerializeField] GameObject panel;

    [SerializeField] Camera mainCam;
    float originalCamSize;
    float zoomOutSize; // 확대될 크기

    private void Awake()
    {
        originalCamSize = mainCam.orthographicSize;
        zoomOutSize = originalCamSize + 1f; // 확대될 크기
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            UIOpen();
        }
        else if (Input.GetKeyUp(KeyCode.T))
        {
            UIclose();
        }
    }
    public void EventEnd()
    {
    }
    public void EventStart()
    {
        UIOpen();
    }
    public void UIOpen()
    {
        mainCam.DOOrthoSize(zoomOutSize, 0.5f);
        panel.SetActive(true);
        foreach (GameObject img in holdMoveButtons)
        {
                img.SetActive(true);
        }
    }
    public void UIclose()
    {
        mainCam.DOOrthoSize(originalCamSize, 0.5f);
        panel.SetActive(false);
        foreach (GameObject img in holdMoveButtons)
        {
            img.SetActive(false);
        }
    }
}
