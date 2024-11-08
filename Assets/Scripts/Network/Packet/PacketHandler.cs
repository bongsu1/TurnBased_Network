using DummyClient;
using ServerCore;
using UnityEngine;

internal class PacketHandler
{
    public static void S_ChatHandler(PacketSession session, IPacket packet)
    {
        S_Chat p = packet as S_Chat;
        ServerSession serverSession = session as ServerSession;
        //Console.WriteLine($"{p.playerId} : {p.chat}");
        Debug.Log($"{p.playerId} : {p.chat}");
    }

    public static void S_BanPickHandler(PacketSession session, IPacket packet)
    {
        S_BanPick p = packet as S_BanPick;
        ServerSession serverSession = session as ServerSession;

    }

    public static void S_PickUpHandler(PacketSession session, IPacket packet)
    {
        S_PickUp p = packet as S_PickUp;
        ServerSession serverSession = session as ServerSession;

    }

    public static void S_AttckHandler(PacketSession session, IPacket packet)
    {
        S_Attck p = packet as S_Attck;
        ServerSession serverSession = session as ServerSession;

    }

    public static void S_ResultHandler(PacketSession session, IPacket packet)
    {
        S_Result p = packet as S_Result;
        ServerSession serverSession = session as ServerSession;

    }

}
