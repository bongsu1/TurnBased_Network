using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketQueue
{
    public static PacketQueue Instance { get; } = new();

    Queue<IPacket> _packetQueue = new();
    object _lock = new();

    public void Push(IPacket packet)
    {
        lock(_lock)
        {
            _packetQueue.Enqueue(packet);
        }
    }

    public IPacket Pop()
    {
        lock(_lock)
        {
            if(_packetQueue.Count == 0)
            {
                return null;
            }
            return _packetQueue.Dequeue();
        }
    }

    public IPacket[] PopAll()
    {
        IPacket[] packets = null;
        lock(_lock)
        {
            if(_packetQueue.Count > 0)
            {
                packets = _packetQueue.ToArray();
                _packetQueue.Clear();
            }
        }
        return packets;
    }
}
