using System;
namespace MTB
{
	//出主机客户端之外的其他客户端向服务器请求chunk数据
	public class RequestChunkPackage : TcpPackage
	{
		public int roleId{get;set;}
		public WorldPos pos{get;set;}
		public RequestChunkPackage (int id)
			:base(id)
		{

		}

		public override void WriteAllData ()
		{
			WriteInt(roleId);
			WriteInt(pos.x);
			WriteInt(pos.y);
			WriteInt(pos.z);
		}

		public override void ReadAllData ()
		{
			roleId = ReadInt();
			int x = ReadInt();
			int y = ReadInt();
			int z = ReadInt();
			pos = new WorldPos(x,y,z);
		}

		public override void ServerDo (ClientConnectionWorker connectionWork)
		{
			ClientPlayer mainPlayer = NetManager.Instance.server.playerManager.mainPlayer;
			if(mainPlayer.id == roleId)
			{
				ResponseChunkPackage package = PackageFactory.GetPackage(PackageType.ResponseChunk) as ResponseChunkPackage;
				package.roleId = roleId;
				package.pos = pos;
				package.isExit = false;
				ClientChangedChunkData data = NetManager.Instance.server.sceneManager.GetChangedChunkData(pos);
				if(data != null)
				{
					package.hasChangedData = true;
					package.changedData = data;
				}
				mainPlayer.worker.SendPackage(package);
			}
			else
			{
				mainPlayer.worker.SendPackage(this);
			}
		}

		public override void ClientDo (ServerConnectionWorker connectionWork)
		{
			NetChunkData data = new NetChunkData(roleId,pos);
			EventManager.SendEvent(NetEventMacro.ON_NET_CHUNK_GENERATOR,data);
		}
	}
}

