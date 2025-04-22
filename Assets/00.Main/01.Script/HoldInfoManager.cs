using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HoldInfoManager : MonoBehaviour
{
    public static HoldInfoManager instance;

    public List<GameObject> holdInfoImage;

    [SerializeField] GameObject infoPanel;

    [SerializeField] TMP_Text priceText;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)){
            HoldInfoIconOpen();
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            HoldInfoIconClose();
        }
    }

    void HoldInfoIconOpen()
    {
        for(int i = 0; i < holdInfoImage.Count; i++)
        {
            holdInfoImage[i].SetActive(true);
        }
    }

    void HoldInfoIconClose()
    {
        for (int i = 0; i < holdInfoImage.Count; i++)
        {
            holdInfoImage[i].SetActive(false);
        }
    }


    public void HoldInfoOpen(int prices)
    {
        priceText.text = "���� : " + FormatKoreanCurrency(prices);
        infoPanel.SetActive(true);
    }
    private string FormatKoreanCurrency(int money)
    {
        if (money < 10000)
        {
            return money.ToString("N0") + "��"; // õ ���� ��ǥ
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
        infoPanel.SetActive(false);
    }
}
