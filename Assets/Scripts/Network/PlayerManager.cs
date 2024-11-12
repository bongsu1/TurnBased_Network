using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager
{
    public static PlayerManager Instance { get; } = new();
    private PlayerManager() { }

    Mine _player;
    Dictionary<int, Player> _players = new();

    #region event
    public event Action<int> OnSelectPickUp;
    public event Action<int> OnSelectItem;
    public event Action<int> OnSelectBan;
    /// <summary>
    /// first int : charIdx, second int : skillIdx
    /// </summary>
    public event Action<int, int> OnTakeSkill;

    public void ClearSelectEvent()
    {
        OnSelectPickUp = null;
        OnSelectItem = null;
    }

    public void ClearBanEvent()
    {
        OnSelectBan = null;
    }

    public void ClearSkillEvent()
    {
        OnTakeSkill = null;
    }
    #endregion

    public void Leave(S_BroadcastLeaveGame p)
    {
        Debug.Log($"LeaveGame : {p.playerId}");
    }

    public void Enter(S_BroadcastEnterGame p)
    {
        Debug.Log($"Enter Player ID : {p.playerId}");

        #region 매칭완료
        Manager.Game.IsFirst = p.playerId == 0;
        Manager.UI.ClearPopupUI();
        Manager.UI.ShowPopupUI<UI_BanPickPopup>();
        #endregion
    }

    public void LastBan(S_LastBanPick p)
    {
        Debug.Log($"LastBan : {p.lastBanIdx}");
    }

    public void GetPick(S_PickUp p)
    {
        OnSelectPickUp?.Invoke(p.pickIdx);
        Debug.Log($"Pick : {p.pickIdx}");
    }

    public void BanPick(S_BanPick p)
    {
        // 밴 대신 픽 아이템(확정이 아닐때)로 사용
        OnSelectItem?.Invoke(p.banId);
        OnSelectBan?.Invoke(p.banId);
        Debug.Log($"Ban : {p.banId}");
    }

    public void Attack(S_Attck p)
    {
        OnTakeSkill?.Invoke(p.atckId, p.skillId);
        Debug.Log($"Attack Index : {p.atckId}, Skill : {p.skillId},Damager {p.damValue}");
    }

    public void Result(S_Result p)
    {
        Debug.Log($"EndGame");
    }


    public void Chat(S_Chat p)
    {
        Debug.Log($"{p.playerId} : {p.chat}");
    }

    public void PlayerList(S_PlayerList player)
    {
        // 이거 뭐임?
        //UnityEngine.Object obj = Resources.Load("Player");
        foreach (var go in player.players)
        {
            Debug.Log($"PlayerList ID: {go.playerId}, Its Mine {go.isSelf}");
        }
    }


    public void LeaveGame(S_BroadcastLeaveGame p)
    {
        if (_player.ID == p.playerId)
        {
            GameObject.Destroy(_player.gameObject);
            _player = null;
        }
        else
        {
            if (_players.TryGetValue(p.playerId, out Player player))
            {
                GameObject.Destroy(player.gameObject);
                _players.Remove(p.playerId);
            }
        }
    }


}
