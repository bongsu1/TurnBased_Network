using DummyClient;
using ServerCore;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    [SerializeField] Define.Connect connect;
    [SerializeField] string domainName;

    Connector _connect;
    ServerSession _session;

    public void Send(ArraySegment<byte> sendBuff)
    {
        if (SessionState())
        {
            _session.Send(sendBuff);
        }
    }

    IPAddress[] ConnectAddress(Define.Connect connect)
    {
        string str;
        switch (connect)
        {
            case Define.Connect.Local:
                str = Dns.GetHostName();
                break;
            case Define.Connect.Domain:
                if (string.IsNullOrEmpty(domainName))
                { domainName = "pkc-5000.shop"; }
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

                    _connect = new Connector();
                    _session = new ServerSession();

                    _connect.Connect(
                        remoteEndPoint,
                        () => { return _session; },
                        connect
                        );

                }
            }
            if (enter == false)
            {
                Debug.Log("Address Invalid");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private void Update()
    {
        IPacket[] ps = PacketQueue.Instance.PopAll();
        if (ps != null)
        {
            foreach (var p in ps)
            {
                PacketManager.Instance.HandlePacket(_session, p);
            }
        }
    }

    private void OnDestroy()
    {
        _connect.Disconnect();
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
