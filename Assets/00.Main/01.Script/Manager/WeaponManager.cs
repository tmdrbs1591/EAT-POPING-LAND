using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Candy,
    Knife,
    Bat,
    BoomGun,
    BubbleGun,
    MagicWand
}

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager instance;

    public WeaponType currentWeaponType;

    public WeaponUI weaponUIScript;
    private void Awake()
    {
        instance = this;
    }

    // WeaponType을 매개변수로 받아서 현재 무기 타입을 변경
    public void WeaponChange(WeaponType newWeaponType)
    {
        currentWeaponType = newWeaponType;
        weaponUIScript.UIUpate();
        Debug.Log("무기 변경됨: " + currentWeaponType);
    }
}
