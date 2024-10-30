using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{

}

// 테스트 캐릭터
public class T_Character
{
    public enum Attribute
    {
        Normal,
        Grass,
        Fire,
        Water,
        electricity,
    }

    public string name;
    public int hp;
    public Attribute attribute;
    public int speed;
    public T_Skill[] skills;

    public T_Character(string name, int hp, Attribute attribute, int speed, T_Skill[] skills)
    {
        this.name = name;
        this.hp = hp;
        this.attribute = attribute;
        this.speed = speed;
        this.skills = skills;
    }
}

public class T_Skill
{
    public string name;
    public int damage;

    public T_Skill(string name, int damage)
    {
        this.name = name;
        this.damage = damage;
    }
}