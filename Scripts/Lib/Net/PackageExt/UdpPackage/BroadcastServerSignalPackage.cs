using System;
namespace MTB
{
	public class BroadcastServerSignalPackage : UdpPackage
	{
		public string serverName{get;set;}
		public BroadcastServerSignalPackage (int id)
			:base(id)
		{
		}

		public override void Do ()
		{
			//客户端接收到服务器发送过来的广播信息
			EventManager.SendEvent(NetEventMacro.ON_SEARCH_SERVER_SIGNAL,this.ip.ToString(),NetManager.Instance.server.port);
			UnityEngine.Debug.Log("BroadcastServerSignalPackage");
		}

		public override void ReadAllData ()
		{
			serverName = ReadString();
		}

		public override void WriteAllData ()
		{
			WriteString(serverName);
		}
	}
}

