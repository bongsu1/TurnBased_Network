using Firebase.Database;
using Firebase.Extensions;
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
        MatchQuitButton,
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
        My1stCharacterButton,
        My2ndCharacterButton,
        My3rdCharacterButton,
        My4thCharacterButton,
        Other1stCharacterButton,
        Other2ndCharacterButton,
        Other3rdCharacterButton,
        Other4thCharacterButton,
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
        Get<Button>((int)Buttons.SecondSkillButton).gameObject.BindEvent(OnClickSecondSkillButton);
        Get<Button>((int)Buttons.ThirdSkillButton).gameObject.BindEvent(OnClickThirdSkillButton);

        ActiveCharacterButton(SkillData.Target.None);
        ActiveSelectImage();
        ActiveSkillButton(false);

        first = Manager.Game.IsFirst;

        for (int i = 0; i < 4; i++)
        {
            int myIdx = first ? i : i + 4;
            int otherIdx = first ? i + 4 : i;

            Get<Button>((int)Buttons.My1stCharacterButton + i)
                .gameObject.BindEvent(() => OnClickCharButton(myIdx));
            Get<Button>((int)Buttons.Other1stCharacterButton + i)
                .gameObject.BindEvent(() => OnClickCharButton(otherIdx));
        }

        InfomationInit(first);

        PlayerManager.Instance.OnTakeSkill -= ActionTurn;
        PlayerManager.Instance.OnTakeSkill += ActionTurn;

        Get<Button>((int)Buttons.MatchQuitButton).gameObject.BindEvent(OnClickMatchQuitButton);

        return true;
    }

    private void OnClickFirstSkillButton()
    {
        if (g_state != GameState.MyTurn)
            return;

        ActiveCharacterButton(curTurn.data.skills[0].target);
        selectSkill = 0;
        ActiveSelectImage();
    }

    private void OnClickSecondSkillButton()
    {
        if (g_state != GameState.MyTurn)
            return;

        ActiveCharacterButton(curTurn.data.skills[1].target);
        selectSkill = 1;
        ActiveSelectImage();
    }

    private void OnClickThirdSkillButton()
    {
        if (g_state != GameState.MyTurn)
            return;

        ActiveCharacterButton(curTurn.data.skills[2].target);
        selectSkill = 2;
        ActiveSelectImage();
    }

    private void OnClickCharButton(int selectCharacter)
    {
        // 두번이상 클릭 방지
        ActiveCharacterButton(SkillData.Target.None);

        ActionTurn(selectCharacter, selectSkill);

        C_Attck attack = new C_Attck { atckId = (short)selectCharacter, skillId = (short)selectSkill };
        Manager.Network.Send(attack.Write());
    }

    private void OnClickMatchQuitButton()
    {
        Manager.UI.ClosePopupUI(this);
        Manager.UI.ShowPopupUI<UI_LobbyPopup>();

        Manager.Network.Disconnect();
    }

    /// <summary>
    /// 행동게이지 UI 세팅
    /// </summary>
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

    /// <summary>
    /// 선택한 스킬을 나타내는 UI
    /// </summary>
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
                for (int i = 0; i < 4; i++)
                {
                    Get<Button>((int)Buttons.My1stCharacterButton + i).gameObject.EventActive(false);
                    Get<Button>((int)Buttons.My1stCharacterButton + i).interactable = false;

                    bool isLive = first ? inBattle[i + 4].IsLive : inBattle[i].IsLive;
                    Get<Button>((int)Buttons.Other1stCharacterButton + i).interactable = isLive;
                    Get<Button>((int)Buttons.Other1stCharacterButton + i).gameObject.EventActive(isLive);
                }
                break;
            case SkillData.Target.MyTeam:
                for (int i = 0; i < 4; i++)
                {
                    bool isLive = first ? inBattle[i].IsLive : inBattle[i + 4].IsLive;
                    Get<Button>((int)Buttons.My1stCharacterButton + i).gameObject.EventActive(isLive);
                    Get<Button>((int)Buttons.My1stCharacterButton + i).interactable = isLive;

                    Get<Button>((int)Buttons.Other1stCharacterButton + i).interactable = false;
                    Get<Button>((int)Buttons.Other1stCharacterButton + i).gameObject.EventActive(false);
                }
                break;
            case SkillData.Target.None:
                for (int i = 0; i < 4; i++)
                {
                    Get<Button>((int)Buttons.My1stCharacterButton + i).gameObject.EventActive(false);
                    Get<Button>((int)Buttons.My1stCharacterButton + i).interactable = false;

                    Get<Button>((int)Buttons.Other1stCharacterButton + i).interactable = false;
                    Get<Button>((int)Buttons.Other1stCharacterButton + i).gameObject.EventActive(false);
                }
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

    public void InfomationInit(bool first)
    {
        int firstTextIdx = first ? (int)Texts.My1stCharacterButtonText : (int)Texts.Other1stCharacterButtonText;
        int firstSliderIdx = first ? (int)Sliders.My1stCharacterHP : (int)Sliders.Other1stCharacterHP;
        int firstButtonIdx = first ? (int)Buttons.My1stCharacterButton : (int)Buttons.Other1stCharacterButton;
        int firstImageIdx = first ? (int)Images.My1stCharacterButton : (int)Images.Other1stCharacterButton;
        BattleCharacter.Team firstTeam = first ? BattleCharacter.Team.My : BattleCharacter.Team.Other;
        GameState secondWin = first ? GameState.OtherWin : GameState.MyWin;

        List<int> pickIDs = Manager.Game.GetPickIDs(true);
        for (int i = 0; i < 4; i++)
        {
            inBattle[i] = new BattleCharacter(Manager.Game.CharacterDictionary[pickIDs[i]], firstTeam);
            Get<TMP_Text>(firstTextIdx + i).text = inBattle[i].data.charName;
            Get<Image>(firstImageIdx + i).sprite =
                first ? inBattle[i].data.backImage : inBattle[i].data.frontImage;

            Slider hpSlider = Get<Slider>(firstSliderIdx + i);
            hpSlider.maxValue = inBattle[i].data.hp;
            hpSlider.value = inBattle[i].data.hp;
            int idx = i;
            inBattle[i].onChangeHP += (hp) =>
            {
                hpSlider.value = hp;
                if (hp == 0)
                {
                    ++myTeamDeathCount;
                    Get<Button>(firstButtonIdx + idx).interactable = false;
                    Get<Button>(firstButtonIdx + idx).gameObject.EventActive(false);

                    if (myTeamDeathCount > 3)
                    {
                        g_state = secondWin;
                    }
                }
            };
        }

        int secondTextIdx = first ? (int)Texts.Other1stCharacterButtonText : (int)Texts.My1stCharacterButtonText;
        int secondSliderIdx = first ? (int)Sliders.Other1stCharacterHP : (int)Sliders.My1stCharacterHP;
        int secondButtonIdx = first ? (int)Buttons.Other1stCharacterButton : (int)Buttons.My1stCharacterButton;
        int secondImageIdx = first ? (int)Images.Other1stCharacterButton : (int)Images.My1stCharacterButton;
        BattleCharacter.Team secondTeam = first ? BattleCharacter.Team.Other : BattleCharacter.Team.My;
        GameState firstWin = first ? GameState.MyWin : GameState.OtherWin;

        pickIDs = Manager.Game.GetPickIDs(false);
        for (int i = 0; i < 4; i++)
        {
            inBattle[i + 4] = new BattleCharacter(Manager.Game.CharacterDictionary[pickIDs[i]], secondTeam);
            Get<TMP_Text>(secondTextIdx + i).text = inBattle[i + 4].data.charName;
            Get<Image>(secondImageIdx + i).sprite =
                first ? inBattle[i + 4].data.frontImage : inBattle[i + 4].data.backImage;

            Slider hpSlider = Get<Slider>(secondSliderIdx + i);
            hpSlider.maxValue = inBattle[i + 4].data.hp;
            hpSlider.value = inBattle[i + 4].data.hp;
            int idx = i;
            inBattle[i + 4].onChangeHP += (hp) =>
            {
                hpSlider.value = hp;
                if (hp == 0)
                {
                    ++otherTeamDeathCount;
                    Get<Button>(secondButtonIdx + idx).interactable = false;
                    Get<Button>(secondButtonIdx + idx).gameObject.EventActive(false);

                    if (otherTeamDeathCount > 3)
                    {
                        g_state = firstWin;
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
        ActiveSkillButton(g_state == GameState.MyTurn);
    }

    private void SetMaxGauge(int maxGauge)
    {
        for (int i = 0; i < 8; i++)
        {
            Get<Slider>((int)Sliders.TurnSlider1 + i).maxValue = maxGauge;
            Get<Image>((int)Images.TurnHandle1 + i).sprite = inBattle[i].data.frontImage;
        }
    }

    private void increaseGauge()
    {
        // 행동게이지가 가득찬 캐릭터가 없으면 계속
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
            for (int i = 0; i < Enum.GetNames(typeof(Buttons)).Length; i++)
            {
                Get<Button>(i).gameObject.EventActive(false);
            }

            ActiveSkillButton(false);

            Manager.Network.Disconnect();

            // 전투 정보 초기화
            Manager.Game.SetPickList(null, null);
            PlayerManager.Instance.OnTakeSkill -= ActionTurn;

            RecordBattle();
            StartCoroutine(EndRoutine());
            return null;
        }

        if (turnQue.Count == 0)
        {
            increaseGauge();
        }

        SetActionGaugeUI();
        BattleCharacter curTurn = turnQue.Dequeue();

        // 지금 턴을 잡은 캐릭터가 이미 전투불능이라면
        if (curTurn.IsLive == false)
            return TakeTurn();

        for (int i = 0; i < 3; i++)
        {
            int idx = i;
            Get<TMP_Text>((int)Texts.FirstSkillText + i).text = curTurn.data.skills[i].skillName;
        }


        Get<TMP_Text>((int)Texts.CurrentTurnNameText).text = curTurn.data.charName;
        Get<TMP_Text>((int)Texts.CurrentTurnHPText).text = $"{curTurn.CurHP} / {curTurn.data.hp}";
        Get<Image>((int)Images.CurrentTurnImage).sprite = curTurn.data.frontImage;
        Slider curhp = Get<Slider>((int)Sliders.CurTurnHP);
        curhp.maxValue = curTurn.data.hp;
        curhp.value = curTurn.CurHP;

        if (curTurn.GetTeam == BattleCharacter.Team.My)
            g_state = GameState.MyTurn;
        else
            g_state = GameState.OtherTurn;

        return curTurn;
    }

    // 전투 기록
    private void RecordBattle()
    {
        int rankPoint = Manager.Game.MyInfo.rankPoint;
        int[] log = Manager.Game.MyInfo.log;
        if (g_state == GameState.MyWin)
        {
            rankPoint += 10;
            ++log[0];
        }
        else if (g_state == GameState.OtherWin)
        {
            rankPoint -= 10;
            // 랭크포인트는 0 이하로 내려가지 않음
            if (rankPoint <= 0)
            {
                rankPoint = 0;
            }
            ++log[1];
        }

        var send = new Dictionary<string, object> { { "rankPoint", rankPoint }, { "log", log } };
        Manager.Data.DB.GetReference(UserInfo.Root).Child(Manager.Data.Auth.CurrentUser.UserId)
            .UpdateChildrenAsync(send);
    }

    // 테스트용
    private IEnumerator EndRoutine()
    {
        yield return new WaitForSeconds(2);
        Manager.UI.ClosePopupUI(this);
        Manager.UI.ShowPopupUI<UI_LobbyPopup>();
    }

    private void ActiveSkillButton(bool active)
    {
        Get<Button>((int)Buttons.FirstSkillButton).gameObject.SetActive(active);
        Get<Button>((int)Buttons.SecondSkillButton).gameObject.SetActive(active);
        Get<Button>((int)Buttons.ThirdSkillButton).gameObject.SetActive(active);

        if (active)
        {
            OnClickFirstSkillButton();
        }
    }

    private void ActionTurn(int selectCharacter, int selectSkill)
    {
        inBattle[selectCharacter].CurHP -= curTurn.data.skills[selectSkill].damage;

        int curIdx = Array.IndexOf(inBattle, curTurn);
        inBattle[curIdx].actionGauge = 0;

        curTurn = TakeTurn();
        if (curTurn == null)
            return;

        ActiveSkillButton(g_state == GameState.MyTurn);
    }

    private void OnDestroy()
    {
        PlayerManager.Instance.OnTakeSkill -= ActionTurn;
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
