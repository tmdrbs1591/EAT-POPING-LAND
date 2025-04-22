using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ColorType
{
    Red,
    Green,
    Blue,
    Yellow,
    Default,
    Key,
    Shop,
    Money,
    Prison

}

public class Hold : MonoBehaviour
{
    public Renderer rend;


    public ColorType holdType = ColorType.Default;
    public int holdPrice;
    public int level; // 단계
    public string holdPlayerName; // 땅 주인 이름


    public Material[] materialList; // 인스펙터에서 등록

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
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
        holdType = (ColorType)holdTypeInt;
    }

    [PunRPC]
    public void HoldPriceUp(int price,string playerName) // 땅 업그레이드
    {
        holdPrice += price;
        level++;
        holdPlayerName = playerName;    
       Instantiate(HoldInfoManager.instance.upgradeEffect,transform.position + new Vector3(0,5,0),Quaternion.identity); ;
        AudioManager.instance.PlaySound(transform.position, 7, Random.Range(1f, 1f), 1f);
        Debug.Log("레벨업!");
    }
}
