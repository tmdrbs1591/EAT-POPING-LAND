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

        while (elapsed < duration)
        {
            float CameraPosX = Random.value * intensity * 2 - intensity;
            float CameraPosY = Random.value * intensity * 2 - intensity;

            Vector3 newCameraPos = originalCameraPos;
            newCameraPos.x += CameraPosX;
            newCameraPos.y += CameraPosY;

            mainCamera.transform.position = newCameraPos;

            elapsed += Time.deltaTime;

            yield return null;
        }

        mainCamera.transform.position = originalCameraPos;
    }
}