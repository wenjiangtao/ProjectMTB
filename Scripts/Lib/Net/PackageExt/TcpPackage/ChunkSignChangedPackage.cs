using System;
namespace MTB
{
	//chunk中sign改变包
	public class ChunkSignChangedPackage : TcpPackage
	{
		public WorldPos pos{get;set;}
		public int sign{get;set;}
		public ChunkSignChangedPackage (int id)
			:base(id)
		{

		}

		public override void WriteAllData ()
		{
			WriteInt(pos.x);
			WriteInt(pos.y);
			WriteInt(pos.z);
			WriteInt(sign);
		}
		
		public override void ReadAllData ()
		{
			int x = ReadInt();
			int y = ReadInt();
			int z = ReadInt();
			pos = new WorldPos(x,y,z);
			sign = ReadInt();
		}
		
		public override void ServerDo (ClientConnectionWorker connectionWork)
		{
			NetManager.Instance.server.sceneManager.ChangedSign(pos,sign);
			//再转发给其他人有这个区块的人
			NetManager.Instance.server.sceneManager.BroadcastPlayerHasChunkPackage(connectionWork.player,pos,this,false);
		}

		public override void ClientDo (ServerConnectionWorker connectionWork)
		{
			Chunk chunk = World.world.GetChunk(pos.x,pos.y,pos.z);
			if(chunk != null)
			{
				chunk.UpdateSign(sign);
			}
		}
	}
}

