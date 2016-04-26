using System;
using System.Collections.Generic;
namespace MTB
{
	//将大量更新的Block收集起来，并发送给服务器
	public class ClientBlockCollection
	{
		public static ClientBlockCollection Instance{get{
				if(_instance == null)_instance = new ClientBlockCollection();
				return _instance;
			}}
		private static ClientBlockCollection _instance;
		private Dictionary<WorldPos,List<ClientChangedBlock>> _map; 
		public bool canCollection{get;private set;}
		public ClientBlockCollection ()
		{
			_map = new Dictionary<WorldPos, List<ClientChangedBlock>>(new WorldPosComparer());
			canCollection = false;
		}

		public void StartInChunk(WorldPos chunkPos)
		{
			_map.Add(chunkPos,new List<ClientChangedBlock>());
		}

		public void BeginCollection()
		{
			canCollection = true;
		}

		public void EndCollection()
		{
			_map.Clear();
			canCollection = false;
		}

		public void Collection(WorldPos pos,int x,int y,int z,Block block)
		{
			if(canCollection)
			{
				List<ClientChangedBlock> list;
				_map.TryGetValue(pos,out list);
				if(list == null)
				{
					list = new List<ClientChangedBlock>();
					_map.Add(pos,list);
				}
				Int16 index = ClientChangedChunk.GetChunkIndex(x,y,z);
				byte blockType = (byte)block.BlockType;
				byte extendId = block.ExtendId;
				ClientChangedBlock changedBlock = new ClientChangedBlock(index,blockType,extendId);
				list.Add(changedBlock);
			}
		}

		public void SendPackage()
		{
			foreach (var item in _map) {
				ChunkPopulationGeneratePackage package = PackageFactory.GetPackage(PackageType.BatchChunkBlockChanged)
					as ChunkPopulationGeneratePackage;
				package.pos = item.Key;
				package.changedBlocks = item.Value;
				package.sign = World.world.GetChunk(item.Key.x,item.Key.y,item.Key.z).GetSign();
				NetManager.Instance.client.SendPackage(package);
			}
		}

		public void Clear()
		{
			_map.Clear();
			canCollection = false;
		}
	}
}

