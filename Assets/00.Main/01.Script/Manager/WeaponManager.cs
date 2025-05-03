using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon; // 꼭 필요!
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
        props["WeaponOwner"] = PhotonNetwork.LocalPlayer.ActorNumber; // 누가 바꿨는지 표시
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    public override void OnRoomPropertiesUpdate(Hashtable changedProps)
    {
        if (changedProps.ContainsKey("WeaponType") && changedProps.ContainsKey("WeaponOwner"))
        {
            WeaponType updatedWeapon = (WeaponType)(int)changedProps["WeaponType"];
            int ownerActorNumber = (int)changedProps["WeaponOwner"];

            // 모든 WeaponUI 중에서 해당 플레이어의 UI만 업데이트
            WeaponUI[] allUIs = FindObjectsOfType<WeaponUI>();
            foreach (var ui in allUIs)
            {
                if (ui.pv.Owner.ActorNumber == ownerActorNumber)
                {
                    ui.UIUpate(updatedWeapon);
                    Debug.Log($"Actor {ownerActorNumber}의 무기 UI 업데이트: {updatedWeapon}");
                }
            }
        }
    }

    public void ShopPanelOpen()
    {
        shopPanel.gameObject.SetActive(true);
    }
}