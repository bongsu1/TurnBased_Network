using ServerCore;
using System;
using System.Collections.Generic;

public class PacketManager
{
    #region Singleton
    static PacketManager _instance = new();
    public static PacketManager Instance
    {
        get { return _instance; }
    }
    #endregion Singleton
    PacketManager()
    {
        Register();
    }

    Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>> _makeFunc = new();
    Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new();


    public void Register()
    {
       _makeFunc.Add((ushort)PacketID.S_BroadcastEnterGame, MakePacket<S_BroadcastEnterGame>);
        _handler.Add((ushort)PacketID.S_BroadcastEnterGame, PacketHandler.S_BroadcastEnterGameHandler);
       _makeFunc.Add((ushort)PacketID.S_BroadcastLeaveGame, MakePacket<S_BroadcastLeaveGame>);
        _handler.Add((ushort)PacketID.S_BroadcastLeaveGame, PacketHandler.S_BroadcastLeaveGameHandler);
       _makeFunc.Add((ushort)PacketID.S_BroadcastMove, MakePacket<S_BroadcastMove>);
        _handler.Add((ushort)PacketID.S_BroadcastMove, PacketHandler.S_BroadcastMoveHandler);
       _makeFunc.Add((ushort)PacketID.S_PlayerList, MakePacket<S_PlayerList>);
        _handler.Add((ushort)PacketID.S_PlayerList, PacketHandler.S_PlayerListHandler);
       _makeFunc.Add((ushort)PacketID.S_BanPick, MakePacket<S_BanPick>);
        _handler.Add((ushort)PacketID.S_BanPick, PacketHandler.S_BanPickHandler);
       _makeFunc.Add((ushort)PacketID.S_PickUp, MakePacket<S_PickUp>);
        _handler.Add((ushort)PacketID.S_PickUp, PacketHandler.S_PickUpHandler);
       _makeFunc.Add((ushort)PacketID.S_Attck, MakePacket<S_Attck>);
        _handler.Add((ushort)PacketID.S_Attck, PacketHandler.S_AttckHandler);
       _makeFunc.Add((ushort)PacketID.S_Chat, MakePacket<S_Chat>);
        _handler.Add((ushort)PacketID.S_Chat, PacketHandler.S_ChatHandler);
       _makeFunc.Add((ushort)PacketID.S_Result, MakePacket<S_Result>);
        _handler.Add((ushort)PacketID.S_Result, PacketHandler.S_ResultHandler);

    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer, Action<PacketSession, IPacket> onRecvCallback = null)
    {
        int count = 0;
        ushort size = (ushort)BitConverter.ToInt16(buffer.Array, buffer.Offset);
        count += 2;
        ushort id = (ushort)BitConverter.ToInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        Func<PacketSession, ArraySegment<byte>, IPacket> fnc = null;
        if(_makeFunc.TryGetValue(id, out fnc))
        {
            IPacket p = fnc.Invoke(session, buffer);
            if (onRecvCallback != null)
            {
                onRecvCallback.Invoke(session, p);
            }
            else
            {
                HandlePacket(session, p);
            }
        }

    }

    T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {
        T pkt = new T();
        pkt.Read(buffer);

        return pkt;
    }

    public void HandlePacket(PacketSession session, IPacket pkt)
    {
        Action<PacketSession, IPacket> action = null;
        if (_handler.TryGetValue(pkt.Protocol, out action))
        {
            action.Invoke(session, pkt);
        }
    }
}
