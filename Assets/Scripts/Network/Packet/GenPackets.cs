
using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

public enum PacketID
{
    C_BanPick = 1,
	C_PickUp = 2,
	C_Attck = 3,
	C_Chat = 4,
	C_LeaveGame = 5,
	C_EndGame = 6,
	C_Move = 7,
	S_BroadcastEnterGame = 8,
	S_BroadcastLeaveGame = 9,
	S_BroadcastMove = 10,
	S_PlayerList = 11,
	S_BanPick = 12,
	S_PickUp = 13,
	S_Attck = 14,
	S_Chat = 15,
	S_Result = 16,
	
}

public interface IPacket
{
	ushort Protocol { get; }
	void Read(ArraySegment<byte> segement);
	ArraySegment<byte> Write();
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

public class C_LeaveGame : IPacket
{
    public short playerId;

    public ushort Protocol { get {return (ushort)PacketID.C_LeaveGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 4;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        this.playerId = BitConverter.ToInt16(s.Slice(count, s.Length - count));
		count += sizeof(short);
		
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_LeaveGame);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
		count += sizeof(short);
		

        success &= BitConverter.TryWriteBytes(s, count);
        return success ? SendBufferHelper.Close(count) : null;
    }
}

public class C_EndGame : IPacket
{
    public bool endGameState;

    public ushort Protocol { get {return (ushort)PacketID.C_EndGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 4;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        this.endGameState = BitConverter.ToBoolean(s.Slice(count, s.Length - count));
		count += sizeof(bool);
		
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_EndGame);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.endGameState);
		count += sizeof(bool);
		

        success &= BitConverter.TryWriteBytes(s, count);
        return success ? SendBufferHelper.Close(count) : null;
    }
}

public class C_Move : IPacket
{
    public float posX;
	public float posY;
	public float posZ;

    public ushort Protocol { get {return (ushort)PacketID.C_Move; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 4;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        this.posX = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
		
		this.posY = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
		
		this.posZ = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
		
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_Move);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.posX);
		count += sizeof(float);
		
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.posY);
		count += sizeof(float);
		
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.posZ);
		count += sizeof(float);
		

        success &= BitConverter.TryWriteBytes(s, count);
        return success ? SendBufferHelper.Close(count) : null;
    }
}

public class S_BroadcastEnterGame : IPacket
{
    public short playerId;

    public ushort Protocol { get {return (ushort)PacketID.S_BroadcastEnterGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 4;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        this.playerId = BitConverter.ToInt16(s.Slice(count, s.Length - count));
		count += sizeof(short);
		
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_BroadcastEnterGame);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
		count += sizeof(short);
		

        success &= BitConverter.TryWriteBytes(s, count);
        return success ? SendBufferHelper.Close(count) : null;
    }
}

public class S_BroadcastLeaveGame : IPacket
{
    public short playerId;
	public short isMine;

    public ushort Protocol { get {return (ushort)PacketID.S_BroadcastLeaveGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 4;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        this.playerId = BitConverter.ToInt16(s.Slice(count, s.Length - count));
		count += sizeof(short);
		
		this.isMine = BitConverter.ToInt16(s.Slice(count, s.Length - count));
		count += sizeof(short);
		
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_BroadcastLeaveGame);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
		count += sizeof(short);
		
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.isMine);
		count += sizeof(short);
		

        success &= BitConverter.TryWriteBytes(s, count);
        return success ? SendBufferHelper.Close(count) : null;
    }
}

public class S_BroadcastMove : IPacket
{
    public short playerId;

    public ushort Protocol { get {return (ushort)PacketID.S_BroadcastMove; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 4;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        this.playerId = BitConverter.ToInt16(s.Slice(count, s.Length - count));
		count += sizeof(short);
		
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_BroadcastMove);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
		count += sizeof(short);
		

        success &= BitConverter.TryWriteBytes(s, count);
        return success ? SendBufferHelper.Close(count) : null;
    }
}

public class S_PlayerList : IPacket
{
    
	#region Player
	public struct Player
	{
	    public bool isSelf;
		public short playerId;
	
	    public void Read(ReadOnlySpan<byte> s, ref ushort count)
	    {
	        this.isSelf = BitConverter.ToBoolean(s.Slice(count, s.Length - count));
			count += sizeof(bool);
			
			this.playerId = BitConverter.ToInt16(s.Slice(count, s.Length - count));
			count += sizeof(short);
			
	    }
	
	    public bool Write(Span<byte> s, ref ushort count)
	    {
	        bool success = true;
	        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.isSelf);
			count += sizeof(bool);
			
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
			count += sizeof(short);
			
	        return success;
	    }
	}
	#endregion Player
	public List<Player> players = new List<Player>();
	

    public ushort Protocol { get {return (ushort)PacketID.S_PlayerList; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 4;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        
		this.players.Clear();
		ushort playerLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		for (int i = 0; i < playerLen; i++)
		{
		    Player player = new Player();
		    player.Read(s, ref count);
		    players.Add(player);
		}
		
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_PlayerList);
        count += sizeof(ushort);

        
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)players.Count);
		count += sizeof(ushort);
		
		foreach (Player player in players)
		{
		    success &= player.Write(s, ref count);
		}
		

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

