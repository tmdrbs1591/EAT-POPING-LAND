using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private Stack<GameObject> uiStack = new Stack<GameObject>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseTopUI();
        }
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


    [SerializeField] Button redButton;
    [SerializeField] Button blueButton;
    [SerializeField] Button yellowButton;
    [SerializeField] Button blackButton;

    private List<Button> buttons = new List<Button>();
    private int currentIndex = 0;

    private Vector3 buttonSpacing = new Vector3(180f, 0, 0); // 버튼 간 간격

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


    void UpdateButtonPositions()
    {
        AudioManager.instance.PlaySound(transform.position, 3, Random.Range(1f, 1f), 1f);
        for (int i = 0; i < buttons.Count; i++)
        {
            int relativePos = i - currentIndex;

            // 위치 이동
            Vector3 targetPos = new Vector3(relativePos * buttonSpacing.x, 0, 0);
            buttons[i].transform.DOLocalMove(targetPos, 0.3f).SetEase(Ease.OutQuad);

            // 스케일 변경
            float targetScale = 0.8f;
            if (Mathf.Abs(relativePos) == 0)
                targetScale = 1f;
            else if (Mathf.Abs(relativePos) == 1)
                targetScale = 0.9f;

            buttons[i].transform.DOScale(Vector3.one * targetScale, 0.3f).SetEase(Ease.OutQuad);
        }

        // currentIndex 기준으로 캐릭터 타입 설정
        CharacterManager.instance.characterType = (CharacterType)currentIndex;
        Debug.Log("선택된 캐릭터 타입: " + CharacterManager.instance.characterType);
    }


    // UI 오브젝트를 스택에 추가하면서 활성화
    public void OpenUI(GameObject uiObject)
    {
        uiObject.SetActive(true);
        uiStack.Push(uiObject);
    }

    // 스택의 최상단 UI를 비활성화하고 제거
    public void CloseTopUI()
    {
        if (uiStack.Count > 0)
        {
            GameObject topUI = uiStack.Pop();
            topUI.SetActive(false);
        }
    }

    // 전체 닫기 (선택사항)
    public void CloseAllUI()
    {
        while (uiStack.Count > 0)
        {
            GameObject ui = uiStack.Pop();
            ui.SetActive(false);
        }
    }
}
