using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon; // �� �ʿ�!
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

public class WeaponManager : MonoBehaviourPunCallbacks
{
    public static WeaponManager instance;

    public WeaponType currentWeaponType;
    public WeaponUI weaponUIScript;

    [SerializeField] private GameObject shopPanel;

    private void Awake()
    {
        instance = this;
    }

    public void WeaponChange(WeaponType newWeaponType)
    {
        currentWeaponType = newWeaponType;

        var props = new ExitGames.Client.Photon.Hashtable();
        props["WeaponType"] = (int)newWeaponType;
        props["WeaponOwner"] = PhotonNetwork.LocalPlayer.ActorNumber; // ���� �ٲ���� ǥ��
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    public override void OnRoomPropertiesUpdate(Hashtable changedProps)
    {
        if (changedProps.ContainsKey("WeaponType") && changedProps.ContainsKey("WeaponOwner"))
        {
            WeaponType updatedWeapon = (WeaponType)(int)changedProps["WeaponType"];
            int ownerActorNumber = (int)changedProps["WeaponOwner"];

            // ��� WeaponUI �߿��� �ش� �÷��̾��� UI�� ������Ʈ
            WeaponUI[] allUIs = FindObjectsOfType<WeaponUI>();
            foreach (var ui in allUIs)
            {
                if (ui.pv.Owner.ActorNumber == ownerActorNumber)
                {
                    ui.UIUpate(updatedWeapon);
                    Debug.Log($"Actor {ownerActorNumber}�� ���� UI ������Ʈ: {updatedWeapon}");
                }
            }
        }
    }

    public void ShopPanelOpen()
    {
        shopPanel.gameObject.SetActive(true);
    }
}