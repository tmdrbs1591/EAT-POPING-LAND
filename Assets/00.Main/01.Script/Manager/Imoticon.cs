using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Imoticon : MonoBehaviourPun
{
    [SerializeField] GameObject[] imoticonOb;     // 이모티콘 오브젝트들
    [SerializeField] Image[] imoticonImage;       // 이모티콘 이미지들
    [SerializeField] Sprite smileImage;           // 스마일 스프라이트
    [SerializeField] Sprite cuteImage;            // 귀여운 스프라이트
    [SerializeField] Sprite sadImage;             // 슬픈 스프라이트
    [SerializeField] Sprite badImage;             // 나쁜(화난) 스프라이트

    [SerializeField] GameObject ImoticonButton;

    private void Start()
    {
        if (!photonView.IsMine)
        {
            ImoticonButton.SetActive(false);
        }
    }
    public void ImoticonOn(string imoticonName)
    {
        photonView.RPC(nameof(RPC_ShowImoticon), RpcTarget.All, imoticonName);
    }

    [PunRPC]
    void RPC_ShowImoticon(string imoticonName)
    {
        Sprite selectedSprite = null;
        AudioManager.instance.PlaySound(transform.position, 5, Random.Range(1f, 1.2f), 1f);
        switch (imoticonName)
        {
            case "Smile":
                selectedSprite = smileImage;
                break;
            case "Cute":
                selectedSprite = cuteImage;
                break;
            case "Sad":
                selectedSprite = sadImage;
                break;
            case "Bad":
                selectedSprite = badImage;
                break;
            default:
                Debug.LogWarning("존재하지 않는 이모티콘 이름: " + imoticonName);
                return;
        }

        foreach (var obj in imoticonOb)
        {
            obj.SetActive(false); // 리셋 후
            obj.SetActive(true);  // 다시 활성화
        }

        foreach (var image in imoticonImage)
        {
            image.sprite = selectedSprite;
        }
    }
}
