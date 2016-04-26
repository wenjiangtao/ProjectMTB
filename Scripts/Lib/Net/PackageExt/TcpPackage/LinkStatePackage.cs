using System;
using UnityEngine;
namespace MTB
{
	//连接服务器发送的连接消息,测试下连通性
	public class LinkStatePackage : TcpPackage
	{

		public LinkStatePackage (int id)
			:base(id)
		{
		}

		public override void ClientDo (ServerConnectionWorker connectionWork)
		{
			EventManager.SendEvent(NetEventMacro.ON_LINK_SERVER);
			UnityEngine.Debug.Log("收到服务器：" + this.ip + ":" + this.port + "的信息，连接请求成功!");
		}

		public override void ServerDo (ClientConnectionWorker connectionWork)
		{
			UnityEngine.Debug.Log("收到客户端:" + this.ip + ":" + this.port + "的连接请求!");
			connectionWork.SendPackage(PackageFactory.GetPackage(this.id));
		}
	}
}

