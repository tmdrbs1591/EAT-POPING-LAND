using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrownManager : MonoBehaviour
{
    public GameObject crownPanel;

    public Animator anim;

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

}
