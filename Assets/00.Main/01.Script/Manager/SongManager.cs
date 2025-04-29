using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongManager : MonoBehaviour
{
    public static SongManager instance;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip titleSong;
    [SerializeField] AudioClip menuSong;
    [SerializeField] AudioClip ingameSong;
    [SerializeField] AudioClip battleSong;
    [SerializeField] AudioClip loadingSong;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        SongChange(0);
    }
    public void SongChange(int index)
    {
        if (audioSource == null) return;

        switch (index)
        {
            case 0:
                audioSource.clip = titleSong;
                break;
            case 1:
                audioSource.clip = menuSong;
                break;
            case 2:
                audioSource.clip = ingameSong;
                break;
            case 3:
                audioSource.clip = battleSong;
                break;
            case 4:
                audioSource.clip = loadingSong;
                break;
            default:
                Debug.LogWarning("잘못된 index입니다: " + index);
                return;
        }

        audioSource.Play();
    }

}
