using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Character/Character InfoData")]
public class CharacterInfoData : ScriptableObject
{
    [Header("�⺻ ����")]
    public string characterName;      // ĳ���� �̸�
    [TextArea]
    public string characterInfo;      // ĳ���� ����
    [TextArea]
    public string characteristic;

    public float moveSpeed;
    public float hp;

}
