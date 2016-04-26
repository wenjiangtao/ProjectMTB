using System;
using System.Collections.Generic;
namespace MTB
{
	public class ChunkPopulationGeneratePackage : TcpPackage
	{
		public WorldPos pos{get;set;}
		public int sign{get;set;}
		public List<ClientChangedBlock> changedBlocks{get;set;}
		public ChunkPopulationGeneratePackage (int id)
			:base(id)
		{
		}
		public override void WriteAllData ()
		{
			WriteInt(sign);
			WriteInt(pos.x);
			WriteInt(pos.y);
			WriteInt(pos.z);
			WriteInt(changedBlocks.Count);
			for (int i = 0; i < changedBlocks.Count; i++) {
				WriteChangedBlock(changedBlocks[i]);
			}
		}
		
		public override void ReadAllData ()
		{
			sign = ReadInt();
			int x = ReadInt();
			int y = ReadInt();
			int z = ReadInt();
			pos = new WorldPos(x,y,z);
			changedBlocks = new List<ClientChangedBlock>();
			int count = ReadInt();
			for (int i = 0; i < count; i++) {
				changedBlocks.Add(ReadChangedBlock());
			}
		}

		private void WriteChangedBlock(ClientChangedBlock block)
		{
			WriteInt16(block.index);
			WriteByte(block.blockType);
			WriteByte(block.extendId);
		}

		private ClientChangedBlock ReadChangedBlock()
		{
			Int16 index = ReadInt16();
			byte blockType = ReadByte();
			byte extendId = ReadByte();
			return new ClientChangedBlock(index,blockType,extendId);
		}
		
		public override void ServerDo (ClientConnectionWorker connectionWork)
		{
			NetManager.Instance.server.sceneManager.ChangedSign(pos,sign);
			for (int i = 0; i < changedBlocks.Count; i++) {
				NetManager.Instance.server.sceneManager.ChangedBlock(pos,changedBlocks[i]);
			}
//			UnityEngine.Debug.Log("收到chunkPos:" + pos.ToString() + "的生物群落产生消息，数量为:" + changedBlocks.Count + " sign:" + sign);
			NetManager.Instance.server.sceneManager.BroadcastPlayerHasChunkPackage(connectionWork.player,pos,this,false);

		}
		
		public override void ClientDo (ServerConnectionWorker connectionWork)
		{
			NetChunkData data = new NetChunkData(NetManager.Instance.client.roleId,pos);
			data.isExist = false;
			data.hasChangeData = true;
			data.changedData = new ClientChangedChunkData(sign,changedBlocks);
			EventManager.SendEvent(NetEventMacro.ON_NET_POPULATION_GENERATE,data);
		}
	}
}

