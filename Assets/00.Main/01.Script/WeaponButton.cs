using UnityEngine;
using DG.Tweening;

public class WeaponButton : MonoBehaviour
{
    public WeaponType weaponTypeToSelect;
    public StatType statTypeToSelect;

    [Header("Fade Settings")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float delayBeforeFade = 0.1f; // ���� ��ȭ ���� �� ������
    [SerializeField] private float fadeDuration = 0.5f;    // ���İ� 1�� �Ǳ���� �ɸ��� �ð�
    [SerializeField] private int price;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    private void OnEnable()
    {
        // �ʱ� ���İ� 0
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
