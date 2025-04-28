using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Slider를 사용하기 위한 네임스페이스
using TMPro; // TextMeshPro를 사용하기 위한 네임스페이스

public class TitleSceneLoad : MonoBehaviour
{
    public string SceneName; // 로드할 씬 이름
    public TMP_Text loadingText; // TMP 텍스트 참조
    public Slider loadingBar; // 로딩 바 슬라이더 참조
    public float messageInterval = 1.0f; // 메시지가 변경되는 간격 (좀 더 빠르게)

    private string[] loadingMessages = new string[]
    {
        "리소스를 불러오는 중입니다...",
        "데이터 동기화 중...",
        "주사위 굴리는중..",
        "사탕 먹는중..",
    };

    private int currentMessageIndex = 0;
    private float timeSinceLastMessage = 0f;
    private float loadingProgress = 0f; // 로딩 진행 상태
    private bool loadingFinished = false; // 로딩 완료 여부

    private float fakeLoadingSpeed = 0.2f; // 기본 로딩 속도 (더 빠르게)
    private float slowdownTime = 0f; // 로딩이 느려지는 시간 계산

    void Start()
    {
        loadingText.text = loadingMessages[currentMessageIndex]; // 첫 번째 메시지를 설정
        loadingBar.value = 0f; // 슬라이더 초기화
    }

    void Update()
    {
        if (!loadingFinished) // 로딩이 완료되지 않았을 때만 진행
        {
            // 메시지를 일정 시간마다 순차적으로 전환
            timeSinceLastMessage += Time.deltaTime;
            if (timeSinceLastMessage >= messageInterval)
            {
                timeSinceLastMessage = 0f;
                currentMessageIndex = (currentMessageIndex + 1) % loadingMessages.Length; // 인덱스를 순환
                loadingText.text = loadingMessages[currentMessageIndex]; // 텍스트 업데이트
            }

            // 로딩 진행 상태 업데이트
            if (slowdownTime <= 0f)
            {
                // 로딩 속도 랜덤으로 빠르게 혹은 느리게 변화
                fakeLoadingSpeed = Random.Range(0.05f, 0.2f); // 더 빠른 로딩 속도
                slowdownTime = Random.Range(0.3f, 0.8f); // 속도가 유지될 시간 설정 (짧게)
            }
            else
            {
                slowdownTime -= Time.deltaTime;
            }

            // 로딩 진행 상태 업데이트 (빠르게 진행되도록)
            loadingProgress += fakeLoadingSpeed * Time.deltaTime;
            loadingBar.value = Mathf.Clamp01(loadingProgress); // 0~1 범위로 값 유지

            // 로딩이 완료되면 슬라이더와 텍스트를 업데이트
            if (loadingProgress >= 1f)
            {
                loadingFinished = true;
                loadingBar.gameObject.SetActive(false); // 슬라이더 숨기기
                loadingText.text = "-아무 키나 눌러주세요-"; // 로딩 완료 메시지 표시
            }
        }

        // 로딩이 완료된 후 키를 눌렀을 때 씬 로드
        if (loadingFinished && Input.anyKey)
        {
            LoadingManager.LoadScene(SceneName); // 씬 로드
        }
    }
}
