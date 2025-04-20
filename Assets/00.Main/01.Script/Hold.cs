using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Hold : MonoBehaviour
{
    Renderer rend;

    public PlayerColorType holdType = PlayerColorType.Default;

    public Material[] materialList; // 인스펙터에서 등록

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();    
        holdType = PlayerColorType.Default;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    public void HoldColorChange(int materialIndex, int holdTypeInt)
    {
        if (materialIndex >= 0 && materialIndex < materialList.Length)
        {
            rend.material = materialList[materialIndex];
        }
        holdType = (PlayerColorType)holdTypeInt;
    }

}
