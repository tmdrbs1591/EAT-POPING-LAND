using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

public class CharSelectImage : MonoBehaviour
{
    [Header("Spine 컴포넌트")]
    [SerializeField] private SkeletonAnimation skeletonAnimation;
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

    private CharacterType lastCharacterType;

    void Start()
    {
        lastCharacterType = CharacterType.Red - 1; // 항상 첫 실행 유도
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
        if (skeletonAnimation == null && skeletonGraphic == null)
            return;

        lastCharacterType = CharacterManager.instance.characterType;

        string skinName = GetSkinNameFromCharacterType(lastCharacterType);
        AnimationReferenceAsset animAsset = GetAnimationAssetFromCharacterType(lastCharacterType);

        // SkeletonAnimation 적용
        if (skeletonAnimation != null)
        {
            skeletonAnimation.Skeleton.SetSkin(skinName);
            skeletonAnimation.Skeleton.SetSlotsToSetupPose();
            skeletonAnimation.AnimationState.Apply(skeletonAnimation.Skeleton);

            if (animAsset != null)
                skeletonAnimation.AnimationState.SetAnimation(0, animAsset, true);
        }

        // SkeletonGraphic 적용
        if (skeletonGraphic != null)
        {
            skeletonGraphic.Skeleton.SetSkin(skinName);
            skeletonGraphic.Skeleton.SetSlotsToSetupPose();
            skeletonGraphic.AnimationState.Apply(skeletonGraphic.Skeleton);

            if (animAsset != null)
                skeletonGraphic.AnimationState.SetAnimation(0, animAsset, true);
        }
    }

    string GetSkinNameFromCharacterType(CharacterType type)
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

    AnimationReferenceAsset GetAnimationAssetFromCharacterType(CharacterType type)
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
}
