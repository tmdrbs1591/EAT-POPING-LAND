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

    public Material MaterialChange() // 메테리얼 변경
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
                return null; // 기본값 (오류 방지)
        }
    }
    public int GetMaterialIndex() // Material의 인덱스 반환
    {
        switch (playerColor)
        {
            case ColorType.Red:
                return 0; // Red에 해당하는 인덱스
            case ColorType.Green:
                return 1; // Green에 해당하는 인덱스
            case ColorType.Blue:
                return 2; // Blue에 해당하는 인덱스
            case ColorType.Yellow:
                return 3; // Yellow에 해당하는 인덱스
            default:
                return -1; // Default일 경우 인덱스 -1 (잘못된 값)
        }
    }

    public ColorType HoldChange() // 타입변경
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
                return ColorType.Default; // 기본값 (오류 방지)
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

        newColor.a = 0.8f; // 알파값을 반으로 줄임
        spriteRenderer.color = newColor;
    } // 발 밑에 색깔

}
