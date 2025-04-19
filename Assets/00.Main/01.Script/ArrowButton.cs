using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ArrowButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private float scaleDuration = 0.3f;
    [SerializeField] private float highlightScale = 1.2f;

    private RectTransform rectTransform;
    private Vector3 originalScale;
    private UnityEngine.UI.Image image;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
        image = GetComponent<UnityEngine.UI.Image>();
    }
    private void OnDisable()
    {
        if (image != null)
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
        }

        rectTransform.DOScale(originalScale * highlightScale, scaleDuration).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (image != null)
        {
            image.material = null;
        }

        rectTransform.DOScale(originalScale, scaleDuration).SetEase(Ease.InOutQuad);
    }
}
