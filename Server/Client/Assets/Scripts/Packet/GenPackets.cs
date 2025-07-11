using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

public enum PacketId
{
    S_BroadcastEnterGame = 1,
	C_LeaveGame = 2,
	S_BroadcastLeaveGame = 3,
	S_PlayerList = 4,
	C_Move = 5,
	S_BroadcastMove = 6,
	
}

public interface IPacket
{
	ushort Protocol { get; }
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}


public class S_BroadcastEnterGame : IPacket
{
    public int PlayerId;
	public float PosX;
	public float PosY;
	public float PosZ;

    public ushort Protocol { get { return (ushort)PacketId.S_BroadcastEnterGame; } }    

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.PlayerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.PosX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.PosY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.PosZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketId.S_BroadcastEnterGame), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.PlayerId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.PosX), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.PosY), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.PosZ), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
} 

public class C_LeaveGame : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketId.C_LeaveGame; } }    

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        count += sizeof(ushort);
        count += sizeof(ushort);
        
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketId.C_LeaveGame), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
} 

public class S_BroadcastLeaveGame : IPacket
{
    public int PlayerId;

    public ushort Protocol { get { return (ushort)PacketId.S_BroadcastLeaveGame; } }    

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.PlayerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketId.S_BroadcastLeaveGame), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.PlayerId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
} 

public class S_PlayerList : IPacket
{
        
	public class Player
	{
	    public bool IsSelf;
		public int PlayerId;
		public float PosX;
		public float PosY;
		public float PosZ;
	
	    public void Read(ArraySegment<byte> segment, ref ushort count)
	    {
	        this.IsSelf = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
			count += sizeof(bool);
			this.PlayerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			this.PosX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			this.PosY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			this.PosZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
	    }
	
	    public bool Write(ArraySegment<byte> segment, ref ushort count)
	    {
	        bool success = true;
	        Array.Copy(BitConverter.GetBytes(this.IsSelf), 0, segment.Array, segment.Offset + count, sizeof(bool));
			count += sizeof(bool);
			Array.Copy(BitConverter.GetBytes(this.PlayerId), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			Array.Copy(BitConverter.GetBytes(this.PosX), 0, segment.Array, segment.Offset + count, sizeof(float));
			count += sizeof(float);
			Array.Copy(BitConverter.GetBytes(this.PosY), 0, segment.Array, segment.Offset + count, sizeof(float));
			count += sizeof(float);
			Array.Copy(BitConverter.GetBytes(this.PosZ), 0, segment.Array, segment.Offset + count, sizeof(float));
			count += sizeof(float);
	        return success;
	    }
	}
	
	public List<Player> Players = new List<Player>();

    public ushort Protocol { get { return (ushort)PacketId.S_PlayerList; } }    

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.Players.Clear();
		ushort playerCount = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for (int i = 0; i < playerCount; i++)
		{
		    Player player = new Player();
		    player.Read(segment, ref count);
		    this.Players.Add(player);
		}
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketId.S_PlayerList), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)this.Players.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach (Player player in this.Players)
		    player.Write(segment, ref count);

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
} 

public class C_Move : IPacket
{
    public float PosX;
	public float PosY;
	public float PosZ;

    public ushort Protocol { get { return (ushort)PacketId.C_Move; } }    

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.PosX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.PosY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.PosZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketId.C_Move), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.PosX), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.PosY), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.PosZ), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
} 

public class S_BroadcastMove : IPacket
{
    public int PlayerId;
	public float PosX;
	public float PosY;
	public float PosZ;

    public ushort Protocol { get { return (ushort)PacketId.S_BroadcastMove; } }    

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.PlayerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.PosX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.PosY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.PosZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketId.S_BroadcastMove), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.PlayerId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.PosX), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.PosY), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.PosZ), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
} 

