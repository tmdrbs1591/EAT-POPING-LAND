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
        priceText.text = "땅값 : " + FormatKoreanCurrency(prices);
        infoPanel.SetActive(true);
    }
    private string FormatKoreanCurrency(int money)
    {
        if (money < 10000)
        {
            return money.ToString("N0") + "원"; // 천 단위 쉼표
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
        infoPanel.SetActive(false);
    }
}
