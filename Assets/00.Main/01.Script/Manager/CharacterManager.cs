using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
