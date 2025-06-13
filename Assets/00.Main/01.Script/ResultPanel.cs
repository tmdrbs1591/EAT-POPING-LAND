using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using Spine.Unity;
using TMPro;

public class ResultPanel : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector2 originalPosition;

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

    private void Awake()
    {
        Player myPlayer = PhotonNetwork.LocalPlayer;
        text.text = PhotonNetwork.LocalPlayer.NickName;

        if (myPlayer.CustomProperties.TryGetValue("CharacterType", out object charTypeObj))
        {
            int charTypeInt = (int)charTypeObj;
            CharacterType characterType = (CharacterType)charTypeInt;
            Debug.Log($"[��] ĳ���� Ÿ��: {characterType}");

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
        }
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
    void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;

        // ������ �� ���� �÷��α�
        rectTransform.anchoredPosition = new Vector2(originalPosition.x, originalPosition.y + 2500f);

        // ������ Ƣ�鼭 �������� �ִϸ��̼�
        rectTransform.DOAnchorPosY(originalPosition.y, 1f)
            .SetEase(Ease.OutBounce);
    }
}
