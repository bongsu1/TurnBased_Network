using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager
{
    public static PlayerManager Instance { get; } = new();

    Mine _player;
    Dictionary<int, Player> _players = new();

    private PlayerManager() { }
    public void Leave(S_BroadcastLeaveGame p)
    {
        Debug.Log($"LeaveGame : {p.playerId}");
    }

    public void Enter(S_BroadcastEnterGame p)
    {
        Debug.Log($"Enter Player ID : {p.playerId}");
    }

    public void LastBan(S_LastBanPick p)
    {
        Debug.Log($"LastBan : {p.lastBanIdx}");
    }

    public void GetPick(S_PickUp p)
    {
        Debug.Log($"Pick : {p.pickIdx}");
    }

    public void BanPick(S_BanPick p)
    {
        Debug.Log($"Ban : {p.banId}");
    }

    public void Attack(S_Attck p)
    {
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
        Object obj = Resources.Load("Player");
        foreach (var go in player.players)
        {
            Debug.Log($"PlayerList ID: {go.playerId}, Its Mine {go.isSelf}");
        }
    }


    public void LeaveGame(S_BroadcastLeaveGame p)
    {
        if(_player.ID == p.playerId)
        {
            GameObject.Destroy(_player.gameObject);
            _player = null;
        }
        else
        {
            if(_players.TryGetValue(p.playerId, out Player player))
            {
                GameObject.Destroy(player.gameObject);
                _players.Remove(p.playerId);
            }
        }
    }


}
