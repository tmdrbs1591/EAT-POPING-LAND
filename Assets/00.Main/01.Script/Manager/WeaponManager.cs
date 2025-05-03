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

    // WeaponType�� �Ű������� �޾Ƽ� ���� ���� Ÿ���� ����
    public void WeaponChange(WeaponType newWeaponType)
    {
        currentWeaponType = newWeaponType;
        weaponUIScript.UIUpate();
        Debug.Log("���� �����: " + currentWeaponType);
    }
}
