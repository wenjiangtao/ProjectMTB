using System;
using System.Collections.Generic;
namespace MTB
{
	public class ClientChangedChunk
	{
		private const int XBit = 15;
		private const int ZBit = (15 << 4);
		private const int YBit = (255 << 8);
		private const int IsPopulationDataPreparedBit = 1;
		private const int HasRefreshEntityBit = 2;
		private Dictionary<Int16,ClientChangedBlock> _blockMap;
		public bool needSave{get;private set;}
		//主要维护chunk标志信息
		private int sign;
		//维护一份玩家对当前块修改的引用,如果当前玩家数量<=0的时候，就应该保存当前修改的数据到主机
		public List<int> players{get;private set;}
		public WorldPos pos{get;private set;}
		public ClientChangedChunk (WorldPos pos)
		{
			_blockMap = new Dictionary<Int16, ClientChangedBlock>();
			players = new List<int>();
			sign = 0;
			needSave = false;
			this.pos = pos;
		}

		public void ChangeNeedSave(bool needSave)
		{
			if(this.needSave != needSave)
			{
				this.needSave = needSave;
			}
		}

		private void CheckNeedSave()
		{
			if(players.Count < 2 && _blockMap.Count > 0)
			{
				ChangeNeedSave(true);
			}
			else
			{
				ChangeNeedSave(false);
			}
		}

		public bool RefreshEntity()
		{
			if((sign & IsPopulationDataPreparedBit) != 0)
			{
				if((sign & HasRefreshEntityBit) == 0)
				{
					sign |= HasRefreshEntityBit;
					return true;
				}
			}
			else
			{
				UnityEngine.Debug.Log("区块位置pos:" + pos.ToString() +"想刷新怪物，但它的生物群落并没有产生!");
			}
			return false;
		}

		public void AddPlayer(int clientPlayerId)
		{
			if(!players.Contains(clientPlayerId))
			{
				players.Add(clientPlayerId);
				CheckNeedSave();
			}
		}

		public bool RemovePlayer(int clientPlayerId)
		{
			bool succ = players.Remove(clientPlayerId);
			CheckNeedSave();
			return succ;
		}

		public void ChangeBlock(ClientChangedBlock block)
		{
			if(_blockMap.ContainsKey(block.index))
			{
				_blockMap[block.index] = block;
			}
			else
			{
				_blockMap.Add(block.index,block);
				CheckNeedSave();
			}
		}

		public void ChangeSign(int sign)
		{
			this.sign = sign;
		}

		public int GetSign()
		{
			return this.sign;
		}

		public ClientChangedChunkData GetChangedData()
		{
			List<ClientChangedBlock> list = new List<ClientChangedBlock>(_blockMap.Count);
			foreach (var item in _blockMap) {
				list.Add(item.Value);
			}
			return new ClientChangedChunkData(sign,list);
		}

		public static Int16 GetChunkIndex(int x,int y,int z)
		{
			return (Int16)(x + (z << 4) + (y << 8));
		}

		public static WorldPos GetChunkPos(Int16 index)
		{
			int x = (Int16)(index & XBit);
			int z = (Int16)((index & ZBit) >> 4);
			int y = (Int16)((index & YBit) >> 8);
			return new WorldPos(x,y,z);
		}
	}

	public class ClientChangedChunkData
	{
		public int sign{get;private set;}
		public List<ClientChangedBlock> list{get;private set;}
		public ClientChangedChunkData(int sign,List<ClientChangedBlock> list)
		{
			this.sign = sign;
			this.list = list;
		}
	}
}

