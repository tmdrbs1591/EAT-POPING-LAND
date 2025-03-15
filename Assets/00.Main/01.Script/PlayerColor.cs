using UnityEngine;
using Photon.Pun;

public enum PlayerColorType
{
    Red,
    Green,
    Blue,
    Yellow
}

public class PlayerColor : MonoBehaviourPun
{
    public PlayerColorType playerColor;

    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Start()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("SetColor", RpcTarget.AllBuffered, (int)playerColor);
        }
    }

    [PunRPC]
    void SetColor(int colorIndex)
    {
        playerColor = (PlayerColorType)colorIndex;
        ApplyColor();
    }

    void ApplyColor()
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
        }
    }
}
