using UnityEngine;
using DG.Tweening;

public class WeaponButton : MonoBehaviour
{
    public WeaponType weaponTypeToSelect;
    public StatType statTypeToSelect;

    [Header("Fade Settings")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float delayBeforeFade = 0.1f; // 알파 변화 시작 전 딜레이
    [SerializeField] private float fadeDuration = 0.5f;    // 알파가 1로 되기까지 걸리는 시간
    [SerializeField] private int price;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    private void OnEnable()
    {
        // 초기 알파값 0
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, fadeDuration)
                       .SetDelay(delayBeforeFade)
                       .SetEase(Ease.OutQuad);
        }
    }

    public void OnClickChangeWeapon()
    {
        PlayerMoney playerMoney = GameManager.instance.playerScript.gameObject.GetComponent<PlayerMoney>();

        if (playerMoney.money >= price)
        {
            WeaponManager.instance.WeaponChange(weaponTypeToSelect);
        playerMoney.AddMoney(-price);
        }
    }


    public void OnClickUpgrade()
    {
        PlayerMoney playerMoney = GameManager.instance.playerScript.gameObject.GetComponent<PlayerMoney>();

        if (playerMoney.money >= price)
        {
            GameManager.instance.playerScript.playerbattleScript.UpgradeStat(statTypeToSelect);
            playerMoney.AddMoney(-price);
        }
    }
}
