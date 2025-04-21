using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    [SerializeField]
    private Camera mainCamera;
    private Vector3 originalCameraPos;

    [SerializeField]
    [Range(0.1f, 0.5f)]
    private float defaultShakeRange = 0.5f;

    [SerializeField]
    [Range(0.1f, 1f)]
    private float defaultDuration = 0.1f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (mainCamera != null)
        {
            originalCameraPos = mainCamera.transform.position;
        }
        else
        {
            Debug.LogError("Main camera not found. Please tag the main camera as 'MainCamera'.");
        }
    }

    // 기존 기본값으로 호출
    public void Shake()
    {
        Shake(defaultShakeRange, defaultDuration);
    }

    // 강도와 지속시간을 매개변수로 받는 Shake
    public void Shake(float intensity, float duration)
    {
        if (mainCamera != null)
        {
            StopAllCoroutines();
            StartCoroutine(ShakeCoroutine(intensity, duration));
        }
        else
        {
            Debug.LogError("Main camera is not assigned.");
        }
    }

    private IEnumerator ShakeCoroutine(float intensity, float duration)
    {
        float elapsed = 0.0f;
        Vector3 startPos = mainCamera.transform.position; // 흔들기 직전의 카메라 위치

        while (elapsed < duration)
        {
            float offsetX = Random.value * intensity * 2 - intensity;
            float offsetY = Random.value * intensity * 2 - intensity;

            Vector3 shakePos = startPos + new Vector3(offsetX, offsetY, 0);
            mainCamera.transform.position = shakePos;

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = startPos;
    }

}