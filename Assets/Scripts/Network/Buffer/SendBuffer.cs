using System;
using System.Threading;

namespace ServerCore
{
    public class SendBufferHelper
    {
        /// <summary>
        /// 전역이긴 하나 해당 스레드에서만 사용할 수 있는 고유한 전역 메소드인 ThreadLocal로 사용.
        /// </summary>
        public static ThreadLocal<SendBuffer> CurrentBuffer =
            new ThreadLocal<SendBuffer>(() => { return null; });


        //public static int ChunkSize { get; set; } = Define.SendBufferChunkSize;

        /// <summary>
        /// 센드 버퍼가 없을 경우 생성 
        /// </summary>
        /// <param name="reserveSize"> 전송 예정 크기 </param>
        /// <returns></returns>
        public static ArraySegment<byte> Open(int reserveSize)
        {
            if (CurrentBuffer.Value == null)
            {
                CurrentBuffer.Value = new SendBuffer(Define.SendBufferChunkSize);
            }
            if (CurrentBuffer.Value.FreeSize < reserveSize)
            {
                CurrentBuffer.Value = new SendBuffer(Define.SendBufferChunkSize);
            }

            return CurrentBuffer.Value.Open(reserveSize);
        }

        /// <summary>
        /// 센드 버퍼 전송 준비 완료
        /// </summary>
        /// <param name="usedSize"> 
        /// 전송 확정 크기
        /// </param>
        public static ArraySegment<byte> Close(int usedSize)
        {
            return CurrentBuffer.Value.Close(usedSize);
        }
    }

    /// <summary>
    /// 한번 사용하고 버리는 전송용 버퍼
    /// 1.한명이 아닌 여러명에게 보내는데 사용 
    /// 2.다른 세션에서 전송을 위해 큐에 넣어서 순차 전송 예정 
    /// 3.재사용이 애매하여 일회용으로 사용
    /// </summary>
    public class SendBuffer
    {
        byte[] _buffer;
        int _usedSize = 0;

        public int FreeSize
        {
            get { return _buffer.Length - _usedSize; }
        }
        public SendBuffer(int chunkSize)
        {
            _buffer = new byte[chunkSize];
        }
        public ArraySegment<byte> Open(int reserveSize)
        {
            if (reserveSize > FreeSize)
            {
                return null;
            }

            return new ArraySegment<byte>(_buffer, _usedSize, reserveSize);
        }

        public ArraySegment<byte> Close(int usedSize)
        {
            ArraySegment<byte> segment = new ArraySegment<byte>(_buffer, _usedSize, usedSize);
            _usedSize += usedSize;
            return segment;
        }


    }
}
