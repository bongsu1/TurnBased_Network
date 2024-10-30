using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_BattlePopup : UI_Popup
{
    private enum Buttons
    {
        FirstSkillButton,
        SecondSkillButton,
        ThirdSkillButton,
        My1stCharacterButton,
        My2ndCharacterButton,
        My3rdCharacterButton,
        My4thCharacterButton,
        Other1stCharacterButton,
        Other2ndCharacterButton,
        Other3rdCharacterButton,
        Other4thCharacterButton,
    }

    private enum Sliders
    {
        TurnSlider1,
        TurnSlider2,
        TurnSlider3,
        TurnSlider4,
        TurnSlider5,
        TurnSlider6,
        TurnSlider7,
        TurnSlider8,
    }

    private enum Images
    {
        TurnHandle1,
        TurnHandle2,
        TurnHandle3,
        TurnHandle4,
        TurnHandle5,
        TurnHandle6,
        TurnHandle7,
        TurnHandle8,
    }

    private enum Texts
    {
        FirstSkillText,
        SecondSkillText,
        ThirdSkillText,
        My1stCharacterButtonText,
        My2ndCharacterButtonText,
        My3rdCharacterButtonText,
        My4thCharacterButtonText,
        Other1stCharacterButtonText,
        Other2ndCharacterButtonText,
        Other3rdCharacterButtonText,
        Other4thCharacterButtonText,
    }

    private int selectSkill = 0; // 스킬을 선택하지 않았을때에도 1번(skills[0]) 스킬을 기본으로

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Button>(typeof(Buttons));
        Bind<Slider>(typeof(Sliders));
        Bind<Image>(typeof(Images));
        Bind<TMP_Text>(typeof(Texts));

        InfomationInit();

        Get<Button>((int)Buttons.FirstSkillButton).gameObject.BindEvent(OnClickFirstSkillButton);
        Get<Button>((int)Buttons.SecondSkillButton).gameObject.BindEvent(OnClickSeconSkillButton);
        Get<Button>((int)Buttons.ThirdSkillButton).gameObject.BindEvent(OnClickThirdSkillButton);

        for (int i = 0; i < 8; i++)
        {
            int idx = i;
            Get<Button>((int)Buttons.My1stCharacterButton + i).gameObject.BindEvent(() => OnClickCharButton(idx));
        }

        return true;
    }

    private void OnClickFirstSkillButton()
    {
        Debug.Log($"{curTurn.name}의 {curTurn.skills[0].name}선택");
        selectSkill = 0;
    }

    private void OnClickSeconSkillButton()
    {
        Debug.Log($"{curTurn.name}의 {curTurn.skills[1].name}선택");
        selectSkill = 1;
    }

    private void OnClickThirdSkillButton()
    {
        Debug.Log($"{curTurn.name}의 {curTurn.skills[2].name}선택");
        selectSkill = 2;
    }

    private void OnClickCharButton(int character)
    {
        Debug.Log($"{inBattle[character].name}에게 사용");
        int curIdx = Array.IndexOf(inBattle, curTurn);
        actionGauges[curIdx] = 0;

        curTurn = TakeTurn();
        OnClickFirstSkillButton();
    }

    private void SetActionGaugeUI()
    {
        for (int i = 0; i < 8; i++)
        {
            Slider turnGauge = Get<Slider>((int)Sliders.TurnSlider1 + i);
            if (lives[i] == false)
            {
                turnGauge.gameObject.SetActive(false);
            }

            turnGauge.value = actionGauges[i];
        }
    }

    // 구현부
    // 테스트

    private T_Character[] inBattle = new T_Character[8]; // 배틀에 참여중인 캐릭터들 
    private int[] actionGauges = new int[8]; // 배틀에 참여한 캐릭터들의 행동게이지
    private bool[] lives = new bool[8]; // 배틀에 참여중인 캐릭터들의 전투 가능여부 // 체력이 다 닳으면 false;
    // 위의 두 배열은 구조체로 한번에 담아도 되겠다

    private int maxSpeed = 0; // 이번 배틀에서 가장 빠른 속도
    private int maxGauge = 0; // 가장 빠른 속도를 기준으로 행동게이지 최대치를 정함
    private bool _setMaxGauge = false;

    private Queue<T_Character> turnQue = new Queue<T_Character>(8);
    private T_Character curTurn = null;

    public void InfomationInit()
    {
        // 테스트
        inBattle[0] = new T_Character("이상해씨", 45, T_Character.Attribute.Grass, 45, new T_Skill[] { new T_Skill("이상해씨1번", 1), new T_Skill("이상해씨2번", 1), new T_Skill("이상해씨3번", 1) });
        inBattle[1] = new T_Character("파이리", 39, T_Character.Attribute.Fire, 65, new T_Skill[] { new T_Skill("파이리1번", 1), new T_Skill("파이리2번", 1), new T_Skill("파이리3번", 1) });
        inBattle[2] = new T_Character("꼬부기", 44, T_Character.Attribute.Water, 43, new T_Skill[] { new T_Skill("꼬부기1번", 1), new T_Skill("꼬부기2번", 1), new T_Skill("꼬부기3번", 1) });
        inBattle[3] = new T_Character("피카츄", 35, T_Character.Attribute.electricity, 90, new T_Skill[] { new T_Skill("피카츄1번", 1), new T_Skill("피카츄2번", 1), new T_Skill("피카츄3번", 1) });
        inBattle[4] = new T_Character("이브이", 55, T_Character.Attribute.Normal, 55, new T_Skill[] { new T_Skill("이브이1번", 1), new T_Skill("이브이2번", 1), new T_Skill("이브이3번", 1) });
        inBattle[5] = new T_Character("나무지기", 30, T_Character.Attribute.Grass, 70, new T_Skill[] { new T_Skill("나무지기1번", 1), new T_Skill("나무지기2번", 1), new T_Skill("나무지기3번", 1) });
        inBattle[6] = new T_Character("아차모", 45, T_Character.Attribute.Fire, 45, new T_Skill[] { new T_Skill("아차모1번", 1), new T_Skill("아차모2번", 1), new T_Skill("아차모3번", 1) });
        inBattle[7] = new T_Character("물짱이", 50, T_Character.Attribute.Water, 40, new T_Skill[] { new T_Skill("물짱이1번", 1), new T_Skill("물짱이2번", 1), new T_Skill("물짱이3번", 1) });

        for (int i = 0; i < 8; i++)
        {
            TMP_Text name = Get<TMP_Text>((int)Texts.My1stCharacterButtonText + i);
            name.text = inBattle[i].name;
        }

        for (int i = 0; i < 8; i++)
        {
            lives[i] = true;
        }

        StartBattle();
    }

    private void StartBattle()
    {
        for (int i = 0; i < 8; i++)
        {
            int curSpeed = inBattle[i].speed;
            if (maxSpeed < curSpeed)
            {
                maxSpeed = curSpeed;
            }
        }

        maxGauge = maxSpeed * 10;
        SetMaxGauge(maxGauge);
        TakeTurn();
    }

    private void SetMaxGauge(int maxGauge)
    {
        if (_setMaxGauge)
            return;

        for (int i = 0; i < 8; i++)
        {
            Slider turnGauge = Get<Slider>((int)Sliders.TurnSlider1 + i);
            turnGauge.maxValue = maxGauge;
        }
        _setMaxGauge = true;
    }

    private void increaseGauge()
    {
        // 행동게이지가 가득찬 캐릭터가 없으면
        while (turnQue.Count == 0)
        {
            for (int i = 0; i < 8; i++)
            {
                if (lives[i] == false)
                    continue;

                actionGauges[i] += inBattle[i].speed;

                // 행동게이지가 가득 찼으면
                if (maxGauge <= actionGauges[i])
                {
                    turnQue.Enqueue(inBattle[i]);
                }
            }
        }
    }

    // 턴 큐에 있는 캐릭터를 가져온다
    private T_Character TakeTurn()
    {
        if (turnQue.Count == 0)
        {
            increaseGauge();
        }

        SetActionGaugeUI();
        curTurn = turnQue.Dequeue();
        for (int i = 0; i < 3; i++)
        {
            int idx = i;
            Get<TMP_Text>((int)Texts.FirstSkillText + i).text = curTurn.skills[i].name;
        }

        return curTurn;
    }
}
