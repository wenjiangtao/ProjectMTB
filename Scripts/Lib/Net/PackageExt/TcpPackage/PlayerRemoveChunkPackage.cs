using System;
namespace MTB
{
	public class PlayerRemoveChunkPackage : TcpPackage
	{
		public WorldPos pos{get;set;}
		public PlayerRemoveChunkPackage (int id)
			:base(id)
		{
		}

		public override void WriteAllData ()
		{
			WriteInt(pos.x);
			WriteInt(pos.y);
			WriteInt(pos.z);
		}

		public override void ReadAllData ()
		{
			int x = ReadInt();
			int y = ReadInt();
			int z = ReadInt();
			pos = new WorldPos(x,y,z);
		}

		public override void ServerDo (ClientConnectionWorker connectionWork)
		{
			NetManager.Instance.server.entityManager.RemoveEntitiesInChunk(pos);
			NetManager.Instance.server.sceneManager.RemovePlayerChunks(connectionWork.player,pos);
		}
	}
}

