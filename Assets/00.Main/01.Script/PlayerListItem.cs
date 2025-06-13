using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using Spine.Unity;
using System.Linq;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text text;
    Player player;
    [SerializeField] private SkeletonGraphic skeletonGraphic;

    [Header("Skin ����")]
    [SerializeField] private string redSkin = "skin3";
    [SerializeField] private string blueSkin = "skin2";
    [SerializeField] private string yellowSkin = "skin4";
    [SerializeField] private string blackSkin = "skin1";

    [Header("�ִϸ��̼� ����")]
    [SerializeField] private AnimationReferenceAsset redAnim;
    [SerializeField] private AnimationReferenceAsset blueAnim;
    [SerializeField] private AnimationReferenceAsset yellowAnim;
    [SerializeField] private AnimationReferenceAsset blackAnim;

    public void Setup(Player _player)
    {
        player = _player;
        text.text = _player.NickName;

        if (_player.CustomProperties.TryGetValue("CharacterType", out object charTypeObj))
        {
            int charTypeInt = (int)charTypeObj;
            CharacterType characterType = (CharacterType)charTypeInt;
            Debug.Log($"[{_player.NickName}] ĳ���� Ÿ��: {characterType}");

            // SkeletonGraphic�� ������� �ʴٸ� ����
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

            // �ؽ�Ʈ�� Ÿ�� ���� �߰�
          //  text.text += $" ({characterType})";
        }
        else
        {
            Debug.LogWarning($"[{_player.NickName}] ĳ���� Ÿ�� ���� ����");
        }
    }
    private string GetSkinName(CharacterType type)
    {
        switch (type)
        {
            case CharacterType.Red: return redSkin;
            case CharacterType.Blue: return blueSkin;
            case CharacterType.Yellow: return yellowSkin;
            case CharacterType.Black: return blackSkin;
            default: return redSkin;
        }
    }

    private AnimationReferenceAsset GetAnimation(CharacterType type)
    {
        switch (type)
        {
            case CharacterType.Red: return redAnim;
            case CharacterType.Blue: return blueAnim;
            case CharacterType.Yellow: return yellowAnim;
            case CharacterType.Black: return blackAnim;
            default: return redAnim;
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
}
