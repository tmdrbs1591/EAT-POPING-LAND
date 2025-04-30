using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum CharacterType
{
    Red,
    Blue,
    Yellow,
    Black
}

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager instance;

    public CharacterType characterType;

    private int currentIndex = 0;

    public bool isCharSelect;

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);

    }


}
