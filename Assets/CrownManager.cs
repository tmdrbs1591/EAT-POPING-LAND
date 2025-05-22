using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrownManager : MonoBehaviour
{
    public GameObject crownPanel;

    public Animator anim;

    public GameObject resultPanel;

    public bool isOpen;
    public void Open()
    {
        if (!isOpen)
        {
            anim.SetTrigger("Open");
            isOpen = true;
        }
        else
        {
            anim.SetTrigger("Out");
            isOpen = false;
        }
    }

    public void GameEnd()
    {
        PlayerMoney playerMoney = GameManager.instance.playerScript.gameObject.GetComponent<PlayerMoney>();

        if (playerMoney.money >= 10000)
        {
            playerMoney.AddMoney(-10000);
            resultPanel.SetActive(true);
        }
    }
}
