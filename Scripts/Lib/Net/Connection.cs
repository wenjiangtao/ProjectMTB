using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System;

public enum ConnectionState
{

}

public abstract class Connection
{
	public virtual void Listen()
	{
	}

	public virtual Connection Accept()
	{
		return null;
	}

	public virtual void Connect()
	{
	}

	public virtual int Read(byte [] buffer)
	{
		return 0;
	}

	public virtual int Write(byte [] buffer)
	{
		return 0;
	}

	public virtual int Write(byte [] buffer, int offset, int length)
	{
		return 0;
	}

	public abstract EndPoint RemoteEndPoint{get;}

	public abstract EndPoint LocalEndPoint{get;}

	public virtual void Close()
	{

	}

	public virtual bool CanRead()
	{
		return false;
	}

	public virtual bool CanWrite()
	{
		return false;
	}

	public virtual bool HasError()
	{
		return false;
	}
}

public class TcpConnection : Connection
{
	Socket socket; 
	string ip;
	int port;
	bool closed = false;
	bool isServer;
	public static long IpToInt(string ip)
	{
		char[] separator = new char[] { '.' };
		string[] items = ip.Split(separator);
		return long.Parse(items[0]) << 24
			| long.Parse(items[1]) << 16
				| long.Parse(items[2]) << 8 
				| long.Parse(items[3]);
	}

	public TcpConnection(string ip, int port, bool isServer)
	{
		this.ip = ip;
		this.port = port;
		this.isServer = isServer;
		this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		if (isServer) {
			this.socket.Bind (new System.Net.IPEndPoint (TcpConnection.IpToInt (this.ip), port));
			this.socket.Listen (10);
		}
	}

	public TcpConnection(Socket socket)
	{
		this.socket = socket;
	}

	public override EndPoint RemoteEndPoint {
		get {
			return socket.RemoteEndPoint;
		}
	}

	public override EndPoint LocalEndPoint {
		get {
			return socket.LocalEndPoint;
		}
	}


	public override Connection Accept()
	{
		return new TcpConnection (this.socket.Accept ());
	}

	public override void Connect()
	{
		this.socket.Connect (this.ip, this.port);
	}

	public override int Read(byte[] buffer)
	{
		try
		{
			return this.socket.Receive(buffer);
		}
		catch (ObjectDisposedException e){
			this.closed = true;
			return -1;
		}
		catch (SocketException e)
		{
			this.closed = true;
			return -1;
		}
	}
	

	public override int Write(byte[] buffer)
	{
		try
		{
			return this.socket.Send(buffer);
		}
		catch (ObjectDisposedException e){
			//Debug.Log("HasError~~~~~~~~~~~~~~~~~Object");
			this.closed = true;
			return -1;
		}
		catch (SocketException e)
		{
			//Debug.Log("HasError~~~~~~~~~~~~~~~~~Exception");
			this.closed = true;
			return -1;
		}
	}

	public override int Write(byte[] buffer, int offset, int length)
	{
		try{
			return this.socket.Send (buffer, offset, length, SocketFlags.None);
		}
		catch (ObjectDisposedException e){
			//Debug.Log("HasError~~~~~~~~~~~~~~~~~Object");
			this.closed = true;
			return -1;
		}
		catch (SocketException e)
		{
			//Debug.Log("HasError~~~~~~~~~~~~~~~~~Exception");
			this.closed = true;
			return -1;
		}
	}

	public override void Close()
	{
		try{
			if(!isServer)
				this.socket.Shutdown (SocketShutdown.Both);
		}catch(SocketException e)
		{
			UnityEngine.Debug.LogError("[ignore]"+e.Message + "\n" + e.StackTrace);
		}
		finally
		{
			this.socket.Close();
		}
	}

	public override bool CanRead()
	{
		try{
		bool res = this.socket.Poll(5, SelectMode.SelectRead);
		return res;
		}catch(Exception e)
		{
			Debug.LogError(e.StackTrace);
			return false;
		}
	}
	
	public override bool CanWrite()
	{
		bool res = this.socket.Poll (5, SelectMode.SelectWrite);
		return res;
	}
	
	public override bool HasError()
	{
		if (closed) {

			return true;
		}
		return false;
	}
}

public class UdpConnection : Connection
{
	private Socket socket;
	private int port;
	private bool closed = false;
	private EndPoint ep;
	public UdpConnection(int port)
	{
		this.port = port;
		socket = new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);
		socket.SetSocketOption(SocketOptionLevel.Socket,SocketOptionName.Broadcast,1);
		ep = new IPEndPoint(IPAddress.Broadcast,port);
		socket.Bind(new IPEndPoint(IPAddress.Any,port));
	}

	public override EndPoint RemoteEndPoint {
		get {
			return ep;
		}
	}

	public override EndPoint LocalEndPoint {
		get {
			return socket.LocalEndPoint;
		}
	}

	public override int Read(byte[] buffer)
	{
		try
		{
			return this.socket.ReceiveFrom(buffer,ref ep);
		}
		catch (ObjectDisposedException e){
			this.closed = true;
			return -1;
		}
		catch (SocketException e)
		{
			this.closed = true;
			return -1;
		}
	}
	
	public override int Write(byte[] buffer)
	{
		try
		{
			return this.socket.SendTo(buffer,ep);
		}
		catch (ObjectDisposedException e){
			//Debug.Log("HasError~~~~~~~~~~~~~~~~~Object");
			this.closed = true;
			return -1;
		}
		catch (SocketException e)
		{
			//Debug.Log("HasError~~~~~~~~~~~~~~~~~Exception");
			this.closed = true;
			return -1;
		}
	}
	
	public override int Write(byte[] buffer, int offset, int length)
	{
		try{
			return this.socket.SendTo(buffer, offset, length, SocketFlags.None,ep);
		}
		catch (ObjectDisposedException e){
			//Debug.Log("HasError~~~~~~~~~~~~~~~~~Object");
			this.closed = true;
			return -1;
		}
		catch (SocketException e)
		{
			//Debug.Log("HasError~~~~~~~~~~~~~~~~~Exception");
			this.closed = true;
			return -1;
		}
	}
	
	public override void Close()
	{
		this.socket.Close();
	}
	
	public override bool CanRead()
	{
		return this.socket.Poll(5, SelectMode.SelectRead);
	}
	
	public override bool CanWrite()
	{
		return this.socket.Poll (5, SelectMode.SelectWrite);
	}
	
	public override bool HasError()
	{
		return closed;
	}
}
