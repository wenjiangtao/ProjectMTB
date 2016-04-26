using UnityEngine;
using System.Collections;
using System.IO;
using System.Net;
using System;
using MTB;

public class MoveCommandPackage : NetPackage
{
    public int uid;
    public byte command;
    public Vector2 dir;
    public MoveCommandPackage()
    {
        this.id = 1;
    }
   
    public override void ReadAllData()
    {
        this.uid = this.ReadInt();
        this.command = this.ReadByte();
        this.dir = this.ReadVector2();
    }

    public override void WriteAllData()
    {
        this.bw.Write(this.uid);
        this.bw.Write(command);
        this.bw.Write(this.dir.ToString());
    }
}

public class JumpCommandPackage : NetPackage
{
    public int uid;
    public byte command;

    public JumpCommandPackage()
    {
        this.id = 2;
    }
}

public class ActionCommandPackage : NetPackage
{
    public ActionCommandPackage()
    {
        this.id = 3;
    }
}

public class ExtCommandPackage : NetPackage
{
    public ExtCommandPackage()
    {
        this.id = 4;
    }
}

public class RoomCommandPackage : NetPackage
{
	public int uid{get;set;}
	public RoomCommandPackage()
	{
		this.id = 5;
		this.uid = 0;
	}

	public RoomCommandPackage(int uid)
	{
		this.id = 5;
		this.uid = uid;
	}

	public override void ReadAllData()
	{
		this.uid = this.ReadInt();
	}
	
	public override void WriteAllData()
	{
		this.bw.Write(this.uid);
	}
}

public abstract class NetPackage
{
    protected int length;
    protected int id;
	protected IPAddress ip;
	protected int port;

    protected MemoryStream ms;
    protected BinaryReader br;
    protected BinaryWriter bw;
    protected byte[] readData;
    protected byte[] writeData;

    protected string[] Vector2SplitStr = { "(", ",", ")" };
	protected string[] Vector3SplitStr = {"(",",",",",")"};

    public int GetId()
    {
        return id;
    }

	public NetPackage()
	{
	}

	public NetPackage(int id)
	{
		this.id = id;
	}

    public static NetPackage GetPackageInstance(int id)
    {
        switch (id)
        {
            case 1: return new MoveCommandPackage();
            case 2: return new JumpCommandPackage();
            case 3: return new ActionCommandPackage();
            case 4: return new ExtCommandPackage();
			case 5: return new RoomCommandPackage();
            default: return null;
        }
    }

    public virtual MemoryStream GetMemoryStream()
    {
        return ms;
    }

    public virtual void Deserialize(byte[] data)
    {
        readData = data;
        this.ms = new MemoryStream(this.readData);
        this.br = new BinaryReader(this.ms);
        ReadAllData();
    }

    public virtual void Serialize()
    {
        this.BeforeWriteData();
        this.WriteAllData();
        this.AfterWriteData();
    }

	//获取发送方的ip与端口号
	public virtual void DeserializeRemoteEndPoint(EndPoint endPoint)
	{
		IPEndPoint iep = (IPEndPoint)endPoint;
		ip = iep.Address;
		port = iep.Port;
	}

    public virtual void ReadAllData()
    {
    }

    public virtual void WriteAllData()
    {
    }

    //写入长度 写入ID
    public virtual void BeforeWriteData()
    {
        this.ms = new MemoryStream();
        this.bw = new BinaryWriter(ms);
        int length = 0;
        this.bw.Write(length);
        this.bw.Write(this.id);
    }

    //回到位置0 重新写入长度
    public virtual void AfterWriteData()
    {
        int length = (int)this.ms.Length;
        this.ms.Position = 0;
        this.bw.Write(length);
        this.ms.Position = length;
    }

    public int ReadInt()
    {
        return this.br.ReadInt32();
    }

	public Int16 ReadInt16()
	{
		return this.br.ReadInt16();
	}

    public byte ReadByte()
    {
        return this.br.ReadByte();
    }

	public byte[] ReadBytes()
	{
		int length = ReadInt();
		return this.br.ReadBytes(length);
	}

	public float ReadFloat()
	{
		return this.br.ReadSingle();
	}

	public bool ReadBool()
	{
		byte b = ReadByte();
		return b == 0x01 ? true : false;
	}

    public Vector2 ReadVector2()
    {
		float x = ReadFloat();
		float y = ReadFloat();
        return new Vector2(x, y);
    }

	public Vector3 ReadVector3()
	{
		float x = ReadFloat();
		float y = ReadFloat();
		float z = ReadFloat();
		return new Vector3(x,y,z);
	}

	public string ReadString()
	{
		return this.br.ReadString();
	}

    public void WriteInt(int value)
    {
        this.bw.Write(value);
    }

	public void WriteInt16(Int16 value)
	{
		this.bw.Write(value);
	}

    public void WriteByte(byte value)
    {
        this.bw.Write(value);
    }

	public void WriteFloat(float value)
	{
		this.bw.Write(value);
	}

	public void WriteBytes(byte[] value)
	{
		WriteInt(value.Length);
		this.bw.Write(value);
	}

	public void WriteBool(bool value)
	{
		byte b = value ? (byte)1 : (byte)0;
		WriteByte(b);
	}

    public void WriteVector2(Vector2 vector)
    {
		WriteFloat(vector.x);
		WriteFloat(vector.y);
    }

	public void WriteVector3(Vector3 vector)
	{
		WriteFloat(vector.x);
		WriteFloat(vector.y);
		WriteFloat(vector.z);
	}

	public void WriteString(string str)
	{
		this.bw.Write(str);
	}

    public NetPackage ReadPackage(NetPackage package)
    {
        int length = br.ReadInt32();
        int id = br.ReadInt32();
        byte[] data = br.ReadBytes(length - 8);
        package.Deserialize(data);
        return package;
    }

    public void WritePackage(NetPackage package)
    {
        package.Serialize();
        MemoryStream ms = package.GetMemoryStream();
        byte[] buffer = ms.GetBuffer();
        int length = (int)ms.Length;
        bw.Write(buffer, 0, length);
    }
}
