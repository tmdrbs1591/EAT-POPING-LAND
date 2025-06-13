using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Spine.Unity;
using System.Linq;
using TMPro;
using Photon.Pun.Demo.PunBasics;
public class CrownManager : MonoBehaviourPunCallbacks
{
    public GameObject crownPanel;

    public Animator anim;

    public GameObject resultPanel;

    public bool isOpen;

    [SerializeField] TMP_Text text;
    Player player;
    [SerializeField] private SkeletonGraphic skeletonGraphic;

    [Header("Skin 설정")]
    [SerializeField] private string redSkin = "skin3";
    [SerializeField] private string blueSkin = "skin2";
    [SerializeField] private string yellowSkin = "skin4";
    [SerializeField] private string blackSkin = "skin1";

    [Header("애니메이션 설정")]
    [SerializeField] private AnimationReferenceAsset redAnim;
    [SerializeField] private AnimationReferenceAsset blueAnim;
    [SerializeField] private AnimationReferenceAsset yellowAnim;
    [SerializeField] private AnimationReferenceAsset blackAnim;

    private void Awake()
    {
      
    }

    private string GetSkinName(CharacterType characterType)
    {
        switch (characterType)
        {
            case CharacterType.Red:
                return redSkin;
            case CharacterType.Blue:
                return blueSkin;
            case CharacterType.Yellow:
                return yellowSkin;
            case CharacterType.Black:
                return blackSkin;
            default:
                return redSkin;
        }
    }

    private AnimationReferenceAsset GetAnimation(CharacterType characterType)
    {
        switch (characterType)
        {
            case CharacterType.Red:
                return redAnim;
            case CharacterType.Blue:
                return blueAnim;
            case CharacterType.Yellow:
                return yellowAnim;
            case CharacterType.Black:
                return blackAnim;
            default:
                return redAnim;
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (player == otherPlayer)
        {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
    public void Open()
    {
        if (!isOpen)
        {
            anim.SetTrigger("Open");
            isOpen = true;
        }
        else
        {
            anim.SetTrigger("Out");
            isOpen = false;
        }
    }

    public void GameEnd()
    {
        PlayerMoney playerMoney = GameManager.instance.playerScript.gameObject.GetComponent<PlayerMoney>();

        if (playerMoney.money >= 10000)
        {
            playerMoney.AddMoney(-10000);

            // 내 캐릭터 타입 가져오기
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("CharacterType", out object charTypeObj);
            int charTypeInt = (int)charTypeObj;

            // 닉네임, 캐릭터 타입 같이 넘기기
            photonView.RPC(nameof(PanelOpenRPC), RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName, charTypeInt);
        }
    }


    [PunRPC]
    void PanelOpenRPC(string winnerName, int charTypeInt)
    {
        CharacterType characterType = (CharacterType)charTypeInt;
        Debug.Log($"[승자] 캐릭터 타입: {characterType}");

        if (skeletonGraphic != null && skeletonGraphic.Skeleton != null)
        {
            string skinName = GetSkinName(characterType);
            skeletonGraphic.Skeleton.SetSkin(skinName);
            skeletonGraphic.Skeleton.SetSlotsToSetupPose();
            skeletonGraphic.AnimationState.Apply(skeletonGraphic.Skeleton);

            AnimationReferenceAsset anim = GetAnimation(characterType);
            if (anim != null)
            {
                skeletonGraphic.AnimationState.SetAnimation(0, anim, true);
            }
        }

        text.text = winnerName;

        resultPanel.SetActive(true);
    }

}
