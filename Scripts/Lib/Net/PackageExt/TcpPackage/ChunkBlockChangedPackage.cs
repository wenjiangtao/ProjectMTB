using System;
namespace MTB
{
	//chunk中的block改变包
	public class ChunkBlockChangedPackage : TcpPackage
	{
		public WorldPos pos{get;set;}
		public ClientChangedBlock changedBlock{get;set;}
		public ChunkBlockChangedPackage (int id)
			:base(id)
		{
		}

		public override void WriteAllData ()
		{
			WriteInt(pos.x);
			WriteInt(pos.y);
			WriteInt(pos.z);
			WriteInt16(changedBlock.index);
			WriteByte(changedBlock.blockType);
			WriteByte(changedBlock.extendId);
		}

		public override void ReadAllData ()
		{
			int x = ReadInt();
			int y = ReadInt();
			int z = ReadInt();
			pos = new WorldPos(x,y,z);
			Int16 index = ReadInt16();
			byte blockType = ReadByte();
			byte extendId = ReadByte();
			changedBlock = new ClientChangedBlock(index,blockType,extendId);
		}

		public override void ServerDo (ClientConnectionWorker connectionWork)
		{
			NetManager.Instance.server.sceneManager.ChangedBlock(pos,changedBlock);
			NetManager.Instance.server.sceneManager.BroadcastPlayerHasChunkPackage(connectionWork.player,pos,this,false);
//			UnityEngine.Debug.Log("收到更改chunk:" + pos.ToString() + " block:" + changedBlock.index + " " + changedBlock.blockType);
		}

		public override void ClientDo (ServerConnectionWorker connectionWork)
		{
			Chunk chunk = World.world.GetChunk(pos.x,pos.y,pos.z);
			chunk.UpdateClientChangedBlock(changedBlock,true);
		}
	}
}

