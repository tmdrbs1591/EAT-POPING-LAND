using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;

public class WeaponUI : MonoBehaviourPunCallbacks
{
    public PhotonView pv;
    [SerializeField] Image[] weaponImage;

    [SerializeField] SpriteRenderer meleeWeaponRen;
    [SerializeField] SpriteRenderer distanceWeaponRen;

    [SerializeField] Sprite candySprite;
    [SerializeField] Sprite knifeSprite;
    [SerializeField] Sprite batSprite;
    [SerializeField] Sprite boomGunSprite;
    [SerializeField] Sprite bubbleGunSprite;
    [SerializeField] Sprite magicWandSprite;

    public PlayerBattle playerBattleScript;
    private void Awake()
    {
        if (pv.IsMine)
        {
            WeaponManager.instance.weaponUIScript = this;

        }
    }
    public void UIUpate(WeaponType weaponType)
    {
        for (int i = 0; i < weaponImage.Length; i++)
        {
            switch (weaponType)
            {
                case WeaponType.Candy:
                    weaponImage[i].sprite = candySprite;
                    meleeWeaponRen.sprite = candySprite;
                    playerBattleScript.attackType = AttackType.Melee;
                    meleeWeaponRen.gameObject.SetActive(true);
                    distanceWeaponRen.gameObject.SetActive(false);
                    break;
                case WeaponType.Knife:
                    weaponImage[i].sprite = knifeSprite;
                    meleeWeaponRen.sprite = knifeSprite;
                    playerBattleScript.attackType = AttackType.Melee;
                    meleeWeaponRen.gameObject.SetActive(true);
                    distanceWeaponRen.gameObject.SetActive(false);
                    break;
                case WeaponType.Bat:
                    weaponImage[i].sprite = batSprite;
                    meleeWeaponRen.sprite = batSprite;
                    playerBattleScript.attackType = AttackType.Melee;
                    meleeWeaponRen.gameObject.SetActive(true);
                    distanceWeaponRen.gameObject.SetActive(false);
                    break;
                case WeaponType.BoomGun:
                    weaponImage[i].sprite = boomGunSprite;
                    distanceWeaponRen.sprite = boomGunSprite;
                    playerBattleScript.attackType = AttackType.Distance;
                    meleeWeaponRen.gameObject.SetActive(false);
                    distanceWeaponRen.gameObject.SetActive(true);
                    break;
                case WeaponType.BubbleGun:
                    weaponImage[i].sprite = bubbleGunSprite;
                    distanceWeaponRen.sprite = bubbleGunSprite;
                    playerBattleScript.attackType = AttackType.Distance;
                    meleeWeaponRen.gameObject.SetActive(false);
                    distanceWeaponRen.gameObject.SetActive(true);
                    break;
                case WeaponType.MagicWand:
                    weaponImage[i].sprite = magicWandSprite;
                    distanceWeaponRen.sprite = magicWandSprite;
                    playerBattleScript.attackType = AttackType.Distance;
                    meleeWeaponRen.gameObject.SetActive(false);
                    distanceWeaponRen.gameObject.SetActive(true);
                    break;
            }
        }
    }

}
