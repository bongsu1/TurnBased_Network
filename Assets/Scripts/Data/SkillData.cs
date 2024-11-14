using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "ScriptableObject/SkillData")]
public class SkillData : ScriptableObject
{
    public enum Target
    {
        OtherTeam, // 상대팀
        MyTeam, // 내팀
        None,
    }

    public string skillName;
    [TextArea] public string descript; // 스킬 설명
    public int damage;
    public Target target;
    // 타입
    // 쿨타임
}
