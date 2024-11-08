using DummyClient;
using ServerCore;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public enum Connect
    {
        Local,
        Domain,
    }
    ServerSession _session;
    [SerializeField] Connect connect;
    [SerializeField] string domainName;

    IPAddress[] ConnectAddress(Connect connect)
    {
        string str;
        switch (connect)
        {
            case Connect.Local:
                str = Dns.GetHostName();
                break;
            case Connect.Domain:
                if (string.IsNullOrEmpty(domainName))
                {   domainName = "pkc-5000.shop";   }
                str = domainName;
                break;
            default:
                str = "pkc-5000.shop";
                break;
        }
        return Dns.GetHostAddresses(str);
    }

    void Start()
    {
        IPAddress[] addresses = ConnectAddress(connect);

        bool enter = false;
        try
        {
            foreach (var address in addresses)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    enter = true;
                    IPEndPoint remoteEndPoint = new IPEndPoint(address, ServerCore.Define.PortNum);
                    Debug.Log($"[RemoteAddress] : {remoteEndPoint} ");

                    Connector connector = new Connector();
                    _session = new ServerSession();

                    connector.Connect(
                        remoteEndPoint,
                        () => { return _session; },
                        connect
                        );

                    StartCoroutine(CoSendPacket());

                }
            }
            if(enter == false)
            {
                Debug.Log("??????");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private void Update()
    {
        IPacket p = PacketQueue.Instance.Pop();
        if (p != null)
        {
            PacketManager.Instance.HandlePacket(_session, p);
        }

    }

    IEnumerator CoSendPacket()
    {
        while(true)
        {
            yield return new WaitForSeconds(3);
            if (SessionState())
            {
                C_Chat chat = new C_Chat() { chat = "Hellow" };
                ArraySegment<byte> segment = chat.Write();
                _session.Send(segment);
            }
        }
    }



    private void OnDestroy()
    {
        if(SessionState())
        {
            //_session.Disconnect();
        }
    }

    private bool SessionState()
    {
        if (_session == null)
        {
            Debug.Log("Session Is null");
            return false;
        }
        if (_session.State != Define.ConnectState.Connect)
        {
            Debug.Log($"Session Is {_session.State}");

            return false;
        }
        return true;
    }
}
