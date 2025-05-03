using UnityEngine;

public class WeaponButton : MonoBehaviour
{
    public WeaponType weaponTypeToSelect;

    public void OnClickChangeWeapon()
    {
        WeaponManager.instance.WeaponChange(weaponTypeToSelect);
    }
}
