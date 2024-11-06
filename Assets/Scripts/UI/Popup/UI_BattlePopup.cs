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
        My1stCharacterHP,
        My2ndCharacterHP,
        My3rdCharacterHP,
        My4thCharacterHP,
        Other1stCharacterHP,
        Other2ndCharacterHP,
        Other3rdCharacterHP,
        Other4thCharacterHP,
        CurTurnHP,
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
        CurrentTurnImage,
        SelectFirst,
        SelectSecond,
        SelectThird,
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
        CurrentTurnNameText,
        CurrentTurnHPText,
    }

    private int selectSkill = 0; // 스킬을 선택하지 않았을때에도 1번(skills[0]) 스킬을 기본으로
    private bool first = false;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Button>(typeof(Buttons));
        Bind<Slider>(typeof(Sliders));
        Bind<Image>(typeof(Images));
        Bind<TMP_Text>(typeof(Texts));

        Get<Button>((int)Buttons.FirstSkillButton).gameObject.BindEvent(OnClickFirstSkillButton);
        Get<Button>((int)Buttons.SecondSkillButton).gameObject.BindEvent(OnClickSeconSkillButton);
        Get<Button>((int)Buttons.ThirdSkillButton).gameObject.BindEvent(OnClickThirdSkillButton);

        ActiveSelectImage();

        for (int i = 0; i < 8; i++)
        {
            int idx = i;
            Get<Button>((int)Buttons.My1stCharacterButton + i).gameObject.BindEvent(() => OnClickCharButton(idx));
        }

        string myUID = Manager.Data.Auth.CurrentUser.UserId;
        first = myUID == Manager.Game.RoomInfo.uids[0];

        InfomationInit();

        return true;
    }

    private void OnClickFirstSkillButton()
    {
        if (curTurn.GetTeam == BattleCharacter.Team.My)
        {
            ActiveCharacterButton(curTurn.data.skills[0].target);
        }
        // 테스트
        else
        {
            ActiveCharacterButton(curTurn.data.skills[0].target == SkillData.Target.OtherTeam ? SkillData.Target.MyTeam : SkillData.Target.OtherTeam);
        }
        selectSkill = 0;
        ActiveSelectImage();
    }

    private void OnClickSeconSkillButton()
    {
        if (curTurn.GetTeam == BattleCharacter.Team.My)
        {
            ActiveCharacterButton(curTurn.data.skills[1].target);
        }
        // 테스트
        else
        {
            ActiveCharacterButton(curTurn.data.skills[1].target == SkillData.Target.OtherTeam ? SkillData.Target.MyTeam : SkillData.Target.OtherTeam);
        }
        selectSkill = 1;
        ActiveSelectImage();
    }

    private void OnClickThirdSkillButton()
    {
        if (curTurn.GetTeam == BattleCharacter.Team.My)
        {
            ActiveCharacterButton(curTurn.data.skills[2].target);
        }
        // 테스트
        else
        {
            ActiveCharacterButton(curTurn.data.skills[2].target == SkillData.Target.OtherTeam ? SkillData.Target.MyTeam : SkillData.Target.OtherTeam);
        }
        selectSkill = 2;
        ActiveSelectImage();
    }

    private void OnClickCharButton(int character)
    {
        Debug.Log($"{inBattle[character].data.charName}에게 사용");
        inBattle[character].CurHP -= curTurn.data.skills[selectSkill].damage;

        int curIdx = Array.IndexOf(inBattle, curTurn);
        inBattle[curIdx].actionGauge = 0;

        curTurn = TakeTurn();
        if (curTurn == null)
            return;

        OnClickFirstSkillButton();
    }

    private void SetActionGaugeUI()
    {
        for (int i = 0; i < 8; i++)
        {
            Slider turnGauge = Get<Slider>((int)Sliders.TurnSlider1 + i);
            if (inBattle[i].IsLive == false)
            {
                turnGauge.gameObject.SetActive(false);
            }

            turnGauge.value = inBattle[i].actionGauge;
        }
    }

    private void ActiveSelectImage()
    {
        Get<Image>((int)Images.SelectFirst).gameObject.SetActive(selectSkill == 0);
        Get<Image>((int)Images.SelectSecond).gameObject.SetActive(selectSkill == 1);
        Get<Image>((int)Images.SelectThird).gameObject.SetActive(selectSkill == 2);
    }

    // 버튼 활성화
    private void ActiveCharacterButton(SkillData.Target target)
    {
        switch (target)
        {
            case SkillData.Target.OtherTeam:
                Get<Button>((int)Buttons.My1stCharacterButton).gameObject.EventActive(false);
                Get<Button>((int)Buttons.My2ndCharacterButton).gameObject.EventActive(false);
                Get<Button>((int)Buttons.My3rdCharacterButton).gameObject.EventActive(false);
                Get<Button>((int)Buttons.My4thCharacterButton).gameObject.EventActive(false);

                Get<Button>((int)Buttons.My1stCharacterButton).interactable = false;
                Get<Button>((int)Buttons.My2ndCharacterButton).interactable = false;
                Get<Button>((int)Buttons.My3rdCharacterButton).interactable = false;
                Get<Button>((int)Buttons.My4thCharacterButton).interactable = false;

                if (inBattle[4].IsLive)
                {
                    Get<Button>((int)Buttons.Other1stCharacterButton).gameObject.EventActive(true);
                    Get<Button>((int)Buttons.Other1stCharacterButton).interactable = true;
                }
                if (inBattle[5].IsLive)
                {
                    Get<Button>((int)Buttons.Other2ndCharacterButton).gameObject.EventActive(true);
                    Get<Button>((int)Buttons.Other2ndCharacterButton).interactable = true;
                }
                if (inBattle[6].IsLive)
                {
                    Get<Button>((int)Buttons.Other3rdCharacterButton).gameObject.EventActive(true);
                    Get<Button>((int)Buttons.Other3rdCharacterButton).interactable = true;
                }
                if (inBattle[7].IsLive)
                {
                    Get<Button>((int)Buttons.Other4thCharacterButton).gameObject.EventActive(true);
                    Get<Button>((int)Buttons.Other4thCharacterButton).interactable = true;
                }

                break;
            case SkillData.Target.MyTeam:
                if (inBattle[0].IsLive)
                {
                    Get<Button>((int)Buttons.My1stCharacterButton).gameObject.EventActive(true);
                    Get<Button>((int)Buttons.My1stCharacterButton).interactable = true;
                }
                if (inBattle[1].IsLive)
                {
                    Get<Button>((int)Buttons.My2ndCharacterButton).gameObject.EventActive(true);
                    Get<Button>((int)Buttons.My2ndCharacterButton).interactable = true;
                }
                if (inBattle[2].IsLive)
                {
                    Get<Button>((int)Buttons.My3rdCharacterButton).gameObject.EventActive(true);
                    Get<Button>((int)Buttons.My3rdCharacterButton).interactable = true;
                }
                if (inBattle[3].IsLive)
                {
                    Get<Button>((int)Buttons.My4thCharacterButton).gameObject.EventActive(true);
                    Get<Button>((int)Buttons.My4thCharacterButton).interactable = true;
                }

                Get<Button>((int)Buttons.Other1stCharacterButton).gameObject.EventActive(false);
                Get<Button>((int)Buttons.Other2ndCharacterButton).gameObject.EventActive(false);
                Get<Button>((int)Buttons.Other3rdCharacterButton).gameObject.EventActive(false);
                Get<Button>((int)Buttons.Other4thCharacterButton).gameObject.EventActive(false);

                Get<Button>((int)Buttons.Other1stCharacterButton).interactable = false;
                Get<Button>((int)Buttons.Other2ndCharacterButton).interactable = false;
                Get<Button>((int)Buttons.Other3rdCharacterButton).interactable = false;
                Get<Button>((int)Buttons.Other4thCharacterButton).interactable = false;
                break;
            case SkillData.Target.None:
                Get<Button>((int)Buttons.My1stCharacterButton).gameObject.EventActive(false);
                Get<Button>((int)Buttons.My2ndCharacterButton).gameObject.EventActive(false);
                Get<Button>((int)Buttons.My3rdCharacterButton).gameObject.EventActive(false);
                Get<Button>((int)Buttons.My4thCharacterButton).gameObject.EventActive(false);

                Get<Button>((int)Buttons.My1stCharacterButton).interactable = false;
                Get<Button>((int)Buttons.My2ndCharacterButton).interactable = false;
                Get<Button>((int)Buttons.My3rdCharacterButton).interactable = false;
                Get<Button>((int)Buttons.My4thCharacterButton).interactable = false;

                Get<Button>((int)Buttons.Other1stCharacterButton).gameObject.EventActive(false);
                Get<Button>((int)Buttons.Other2ndCharacterButton).gameObject.EventActive(false);
                Get<Button>((int)Buttons.Other3rdCharacterButton).gameObject.EventActive(false);
                Get<Button>((int)Buttons.Other4thCharacterButton).gameObject.EventActive(false);

                Get<Button>((int)Buttons.Other1stCharacterButton).interactable = false;
                Get<Button>((int)Buttons.Other2ndCharacterButton).interactable = false;
                Get<Button>((int)Buttons.Other3rdCharacterButton).interactable = false;
                Get<Button>((int)Buttons.Other4thCharacterButton).interactable = false;
                break;
        }
    }

    // 구현부
    private BattleCharacter[] inBattle = new BattleCharacter[8]; // 배틀에 참여중인 캐릭터들의 정보

    private int maxSpeed = 0; // 이번 배틀에서 가장 빠른 속도
    private int maxGauge = 0; // 가장 빠른 속도를 기준으로 행동게이지 최대치를 정함

    private Queue<BattleCharacter> turnQue = new Queue<BattleCharacter>(8);
    private BattleCharacter curTurn = null;

    private int myTeamDeathCount = 0; // 내팀 데스카운트
    private int otherTeamDeathCount = 0; // 상대팀 데스카운트

    private enum GameState
    {
        Start,
        MyTurn,
        OtherTurn,
        InPrograss,
        End,
        MyWin,
        OtherWin,
    }
    private GameState g_state = GameState.Start;

    public void InfomationInit()
    {
        List<int> pickIDs = Manager.Game.GetPickIDs(first);
        for (int i = 0; i < 4; i++)
        {
            inBattle[i] = new BattleCharacter(Manager.Game.CharacterDictionary[pickIDs[i]], BattleCharacter.Team.My);
            Get<TMP_Text>((int)Texts.My1stCharacterButtonText + i).text = inBattle[i].data.charName;
            Slider hpSlider = Get<Slider>((int)Sliders.My1stCharacterHP + i);
            hpSlider.maxValue = inBattle[i].data.hp;
            hpSlider.value = inBattle[i].data.hp;
            int idx = i;
            inBattle[i].onChangeHP += (hp) =>
            {
                hpSlider.value = hp;
                if (hp == 0)
                {
                    ++myTeamDeathCount;
                    Get<Button>((int)Buttons.My1stCharacterButton + idx).interactable = false;
                    Get<Button>((int)Buttons.My1stCharacterButton + idx).gameObject.EventActive(false);

                    if (myTeamDeathCount > 3)
                    {
                        g_state = GameState.OtherWin;
                    }
                }
            };
        }

        pickIDs = Manager.Game.GetPickIDs(first == false);
        for (int i = 0; i < 4; i++)
        {
            inBattle[i + 4] = new BattleCharacter(Manager.Game.CharacterDictionary[pickIDs[i]], BattleCharacter.Team.Other);
            Debug.Log($"상대 {i}번째 캐릭터 이름 : {inBattle[i + 4].data.charName}");
            Get<TMP_Text>((int)Texts.Other1stCharacterButtonText + i).text = inBattle[i + 4].data.charName;
            Slider hpSlider = Get<Slider>((int)Sliders.Other1stCharacterHP + i);
            hpSlider.maxValue = inBattle[i + 4].data.hp;
            hpSlider.value = inBattle[i + 4].data.hp;
            int idx = i;
            inBattle[i + 4].onChangeHP += (hp) =>
            {
                hpSlider.value = hp;
                if (hp == 0)
                {
                    ++otherTeamDeathCount;
                    Get<Button>((int)Buttons.Other1stCharacterButton + idx).interactable = false;
                    Get<Button>((int)Buttons.Other1stCharacterButton + idx).gameObject.EventActive(false);

                    if (otherTeamDeathCount > 3)
                    {
                        g_state = GameState.MyWin;
                    }
                }
            };
        }

        StartBattle();
    }

    private void StartBattle()
    {
        for (int i = 0; i < 8; i++)
        {
            int speed = inBattle[i].curSpeed;
            if (maxSpeed < speed)
            {
                maxSpeed = speed;
            }
        }

        maxGauge = maxSpeed * 10;
        SetMaxGauge(maxGauge);
        curTurn = TakeTurn();
        OnClickFirstSkillButton();
    }

    private void SetMaxGauge(int maxGauge)
    {
        for (int i = 0; i < 8; i++)
        {
            Slider turnGauge = Get<Slider>((int)Sliders.TurnSlider1 + i);
            turnGauge.maxValue = maxGauge;
        }
    }

    private void increaseGauge()
    {
        // 행동게이지가 가득찬 캐릭터가 없으면
        while (turnQue.Count == 0)
        {
            for (int i = 0; i < 8; i++)
            {
                if (inBattle[i].IsLive == false)
                    continue;

                inBattle[i].actionGauge += inBattle[i].curSpeed;

                // 행동게이지가 가득 찼으면
                if (maxGauge <= inBattle[i].actionGauge)
                {
                    turnQue.Enqueue(inBattle[i]);
                }
            }
        }
    }

    // 턴 큐에 있는 캐릭터를 가져온다
    private BattleCharacter TakeTurn()
    {
        if (g_state >= GameState.End) // 전투가 끝난 상태
        {
            Debug.Log(g_state);
            for (int i = 0; i < Enum.GetNames(typeof(Buttons)).Length; i++)
            {
                Get<Button>(i).gameObject.EventActive(false);
            }
            // 스킬 UI 끄기 등

            // 테스트
            Manager.UI.ClosePopupUI(this);
            Manager.UI.ShowPopupUI<UI_LobbyPopup>();
            return null;
        }

        if (turnQue.Count == 0)
        {
            increaseGauge();
        }

        SetActionGaugeUI();
        BattleCharacter curTurn = turnQue.Dequeue();
        for (int i = 0; i < 3; i++)
        {
            int idx = i;
            Get<TMP_Text>((int)Texts.FirstSkillText + i).text = curTurn.data.skills[i].skillName;
        }

        if (curTurn.GetTeam == BattleCharacter.Team.My)
            g_state = GameState.MyTurn;
        else
            g_state = GameState.OtherTurn;

        Get<TMP_Text>((int)Texts.CurrentTurnNameText).text = curTurn.data.charName;
        Get<TMP_Text>((int)Texts.CurrentTurnHPText).text = $"{curTurn.CurHP} / {curTurn.data.hp}";
        Slider curhp = Get<Slider>((int)Sliders.CurTurnHP);
        curhp.maxValue = curTurn.data.hp;
        curhp.value = curTurn.CurHP;

        return curTurn;
    }
}

public class BattleCharacter
{
    public enum Team
    {
        None,
        My,
        Other,
    }

    public CharacterData data;
    private Team team;
    public Team GetTeam { get { return team; } }
    public int curSpeed;
    private int curHP;
    public int CurHP
    {
        get
        {
            return curHP;
        }
        set
        {
            curHP = value;
            if (curHP <= 0)
            {
                curHP = 0;
                isLive = false;
            }
            onChangeHP?.Invoke(curHP);
        }
    }

    private bool isLive;
    public bool IsLive { get { return isLive; } }
    public int actionGauge;
    public int[] skillCool;

    public Action<int> onChangeHP;

    public BattleCharacter(CharacterData data, Team team)
    {
        this.data = data;
        curSpeed = data.speed;
        curHP = data.hp;
        isLive = true;
        actionGauge = 0;
        skillCool = new int[] { 1, 1, 1 }; // 스킬 쿨타임

        this.team = team;
    }
}
