using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class KeyCardDataList
{
    public KeyCardType keyCardType;

    public string keyCardName;
    [TextArea]
    public string keyCardInfo;
    public Sprite keyCardImage; // Image -> Sprite·Î º¯°æ
}

public enum KeyCardType
{
    None,
    UFO,
}

[CreateAssetMenu(fileName = "New KeyCardDataList", menuName = "KeyCard/KeyCardDataList")]
public class KeyCardData : ScriptableObject
{
    public List<KeyCardDataList> keyCards;
}
