using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "ScriptableObject/SkillData")]
public class SkillData : ScriptableObject
{
    public string skillName;
    [TextArea] public string descript; // 스킬 설명
    public int damage;
    // 타겟팅 : 아군인지 적인지 자신인지 타인인지
    // 타입
    // 쿨타임
}
