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

    public Material redMaterial;
    public Material blueMaterial;
    public Material yellowMaterial;
    public Material greenMaterial;

    private void Start()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("SetColor", RpcTarget.AllBuffered, (int)playerColor);
        }
    }

    public Material MaterialChange()
    {
        switch (playerColor)
        {
            case PlayerColorType.Red:
                return redMaterial;
            case PlayerColorType.Green:
                return greenMaterial;
            case PlayerColorType.Blue:
                return blueMaterial;
            case PlayerColorType.Yellow:
                return yellowMaterial;
            default:
                return null; // 기본값 (오류 방지)
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
