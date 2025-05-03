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

    [SerializeField] Sprite candySprite;
    [SerializeField] Sprite knifeSprite;
    [SerializeField] Sprite batSprite;
    [SerializeField] Sprite boomGunSprite;
    [SerializeField] Sprite bubbleGunSprite;
    [SerializeField] Sprite magicWandSprite;

    private void Awake()
    {
        if (pv.IsMine)
        {
            WeaponManager.instance.weaponUIScript = this;

        }
    }
    public void UIUpate()
    {
        for (int i = 0; i < weaponImage.Length; i++)
        {
            switch (WeaponManager.instance.currentWeaponType)
            {
                case WeaponType.Candy:
                    weaponImage[i].sprite = candySprite;
                    break;
                case WeaponType.Knife:
                    weaponImage[i].sprite = knifeSprite;
                    break;
                case WeaponType.Bat:
                    weaponImage[i].sprite = batSprite;
                    break;
                case WeaponType.BoomGun:
                    weaponImage[i].sprite = boomGunSprite;
                    break;
                case WeaponType.BubbleGun:
                    weaponImage[i].sprite = bubbleGunSprite;
                    break;
                case WeaponType.MagicWand:
                    weaponImage[i].sprite = magicWandSprite;
                    break;
            }
        }
    }
}
