using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Photon.Pun;

public class HoldUpgradeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material originalMaterial;
    [SerializeField] private float scaleDuration = 0.3f;
    [SerializeField] private float highlightScale = 1.2f;

    private RectTransform rectTransform;
    private Vector3 originalScale;
    private UnityEngine.UI.Image image;

    public int hightlightAudioIndex;
    public int clickIndex;

    [SerializeField]  public Hold hold;


    void Awake()
    {
        hold = transform.parent.parent.GetComponent<Hold>();
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
        image = GetComponent<UnityEngine.UI.Image>();
    }
    private void OnDisable()
    {
        if (image != null)
        {
            image.material = originalMaterial;
        }
        else
        {
            image.material = null;
        }

        rectTransform.DOScale(originalScale, scaleDuration).SetEase(Ease.InOutQuad);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (image != null && highlightMaterial != null)
        {
            image.material = highlightMaterial;
            //AudioManager.instance.PlaySound(transform.position, hightlightAudioIndex, Random.Range(1f, 1f), 1f);
        }

        rectTransform.DOScale(originalScale * highlightScale, scaleDuration).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (image != null)
        {
            image.material = originalMaterial;
        }
        else
        {
            image.material = null;
        }
        rectTransform.DOScale(originalScale, scaleDuration).SetEase(Ease.InOutQuad);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PhotonView holdView = hold.GetComponent<PhotonView>();
        if (holdView != null)
        {
            int materialIndex = GameManager.instance.playerScript.GetComponent<PlayerColor>().GetMaterialIndex();
            int holdTypeInt = (int)GameManager.instance.playerScript.GetComponent<PlayerColor>().HoldChange();
            holdView.RPC("HoldColorChange", RpcTarget.AllBuffered, materialIndex, holdTypeInt);
            holdView.RPC("HoldPriceUp", RpcTarget.AllBuffered, 100, PhotonNetwork.NickName);
            KeyCardManager.instance.keycardHoldUpgrade.UIclose();
        }
        TurnManager.instance.EndTurn();
        //  AudioManager.instance.PlaySound(transform.position, clickIndex, Random.Range(1f, 1f), 1f);
    }

}
