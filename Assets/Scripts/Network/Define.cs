using System.Net.Sockets;

namespace ServerCore
{
    public class Define
    {
        readonly public static AddressFamily AddressType = AddressFamily.InterNetwork;
        readonly public static SocketType SocketType = SocketType.Stream;
        readonly public static ProtocolType ProtocolType = ProtocolType.Tcp;

        readonly public static int PortNum = 55555;
        readonly public static int SendBufferChunkSize = 6535 * 100;
        public enum PacketID
        {
            PlayerInfoReq = 1,
            PlayerInfoOk = 2,
        }

        public enum ConnectState
        {
            Invaild = 0,
            Connect = 1,
            DisConnect = 2,
        }
        public enum Connect
        {
            Local,
            Domain,
        }

    }
}
