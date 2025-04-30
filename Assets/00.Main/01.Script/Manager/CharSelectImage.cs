using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

public class CharSelectImage : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation skeletonAnimation;

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

    private CharacterType lastCharacterType;

    void Start()
    {
        lastCharacterType = CharacterType.Red - 1; // 무조건 다른 값으로 시작해서 무조건 한 번은 실행됨
        UpdateSkinAndAnimation();
    }

    void Update()
    {
        if (CharacterManager.instance.characterType != lastCharacterType)
        {
            UpdateSkinAndAnimation();
        }
    }

    void UpdateSkinAndAnimation()
    {
        lastCharacterType = CharacterManager.instance.characterType;

        // 스킨 적용
        string skinName = GetSkinNameFromCharacterType(lastCharacterType);
        skeletonAnimation.Skeleton.SetSkin(skinName);
        skeletonAnimation.Skeleton.SetSlotsToSetupPose();
        skeletonAnimation.AnimationState.Apply(skeletonAnimation.Skeleton);

        // 애니메이션 적용
        AnimationReferenceAsset animAsset = GetAnimationAssetFromCharacterType(lastCharacterType);
        if (animAsset != null)
        {
            skeletonAnimation.AnimationState.SetAnimation(0, animAsset, true);
        }
    }

    string GetSkinNameFromCharacterType(CharacterType type)
    {
        switch (type)
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

    AnimationReferenceAsset GetAnimationAssetFromCharacterType(CharacterType type)
    {
        switch (type)
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
}
