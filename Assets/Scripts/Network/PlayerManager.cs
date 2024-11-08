using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager
{
    public static PlayerManager Instance { get; } = new();

    Mine _player;
    Dictionary<int, Player> _players = new();

    private PlayerManager() { }

    public void Add(S_PlayerList player)
    {
        Object obj = Resources.Load("Player");
        foreach (var go in player.players)
        {
            GameObject ob = Object.Instantiate(obj) as GameObject;
            Player clone;
            if (go.isSelf)
            {
                _player = ob.AddComponent<Mine>();
                clone = _player;
            }
            else
            {
                Player other = ob.AddComponent<Player>();
                clone = other;
                _players.Add(go.playerId, other);
            }
            clone.ID = go.playerId;
        }
    }

    public void EnterGame(S_BroadcastEnterGame p)
    {
        if(p.playerId == _player.ID)
        {
            return;
        }
        Object obj = Resources.Load("Player");
        GameObject ob = Object.Instantiate(obj) as GameObject;

        Player other = ob.AddComponent<Player>();
        _players.Add(p.playerId, other);
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

    public void Move(S_BroadcastMove p)
    {
        Vector3 pos = new(p.posX, p.posY, p.posZ);
        if (_player.ID == p.playerId)
        {
            _player.MoveDir = pos;
        }
        else
        {
            if (_players.TryGetValue(p.playerId, out Player player))
            {
                player.MoveDir = pos;
            }
        }
    }

}
