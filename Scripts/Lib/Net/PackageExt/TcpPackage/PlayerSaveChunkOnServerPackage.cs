using System;
namespace MTB
{
	//发送移除chunk信息，并将该数据保存到服务器上
	public class PlayerSaveChunkOnServerPackage : TcpPackage
	{
		public int roleId{get;set;}
		public WorldPos pos{get;set;}
		public byte[] chunkByteData{get;set;}
		public byte compressType{get;set;}
		public PlayerSaveChunkOnServerPackage (int id)
			:base(id)
		{
		}

		public override void WriteAllData ()
		{
			try{
				WriteInt(roleId);
				WriteInt(pos.x);
				WriteInt(pos.y);
				WriteInt(pos.z);
				WriteByte(compressType);
				WriteBytes(chunkByteData);
			}catch(Exception e)
			{
				UnityEngine.Debug.LogError(e.StackTrace);
			}
		}
		
		public override void ReadAllData ()
		{
			try{
				int roleId = ReadInt();
				int x = ReadInt();
				int y = ReadInt();
				int z = ReadInt();
				pos = new WorldPos(x,y,z);
				compressType = ReadByte();
				chunkByteData = ReadBytes();
			}catch(Exception e)
			{
				UnityEngine.Debug.LogError(e.StackTrace);
			}
		}

		public override void ServerDo (ClientConnectionWorker connectionWork)
		{
			NetManager.Instance.server.playerManager.mainPlayer.worker.SendPackage(this);
		}

		public override void ClientDo (ServerConnectionWorker connectionWork)
		{
			NetChunkData netChunkData = new NetChunkData(roleId,pos);
			netChunkData.data.data = chunkByteData;
			netChunkData.data.compressType = (MTBCompressType)compressType;
			EventManager.SendEvent(NetEventMacro.ON_NET_SAVE_CHUNK,netChunkData);
		}
	}
}

