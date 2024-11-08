using ServerCore;
using System;
using System.Diagnostics;
using System.Net;
using UnityEngine;

namespace DummyClient
{
    class ServerSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            UnityEngine.Debug.Log($"OnConnected Server : {endPoint}");
        }


        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(
                this, buffer,(session,packet) => { PacketQueue.Instance.Push(packet); });
        }

        public override void OnSend(int numOfByte)
        {
            //C_Chat chat = new C_Chat() { chat = "Hellow" };
            //Send(chat.Write());
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            UnityEngine.Debug.Log($"OnDisconnected : {endPoint}");
            //Console.WriteLine($"OnDisconnected : {endPoint}");
        }

    }
}
