using UnityEngine;
using Photon.Pun;



public class PlayerColor : MonoBehaviourPun
{
    public ColorType playerColor;

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

    public Material MaterialChange() // ���׸��� ����
    {
        switch (playerColor)
        {
            case ColorType.Red:
                return redMaterial;
            case ColorType.Green:
                return greenMaterial;
            case ColorType.Blue:
                return blueMaterial;
            case ColorType.Yellow:
                return yellowMaterial;
            default:
                return null; // �⺻�� (���� ����)
        }
    }
    public int GetMaterialIndex() // Material�� �ε��� ��ȯ
    {
        switch (playerColor)
        {
            case ColorType.Red:
                return 0; // Red�� �ش��ϴ� �ε���
            case ColorType.Green:
                return 1; // Green�� �ش��ϴ� �ε���
            case ColorType.Blue:
                return 2; // Blue�� �ش��ϴ� �ε���
            case ColorType.Yellow:
                return 3; // Yellow�� �ش��ϴ� �ε���
            default:
                return -1; // Default�� ��� �ε��� -1 (�߸��� ��)
        }
    }

    public ColorType HoldChange() // Ÿ�Ժ���
    {
        switch (playerColor)
        {
            case ColorType.Red:
                return ColorType.Red;
            case ColorType.Green:
                return ColorType.Green; 
            case ColorType.Blue:
                return ColorType.Blue; 
            case ColorType.Yellow:
                return ColorType.Yellow; 
            default:
                return ColorType.Default; // �⺻�� (���� ����)
        }
    }
    [PunRPC]
    void SetColor(int colorIndex)
    {
        playerColor = (ColorType)colorIndex;
        ApplyColor();
    }

    void ApplyColor()
    {
        Color newColor;

        switch (playerColor)
        {
            case ColorType.Red:
                newColor = Color.red;
                break;
            case ColorType.Green:
                newColor = Color.green;
                break;
            case ColorType.Blue:
                newColor = Color.blue;
                break;
            case ColorType.Yellow:
                newColor = Color.yellow;
                break;
            default:
                newColor = Color.white;
                break;
        }

        newColor.a = 0.8f; // ���İ��� ������ ����
        spriteRenderer.color = newColor;
    } // �� �ؿ� ����

}
