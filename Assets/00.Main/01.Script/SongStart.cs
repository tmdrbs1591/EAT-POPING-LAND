using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongStart : MonoBehaviour
{

   public AudioSource audioSource;


    private void Start()
    {
    }
    public void SongStarts()
    {
        SongManager.instance.SongChange(2);
    }
    public void LodingSongStarts()
    {
        SongManager.instance.SongChange(4);
    }

}
