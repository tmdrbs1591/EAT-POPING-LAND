using UnityEngine;
using BackEnd; // 뒤끝SDK

public class BackendManager : MonoBehaviour
{

    private void Awake()
    {
        BackendSetup();
    }
    private void BackendSetup()
    {
        var bro = Backend.Initialize();

        if (bro.IsSuccess())
        {
            Debug.Log($"초기화 성공{bro}");
        }
        else
        {
            Debug.LogError("초기화ㅣ실패");
        }
    }
    
}
