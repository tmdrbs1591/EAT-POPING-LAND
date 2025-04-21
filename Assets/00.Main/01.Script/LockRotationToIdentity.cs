using UnityEngine;

public class LockRotationToIdentity : MonoBehaviour
{
    private Quaternion initialRotation;

    void Awake()
    {
        initialRotation = Quaternion.identity; // 회전 초기값 고정
    }

    void LateUpdate()
    {
        transform.rotation = initialRotation;
    }
}
