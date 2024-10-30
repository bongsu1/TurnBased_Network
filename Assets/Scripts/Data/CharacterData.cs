using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName ="ScriptableObject/CharacterData")]
public class CharacterData : ScriptableObject
{
    public int charID; // 캐릭터 ID

    public string charName;
    public int hp;
    public int speed;
    public SkillData[] skills;
    // 타입

    // 스프라이트 앞모습, 뒷모습
}
