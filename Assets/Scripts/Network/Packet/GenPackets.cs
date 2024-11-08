using ServerCore;
using System;
using System.Text;

public enum PacketID
{
    C_Match = 1,
	C_BanPick = 2,
	C_PickUp = 3,
	C_Attck = 4,
	C_Chat = 5,
	S_BanPick = 6,
	S_PickUp = 7,
	S_Attck = 8,
	S_Chat = 9,
	S_Result = 10,
	
}

public interface IPacket
{
	ushort Protocol { get; }
	void Read(ArraySegment<byte> segement);
	ArraySegment<byte> Write();
}

public class C_Match : IPacket
{
    public short playerId;
	public short targetId;

    public ushort Protocol { get {return (ushort)PacketID.C_Match; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 4;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        this.playerId = BitConverter.ToInt16(s.Slice(count, s.Length - count));
		count += sizeof(short);
		
		this.targetId = BitConverter.ToInt16(s.Slice(count, s.Length - count));
		count += sizeof(short);
		
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_Match);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
		count += sizeof(short);
		
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.targetId);
		count += sizeof(short);
		

        success &= BitConverter.TryWriteBytes(s, count);
        return success ? SendBufferHelper.Close(count) : null;
    }
}
public class C_BanPick : IPacket
{
    public short banId;

    public ushort Protocol { get {return (ushort)PacketID.C_BanPick; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 4;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        this.banId = BitConverter.ToInt16(s.Slice(count, s.Length - count));
		count += sizeof(short);
		
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_BanPick);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.banId);
		count += sizeof(short);
		

        success &= BitConverter.TryWriteBytes(s, count);
        return success ? SendBufferHelper.Close(count) : null;
    }
}
public class C_PickUp : IPacket
{
    public short pickIdx;

    public ushort Protocol { get {return (ushort)PacketID.C_PickUp; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 4;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        this.pickIdx = BitConverter.ToInt16(s.Slice(count, s.Length - count));
		count += sizeof(short);
		
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_PickUp);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.pickIdx);
		count += sizeof(short);
		

        success &= BitConverter.TryWriteBytes(s, count);
        return success ? SendBufferHelper.Close(count) : null;
    }
}
public class C_Attck : IPacket
{
    public short atckId;
	public short skillId;
	public short damValue;

    public ushort Protocol { get {return (ushort)PacketID.C_Attck; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 4;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        this.atckId = BitConverter.ToInt16(s.Slice(count, s.Length - count));
		count += sizeof(short);
		
		this.skillId = BitConverter.ToInt16(s.Slice(count, s.Length - count));
		count += sizeof(short);
		
		this.damValue = BitConverter.ToInt16(s.Slice(count, s.Length - count));
		count += sizeof(short);
		
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_Attck);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.atckId);
		count += sizeof(short);
		
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.skillId);
		count += sizeof(short);
		
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.damValue);
		count += sizeof(short);
		

        success &= BitConverter.TryWriteBytes(s, count);
        return success ? SendBufferHelper.Close(count) : null;
    }
}
public class C_Chat : IPacket
{
    public string chat;

    public ushort Protocol { get {return (ushort)PacketID.C_Chat; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 4;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        ushort chatLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		
		this.chat = Encoding.Unicode.GetString(s.Slice(count, chatLen));
		count += chatLen;
		
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_Chat);
        count += sizeof(ushort);

        ushort chatLen = (ushort)Encoding.Unicode.GetBytes(this.chat, 0, this.chat.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), chatLen);
		count += sizeof(ushort);
		count += chatLen;
		

        success &= BitConverter.TryWriteBytes(s, count);
        return success ? SendBufferHelper.Close(count) : null;
    }
}
public class S_BanPick : IPacket
{
    public short playerId;
	public short banId;

    public ushort Protocol { get {return (ushort)PacketID.S_BanPick; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 4;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        this.playerId = BitConverter.ToInt16(s.Slice(count, s.Length - count));
		count += sizeof(short);
		
		this.banId = BitConverter.ToInt16(s.Slice(count, s.Length - count));
		count += sizeof(short);
		
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_BanPick);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
		count += sizeof(short);
		
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.banId);
		count += sizeof(short);
		

        success &= BitConverter.TryWriteBytes(s, count);
        return success ? SendBufferHelper.Close(count) : null;
    }
}
public class S_PickUp : IPacket
{
    public short playerId;
	public short pickIdx;

    public ushort Protocol { get {return (ushort)PacketID.S_PickUp; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 4;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        this.playerId = BitConverter.ToInt16(s.Slice(count, s.Length - count));
		count += sizeof(short);
		
		this.pickIdx = BitConverter.ToInt16(s.Slice(count, s.Length - count));
		count += sizeof(short);
		
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_PickUp);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
		count += sizeof(short);
		
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.pickIdx);
		count += sizeof(short);
		

        success &= BitConverter.TryWriteBytes(s, count);
        return success ? SendBufferHelper.Close(count) : null;
    }
}
public class S_Attck : IPacket
{
    public short playerId;
	public short atckId;
	public short skillId;
	public short damValue;

    public ushort Protocol { get {return (ushort)PacketID.S_Attck; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 4;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        this.playerId = BitConverter.ToInt16(s.Slice(count, s.Length - count));
		count += sizeof(short);
		
		this.atckId = BitConverter.ToInt16(s.Slice(count, s.Length - count));
		count += sizeof(short);
		
		this.skillId = BitConverter.ToInt16(s.Slice(count, s.Length - count));
		count += sizeof(short);
		
		this.damValue = BitConverter.ToInt16(s.Slice(count, s.Length - count));
		count += sizeof(short);
		
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_Attck);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
		count += sizeof(short);
		
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.atckId);
		count += sizeof(short);
		
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.skillId);
		count += sizeof(short);
		
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.damValue);
		count += sizeof(short);
		

        success &= BitConverter.TryWriteBytes(s, count);
        return success ? SendBufferHelper.Close(count) : null;
    }
}
public class S_Chat : IPacket
{
    public int playerId;
	public string chat;

    public ushort Protocol { get {return (ushort)PacketID.S_Chat; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 4;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        this.playerId = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
		
		ushort chatLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		
		this.chat = Encoding.Unicode.GetString(s.Slice(count, chatLen));
		count += chatLen;
		
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_Chat);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
		count += sizeof(int);
		
		ushort chatLen = (ushort)Encoding.Unicode.GetBytes(this.chat, 0, this.chat.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), chatLen);
		count += sizeof(ushort);
		count += chatLen;
		

        success &= BitConverter.TryWriteBytes(s, count);
        return success ? SendBufferHelper.Close(count) : null;
    }
}
public class S_Result : IPacket
{
    public bool result;

    public ushort Protocol { get {return (ushort)PacketID.S_Result; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 4;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        this.result = BitConverter.ToBoolean(s.Slice(count, s.Length - count));
		count += sizeof(bool);
		
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_Result);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.result);
		count += sizeof(bool);
		

        success &= BitConverter.TryWriteBytes(s, count);
        return success ? SendBufferHelper.Close(count) : null;
    }
}

