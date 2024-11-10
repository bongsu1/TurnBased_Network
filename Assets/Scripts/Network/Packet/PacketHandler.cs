using DummyClient;
using ServerCore;
using System;
using UnityEngine;

internal class PacketHandler
{
    public static void S_ChatHandler(PacketSession session, IPacket packet)
    {
        S_Chat p = packet as S_Chat;
        ServerSession serverSession = session as ServerSession;
        PlayerManager.Instance.Chat(p);
    }

    public static void S_BroadcastLeaveGameHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastLeaveGame p = packet as S_BroadcastLeaveGame;
        ServerSession serverSession = session as ServerSession;

        PlayerManager.Instance.LeaveGame(p);
    }

    public static void S_BroadcastEnterGameHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastEnterGame p = packet as S_BroadcastEnterGame;
        ServerSession serverSession = session as ServerSession;

        PlayerManager.Instance.Enter(p);
    }

    public static void S_BroadcastMoveHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastMove p = packet as S_BroadcastMove;
        ServerSession serverSession = session as ServerSession;

        //PlayerManager.Instance.Move(p);
    }

    public static void S_PlayerListHandler(PacketSession session, IPacket packet)
    {
        S_PlayerList p = packet as S_PlayerList;
        ServerSession serverSession = session as ServerSession;

        PlayerManager.Instance.PlayerList(p);
    }

    public static void S_BanPickHandler(PacketSession session, IPacket packet)
    {
        S_BanPick p = packet as S_BanPick;
        ServerSession serverSession = session as ServerSession;
        PlayerManager.Instance.BanPick(p);
    }

    public static void S_PickUpHandler(PacketSession session, IPacket packet)
    {
        S_PickUp p = packet as S_PickUp;
        ServerSession serverSession = session as ServerSession;
        PlayerManager.Instance.GetPick(p);
    }

    public static void S_AttckHandler(PacketSession session, IPacket packet)
    {
        S_Attck p = packet as S_Attck;
        ServerSession serverSession = session as ServerSession;
        PlayerManager.Instance.Attack(p);
    }

    public static void S_ResultHandler(PacketSession session, IPacket packet)
    {
        S_Result p = packet as S_Result;
        ServerSession serverSession = session as ServerSession;
        PlayerManager.Instance.Result(p);
    }

}
