using System;
using System.Collections.Generic;
namespace MTB
{
	//服务器返回除主机外的其他客户端请求的chunk数据
	public class ResponseChunkPackage : TcpPackage
	{
		public int roleId{get;set;}
		public WorldPos pos{get;set;}
		public bool isExit{get;set;}
		public byte[] chunkByteData{get;set;}
		public byte compressType{get;set;}
		public bool hasChangedData{get;set;}
		public ClientChangedChunkData changedData{get;set;}
		public ResponseChunkPackage (int id)
			:base(id)
		{
			hasChangedData = false;
		}

		public override void WriteAllData ()
		{
			try{
				WriteInt(roleId);
				WriteInt(pos.x);
				WriteInt(pos.y);
				WriteInt(pos.z);
				WriteBool(isExit);
				if(isExit)
				{
					WriteByte(compressType);
					WriteBytes(chunkByteData);
				}
				WriteBool(hasChangedData);
				if(hasChangedData)
				{
					WriteInt(changedData.sign);
					WriteListChangedBlockData(changedData.list);
				}
			}catch(Exception e)
			{
				UnityEngine.Debug.LogError(e.StackTrace);
			}
		}

		public override void ReadAllData ()
		{
			try{
				roleId = ReadInt();
				int x = ReadInt();
				int y = ReadInt();
				int z = ReadInt();
				pos = new WorldPos(x,y,z);
				isExit = ReadBool();
				if(isExit)
				{
					compressType = ReadByte();
					chunkByteData = ReadBytes();
				}
				hasChangedData = ReadBool();
				if(hasChangedData)
				{
					int sign = ReadInt();
					List<ClientChangedBlock> list = ReadListChangedBlockData();
					changedData = new ClientChangedChunkData(sign,list);
				}
			}catch(Exception e)
			{
				UnityEngine.Debug.LogError(e.StackTrace);
			}
		}

		private void WriteListChangedBlockData(List<ClientChangedBlock> list)
		{
			WriteInt(list.Count);
			for (int i = 0; i < list.Count; i++) {
				WriteChangedBlockData(list[i]);
			}
		}

		private List<ClientChangedBlock> ReadListChangedBlockData()
		{
			int count = ReadInt();
			List<ClientChangedBlock> list = new List<ClientChangedBlock>();
			for (int i = 0; i < count; i++) {
				list.Add(ReadChangedBlockData());
			}
			return list;
		}

		private void WriteChangedBlockData(ClientChangedBlock block)
		{
			WriteInt16(block.index);
			WriteByte(block.blockType);
			WriteByte(block.extendId);
		}

		private ClientChangedBlock ReadChangedBlockData()
		{
			Int16 index = ReadInt16();
			byte blockType = ReadByte();
			byte extendId = ReadByte();
			ClientChangedBlock block = new ClientChangedBlock(index,blockType,extendId);
			return block;
		}

		public override void ServerDo (ClientConnectionWorker connectionWork)
		{
			ClientPlayer player = NetManager.Instance.server.playerManager.GetPlayer(roleId);
			ClientChangedChunkData data = NetManager.Instance.server.sceneManager.GetChangedChunkData(pos);
			if(data != null)
			{
				hasChangedData = true;
				changedData = data;
			}
			if(player != null)player.worker.SendPackage(this);
		}

		public override void ClientDo (ServerConnectionWorker connectionWork)
		{
			NetChunkData data = new NetChunkData(roleId,pos);
			data.isExist = isExit;
			data.data.compressType = (MTBCompressType)compressType;
			data.data.data = chunkByteData;
			data.hasChangeData = hasChangedData;
			data.changedData = changedData;
			EventManager.SendEvent(NetEventMacro.ON_NET_CHUNK_GENERATOR_RETURN,data);
		}
	}
}

