using UnityEngine;

public class LockRotationToIdentity : MonoBehaviour
{
    private Quaternion initialRotation;

    void Awake()
    {
        initialRotation = Quaternion.identity; // ȸ�� �ʱⰪ ����
    }

    void LateUpdate()
    {
        transform.rotation = initialRotation;
    }
}
