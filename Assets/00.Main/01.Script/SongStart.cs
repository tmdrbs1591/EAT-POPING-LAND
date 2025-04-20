using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongStart : MonoBehaviour
{

   public AudioSource audioSource;


    private void Start()
    {
    }
    public void AudioStart()
    {
        audioSource.Play();
    }
}
