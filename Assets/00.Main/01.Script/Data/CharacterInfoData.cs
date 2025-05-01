using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Character/Character InfoData")]
public class CharacterInfoData : ScriptableObject
{
    [Header("기본 정보")]
    public string characterName;      // 캐릭터 이름
    [TextArea]
    public string characterInfo;      // 캐릭터 설명
    [TextArea]
    public string characteristic;

    public float moveSpeed;
    public float hp;

}
