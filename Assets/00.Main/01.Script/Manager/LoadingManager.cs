using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    static string nextScene;// 다음 씬의 이름을 저장하는 변수
    [SerializeField]
    Image progressBar;// 로딩 진행 상황을 표시할 UI 이미지

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;// 다음 씬 이름 설정
        SceneManager.LoadScene("LoadingScene");// 다음 씬 이름 설정
    }

    void Start()
    {
        StartCoroutine(LoadSceneProgress()); // 씬 로딩 진행 상황을 관리하는 코루틴 시작
    }

    IEnumerator LoadSceneProgress()    // 씬 로딩 진행 상황을 관리하는 코루틴
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene); // 비동기 방식으로 씬을 로드하는 AsyncOperation 객체 생성
        op.allowSceneActivation = false;// 씬 활성화를 막음

        float timer = 0f; // 타이머 변수 초기화
        float fakeLoadTime = 2f; // 로딩 속도를 조절하기 위한 변수

        while (!op.isDone)// 씬 로드가 완료될 때까지 반복
        {
            yield return null; // 다음 프레임까지 대기

            if (op.progress < 0.9f)// 씬 로드 진행도가 90% 미만일 경우
            {
                progressBar.fillAmount = op.progress;// 진행 상황을 UI에 반영
            }
            else// 씬 로드 진행도가 90% 이상일 경우
            {
                timer += Time.unscaledDeltaTime / fakeLoadTime; // 타이머 증가 
                progressBar.fillAmount = Mathf.Lerp(0f, 1f, timer);// 진행 상황을 부드럽게 UI에 반영

                if (progressBar.fillAmount >= 1f)// 진행 상황이 100% 이상일 경우
                {
                    op.allowSceneActivation = true; // 씬 활성화 허용
                    yield break; // 코루틴 종료
                }
            }
        }
    }
}
