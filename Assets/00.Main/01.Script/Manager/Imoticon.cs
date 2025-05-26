using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Imoticon : MonoBehaviourPun
{
    [SerializeField] GameObject[] imoticonOb;     // �̸�Ƽ�� ������Ʈ��
    [SerializeField] Image[] imoticonImage;       // �̸�Ƽ�� �̹�����
    [SerializeField] Sprite smileImage;           // ������ ��������Ʈ
    [SerializeField] Sprite cuteImage;            // �Ϳ��� ��������Ʈ
    [SerializeField] Sprite sadImage;             // ���� ��������Ʈ
    [SerializeField] Sprite badImage;             // ����(ȭ��) ��������Ʈ

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
                Debug.LogWarning("�������� �ʴ� �̸�Ƽ�� �̸�: " + imoticonName);
                return;
        }

        foreach (var obj in imoticonOb)
        {
            obj.SetActive(false); // ���� ��
            obj.SetActive(true);  // �ٽ� Ȱ��ȭ
        }

        foreach (var image in imoticonImage)
        {
            image.sprite = selectedSprite;
        }
    }
}
