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
    public event Action<int> OnEnterRoom;
    public event Action OnLeaveRoom;
    public event Action<int> OnSelectPickUp;
    public event Action<int> OnSelectItem;
    public event Action<int> OnSelectBan;
    /// <summary>
    /// first int : charIdx, second int : skillIdx
    /// </summary>
    public event Action<int, int> OnTakeSkill;
    #endregion

    public void Leave(S_BroadcastLeaveGame p)
    {
        Debug.Log($"LeaveGame : {p.playerId}");
    }

    public void Enter(S_BroadcastEnterGame p)
    {
        // 상대 Player ID
        Debug.Log($"Enter Player ID : {p.playerId}");

        OnEnterRoom?.Invoke(p.playerId);
    }

    public void LastBan(S_LastBanPick p)
    {
        Debug.Log($"LastBan : {p.lastBanIdx}");
    }

    public void GetPick(S_PickUp p)
    {
        Debug.Log($"Pick : {p.pickIdx}");

        OnSelectPickUp?.Invoke(p.pickIdx);
    }

    public void BanPick(S_BanPick p)
    {
        Debug.Log($"Ban : {p.banId}");

        // 밴 대신 픽 아이템(확정이 아닐때)로 사용
        OnSelectItem?.Invoke(p.banId);
        OnSelectBan?.Invoke(p.banId);
    }

    public void Attack(S_Attck p)
    {
        Debug.Log($"Attack Index : {p.atckId}, Skill : {p.skillId},Damager {p.damValue}");

        OnTakeSkill?.Invoke(p.atckId, p.skillId);
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
        foreach (var go in player.players)
        {
            Debug.Log($"PlayerList ID: {go.playerId}, Its Mine {go.isSelf}");
        }
    }


    public void LeaveGame(S_BroadcastLeaveGame p)
    {
        Debug.Log("LeaveGame0");
        if (_player.ID == p.playerId)
        {
            Debug.Log("LeaveGame1");
            GameObject.Destroy(_player.gameObject);
            _player = null;
        }
        else
        {
            Debug.Log("LeaveGame2");
            if (_players.TryGetValue(p.playerId, out Player player))
            {
                Debug.Log("LeaveGame3");
                GameObject.Destroy(player.gameObject);
                _players.Remove(p.playerId);
            }
        }

        Debug.Log("LeaveGame4");
    }


}
