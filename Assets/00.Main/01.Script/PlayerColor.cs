using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlayerColorType
{
    Red,
    Green, 
    Blue,
    Yellow
}
public class PlayerColor : MonoBehaviour
{

    public PlayerColorType playerColor;

    [SerializeField] SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        switch (playerColor)
        {
            case PlayerColorType.Red:
                spriteRenderer.color = Color.red;
                break;
            case PlayerColorType.Green:
                spriteRenderer.color = Color.green;
                break;
            case PlayerColorType.Blue:
                spriteRenderer.color = Color.blue;
                break;
            case PlayerColorType.Yellow:
                spriteRenderer.color = Color.yellow;
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
