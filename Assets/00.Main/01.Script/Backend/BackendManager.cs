using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

public class BackendManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        BackendSetup();
    }
    private void Update()
    {
        if (Backend.IsInitialized) {
            Backend.AsyncPoll();
        }
    }
    private void BackendSetup()
    {
        var bro = Backend.Initialize(true);

        if (bro.IsSuccess())
        {
            Debug.Log($"초기화 성공 : {bro}");

        }
        else
        {
            Debug.Log($"초기화 실패 : {bro}" );
        }
    }
}
