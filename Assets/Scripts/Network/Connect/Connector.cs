using System;
using System.Net;
using System.Net.Sockets;


namespace ServerCore
{
    public class Connector
    {
        Func<Session> _sessionFactory;
        //Lobby lobby = new Lobby();

        public void Connect(IPEndPoint endPoint, Func<Session> sessionFactory, NetworkManager.Connect state, int count = 1)
        {
            Socket socket = new Socket(
                Define.AddressType,
                Define.SocketType,
                Define.ProtocolType);

            //socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true); // 재사용 설정 추가
            //socket.ReceiveTimeout = 1000;

            if (state == NetworkManager.Connect.Domain)
            {
                IPEndPoint bindPoint = new IPEndPoint(IPAddress.Any, Define.PortNum);
                socket.Bind(bindPoint);
            }

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.UserToken = socket;
            args.RemoteEndPoint = endPoint;
            args.Completed += OnConnectedCompleted;

            _sessionFactory = sessionFactory;

            RegistConnect(args);
        }

        void RegistConnect(SocketAsyncEventArgs args)
        {
            Socket socket = args.UserToken as Socket;
            if (socket == null)
            {
                Console.WriteLine($"socket invalid exception");
                return;
            }
            bool pending = socket.ConnectAsync(args);
            if (pending == false)
            {
                OnConnectedCompleted(null, args);
            }
        }

        void OnConnectedCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                Console.WriteLine("OnConnectedCompleted");
                //lobby.EnterLobby(args.ConnectSocket, args.RemoteEndPoint);
                Session session = _sessionFactory.Invoke();
                session.Start(args.ConnectSocket);
                session.OnConnected(args.RemoteEndPoint);
            }
            else
            {
                Console.WriteLine("OnConnectedCompleted Failed");
            }
        }
    }

}