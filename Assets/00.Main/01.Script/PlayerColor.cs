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
                return null; // �⺻�� (���� ����)
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
        Color newColor;

        switch (playerColor)
        {
            case PlayerColorType.Red:
                newColor = Color.red;
                break;
            case PlayerColorType.Green:
                newColor = Color.green;
                break;
            case PlayerColorType.Blue:
                newColor = Color.blue;
                break;
            case PlayerColorType.Yellow:
                newColor = Color.yellow;
                break;
            default:
                newColor = Color.white;
                break;
        }

        newColor.a = 0.5f; // ���İ��� ������ ����
        spriteRenderer.color = newColor;
    }

}
