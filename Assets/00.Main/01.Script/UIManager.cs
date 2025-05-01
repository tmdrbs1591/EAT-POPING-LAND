using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private Stack<GameObject> uiStack = new Stack<GameObject>();

    [Header("ĳ���� ���� ��ư")]
    [SerializeField] Button redButton;
    [SerializeField] Button blueButton;
    [SerializeField] Button yellowButton;
    [SerializeField] Button blackButton;

    [Header("Data")]
    [SerializeField] CharacterInfoData redCharacterInfoData;
    [SerializeField] CharacterInfoData yellowCharacterInfoData;
    [SerializeField] CharacterInfoData blueCharacterInfoData;
    [SerializeField] CharacterInfoData blackCharacterInfoData;

    [Header("UI")]
    [SerializeField] TMP_Text chracterNameText;
    [SerializeField] TMP_Text chracterInfoText;
    [SerializeField] TMP_Text chracteristiclText;
    [SerializeField] Slider moveSpeedSldier;
    [SerializeField] Slider hpSldier;

    private Coroutine hpCoroutine;
    private Coroutine moveSpeedCoroutine;

    private List<Button> buttons = new List<Button>();
    private int currentIndex = 0;

    private Vector3 buttonSpacing = new Vector3(180f, 0, 0); // ��ư �� ����

    void Awake()
    {

        buttons.Add(redButton);
        buttons.Add(blueButton);
        buttons.Add(yellowButton);
        buttons.Add(blackButton);
    }

    void Start()
    {
        UpdateButtonPositions();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseTopUI();
        }
        if (CharacterManager.instance.isCharSelect)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                currentIndex = Mathf.Clamp(currentIndex + 1, 0, buttons.Count - 1);
                UpdateButtonPositions();
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                currentIndex = Mathf.Clamp(currentIndex - 1, 0, buttons.Count - 1);
                UpdateButtonPositions();
            }
        }
    }

    void UpdateButtonPositions()
    {
        AudioManager.instance.PlaySound(transform.position, 3, Random.Range(1f, 1f), 1f);
        for (int i = 0; i < buttons.Count; i++)
        {
            int relativePos = i - currentIndex;

            // ��ġ �̵�
            Vector3 targetPos = new Vector3(relativePos * buttonSpacing.x, 0, 0);
            buttons[i].transform.DOLocalMove(targetPos, 0.3f).SetEase(Ease.OutQuad);

            // ������ ����
            float targetScale = 0.8f;
            if (Mathf.Abs(relativePos) == 0)
                targetScale = 1f;
            else if (Mathf.Abs(relativePos) == 1)
                targetScale = 0.9f;

            buttons[i].transform.DOScale(Vector3.one * targetScale, 0.3f).SetEase(Ease.OutQuad);
        }

        // currentIndex �������� ĳ���� Ÿ�� ����
        CharacterManager.instance.characterType = (CharacterType)currentIndex;

        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        hash["CharacterType"] = (int)CharacterManager.instance.characterType; // enum�� int�� ��ȯ�ؼ� ����
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        Debug.Log("���õ� ĳ���� Ÿ��: " + CharacterManager.instance.characterType);


        switch (CharacterManager.instance.characterType)
        {
            case CharacterType.Red:
                InfoUIUpdate(redCharacterInfoData);
                break;
            case CharacterType.Yellow:
                InfoUIUpdate(yellowCharacterInfoData);
                break;
            case CharacterType.Blue:
                InfoUIUpdate(blueCharacterInfoData);
                break;
            case CharacterType.Black:
                InfoUIUpdate(blackCharacterInfoData);
                break;
        }

    }
    public void InfoUIUpdate(CharacterInfoData cid)
    {
        chracterNameText.text = cid.characterName;
        chracterInfoText.text = cid.characterInfo;
        chracteristiclText.text = cid.characteristic;

        if (hpCoroutine != null) StopCoroutine(hpCoroutine);
        if (moveSpeedCoroutine != null) StopCoroutine(moveSpeedCoroutine);

        hpCoroutine = StartCoroutine(SmoothSliderUpdate(hpSldier, cid.hp / 10f));
        moveSpeedCoroutine = StartCoroutine(SmoothSliderUpdate(moveSpeedSldier, cid.moveSpeed / 10f));
    }

    private IEnumerator SmoothSliderUpdate(Slider slider, float targetValue)
    {
        float duration = 0.1f;
        float elapsed = 0f;
        float startValue = slider.value;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            slider.value = Mathf.Lerp(startValue, targetValue, elapsed / duration);
            yield return null;
        }

        slider.value = targetValue;
    }

    // UI ������Ʈ�� ���ÿ� �߰��ϸ鼭 Ȱ��ȭ
    public void OpenUI(GameObject uiObject)
    {
        uiObject.SetActive(true);
        uiStack.Push(uiObject);
    }

    // ������ �ֻ�� UI�� ��Ȱ��ȭ�ϰ� ����
    public void CloseTopUI()
    {
        if (uiStack.Count > 0)
        {
            GameObject topUI = uiStack.Pop();
            topUI.SetActive(false);
        }
    }

    // ��ü �ݱ� (���û���)
    public void CloseAllUI()
    {
        while (uiStack.Count > 0)
        {
            GameObject ui = uiStack.Pop();
            ui.SetActive(false);
        }
    }
}
