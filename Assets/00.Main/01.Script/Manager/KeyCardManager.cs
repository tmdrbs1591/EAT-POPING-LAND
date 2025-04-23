using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyCardManager : MonoBehaviour
{
    public static KeyCardManager instance;

    [SerializeField] private GameObject keyCardPanel;
    private void Awake()
    {
        instance = this;
    }

    public void KeyCardPanelOpenColose(bool tf)
    {
        keyCardPanel.SetActive(tf);
    }
}
