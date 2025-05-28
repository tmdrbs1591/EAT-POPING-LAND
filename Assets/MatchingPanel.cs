using System.Collections;
using TMPro;
using UnityEngine;

public class MatchingPanel : MonoBehaviour
{
    [SerializeField] TMP_Text countText;
    [SerializeField] TMP_Text playerMactchingText;

    private float countTimer = 0f;
    private int secondsPassed = 0;

    private float textChangeTimer = 0f;
    private float textChangeInterval = 0.6f; // 텍스트 변경 주기

    private bool isCounting = false;

    public GameObject matchingPanel;
    public GameObject matchingCanButton;

    private string[] matchingTexts = {
        "플레이어 찾는중.",
        "플레이어 찾는중..",
        "플레이어 찾는중..."
    };
    private int matchingTextIndex = 0;

    void OnEnable()
    {
        countTimer = 0f;
        secondsPassed = 0;

        textChangeTimer = 0f;
        matchingTextIndex = 0;
        playerMactchingText.text = matchingTexts[matchingTextIndex];

        UpdateCountText();
        isCounting = true;
        StartCoroutine(MatchingCanPanelDownCor());
    }

    void OnDisable()
    {
        isCounting = false;
    }

    void Update()
    {
        if (!isCounting) return;

        // 시간 카운트 (1초마다 증가)
        countTimer += Time.deltaTime;
        if (countTimer >= 1f)
        {
            countTimer -= 1f;
            secondsPassed++;
            UpdateCountText();
        }

        // 텍스트 변경 (0.3초마다)
        textChangeTimer += Time.deltaTime;
        if (textChangeTimer >= textChangeInterval)
        {
            textChangeTimer -= textChangeInterval;
            matchingTextIndex = (matchingTextIndex + 1) % matchingTexts.Length;
            playerMactchingText.text = matchingTexts[matchingTextIndex];
        }
    }

    void UpdateCountText()
    {
        int minutes = secondsPassed / 60;
        int seconds = secondsPassed % 60;
        countText.text = $"{minutes:00}:{seconds:00}";
    }

    public void MatchingPanelDown()
    {
        StartCoroutine(MatchingPanelDownCor());
    }
  public  IEnumerator MatchingPanelDownCor()
    {
        yield return new WaitForSeconds(2f);
        matchingPanel.SetActive(false);
    }
    public IEnumerator MatchingCanPanelDownCor()
    {
        yield return new WaitForSeconds(2f);
        matchingCanButton.SetActive(true);
    }
}
