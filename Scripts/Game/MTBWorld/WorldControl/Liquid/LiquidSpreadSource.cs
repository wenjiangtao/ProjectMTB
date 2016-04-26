using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MTB
{
	public class LiquidSpreadSource
	{
		public delegate void SpreadPlaneFinishHandler(LiquidSpreadSource source);
		public event SpreadPlaneFinishHandler OnSpreadPlaneFinish;
		public BlockType liquidType{get;set;}

		public WorldPos pos{get;set;}
		public Chunk chunk{get;set;}
		public int curSpreadLevel{get;set;}
		private int _spreadWidth;
		private int _spreadMaxLevel = int.MinValue;
		public int spreadMaxLevel{get{
				return _spreadMaxLevel;
			}
			set{
				if(_spreadMaxLevel != value)
				{
					_spreadMaxLevel = value;
					_spreadWidth = _spreadMaxLevel * 2 + 1;
					_levelMap = new int[_spreadWidth * _spreadWidth];
				}
				ClearMap();
			}}
		private List<BlockType> _relationLiquidTypes;
		private Queue<int> _spreadQueue;
		private List<int> _holeList;
		private int[] _levelMap;
		private List<WorldPos> _nextSpreadSourceList;
		private Task _task;

		public LiquidSpreadSource ()
		{
			_spreadQueue = new Queue<int>();
			_holeList = new List<int>();
			_holeIndexs = new List<int>();
			_holeSearchQueue = new Queue<int>();
			_nextSpreadSourceList = new List<WorldPos>();
			_relationLiquidTypes = new List<BlockType>();
		}

		public void StartSpread()
		{
			_task = new Task(BeginSpread());
			_task.OnFinished += HandleOnFinished;
			TaskManager.Instance.AddTask(_task);
		}

		void HandleOnFinished ()
		{
			_task.OnFinished -= HandleOnFinished;
			if(OnSpreadPlaneFinish != null)
			{
				OnSpreadPlaneFinish(this);
			}
		}

		public List<WorldPos> GetNextSpreadSourceList()
		{
			return _nextSpreadSourceList;
		}

		private void ClearMap()
		{
			for (int i = 0; i < _levelMap.Length; i++) {
				_levelMap[i] = int.MaxValue;
			}
		}

		public void Reset()
		{
			liquidType = BlockType.Null;
			chunk = null;
			curSpreadLevel = 0;
			_relationLiquidTypes.Clear();
			_spreadQueue.Clear();
			_holeList.Clear();
			_holeIndexs.Clear();
			_holeSearchQueue.Clear();
			_nextSpreadSourceList.Clear();
			ClearMap();
			_task = null;
		}

		public void AddRelationLiquidType(BlockType type)
		{
			_relationLiquidTypes.Add(type);
		}

		private List<int> _holeIndexs;
		private Queue<int> _holeSearchQueue;
		private IEnumerator BeginSpread()
		{
			int startIndex = GetIndex(spreadMaxLevel,spreadMaxLevel);
			_levelMap[startIndex] = curSpreadLevel;
			_spreadQueue.Enqueue(startIndex);
			yield return new TaskWaitForSeconds(0.3f);
			while(curSpreadLevel < spreadMaxLevel)
			{
				SpreadInLevelMap();
				curSpreadLevel++;
				//找到洞
				if(_holeList.Count > 0)
				{
					bool findHole = false;
					_holeIndexs.Clear();
					_holeSearchQueue.Clear();
					//遍历所有洞穴
					for (int i = 0; i < _holeList.Count; i++) {
						int curIndex = _holeList[i];
						if(_levelMap[curIndex] >= curSpreadLevel)
						{
							_holeSearchQueue.Clear();
							//使用广度搜索遍历洞穴路径
							_holeSearchQueue.Enqueue(curIndex);
							while(_holeSearchQueue.Count > 0)
							{
								int searchHoleIndex = _holeSearchQueue.Dequeue();
								int x = searchHoleIndex % _spreadWidth;
								int z = searchHoleIndex / _spreadWidth;
								int tempIndex;
								if(_levelMap[searchHoleIndex] == curSpreadLevel)
								{
									_holeIndexs.Add(searchHoleIndex);
									continue;
								}

								if(_levelMap[searchHoleIndex] <= curSpreadLevel)continue;

								if(x -  1 >= 0)
								{
									tempIndex = GetIndex(x - 1,z);
									if(_levelMap[tempIndex] < _levelMap[searchHoleIndex])_holeSearchQueue.Enqueue(tempIndex);
								}
								if(x + 1 < _spreadWidth)
								{
									tempIndex = GetIndex(x + 1,z);
									if(_levelMap[tempIndex] < _levelMap[searchHoleIndex])_holeSearchQueue.Enqueue(tempIndex);
								}
								if(z - 1 >= 0)
								{
									tempIndex = GetIndex(x,z - 1);
									if(_levelMap[tempIndex] < _levelMap[searchHoleIndex])_holeSearchQueue.Enqueue(tempIndex);
								}
								if(z + 1 < _spreadWidth)
								{
									tempIndex = GetIndex(x,z + 1);
									if(_levelMap[tempIndex] < _levelMap[searchHoleIndex])_holeSearchQueue.Enqueue(tempIndex);
								}
							}
						}
						else
						{
							findHole = true;
							break;
						}
					}
					if(findHole)
					{
						break;
					}
					else
					{
						for (int i = 0; i < _holeIndexs.Count; i++) {
							SetLiquidBlock(_holeIndexs[i],curSpreadLevel);
							_spreadQueue.Enqueue(_holeIndexs[i]);
						}
					}
				}
				else
				{
					for (int i = 0; i < _levelMap.Length; i++) {
						if(_levelMap[i] == curSpreadLevel)
						{
							SetLiquidBlock(i,curSpreadLevel);
							_spreadQueue.Enqueue(i);
						}
					}
				}
				for (int i = 0; i < _levelMap.Length; i++) {
					if(_spreadQueue.Contains(i))
					{
						_levelMap[i] = curSpreadLevel;
					}else
					{
						_levelMap[i] = int.MaxValue;
					}
				}
				yield return new TaskWaitForSeconds(0.3f);
			}
			for (int i = 0; i < _holeList.Count; i++) {
				int nextSpreadSourceIndex = _holeList[i];
				int x = nextSpreadSourceIndex % _spreadWidth - spreadMaxLevel + pos.x;
				int z = nextSpreadSourceIndex / _spreadWidth - spreadMaxLevel + pos.z;
				Block holeBlock = chunk.GetBlock(x,pos.y - 1,z);

				if(holeBlock.BlockType == liquidType)
				{
					int holeBlockLevel = holeBlock.ExtendId > spreadMaxLevel ? 0 : (int)holeBlock.ExtendId;
					if(holeBlockLevel == 0)continue;
				}
				if(_relationLiquidTypes.Contains(holeBlock.BlockType))
				{
					continue;
				}
				_nextSpreadSourceList.Add(new WorldPos(x + chunk.worldPos.x,pos.y - 1 +chunk.worldPos.y,z + chunk.worldPos.z));
			}
			yield return null;
		}

		private void SetLiquidBlock(int index,int level)
		{
			int posXInChunk = index % _spreadWidth - spreadMaxLevel + pos.x;
			int posZInChunk = index / _spreadWidth - spreadMaxLevel + pos.z;
			Block liquid = new Block(liquidType,(byte)level);

			BlockDispatcher dispatcher = BlockDispatcherFactory.GetDefaultDispatcher();
			dispatcher.SetBlock(chunk.world,posXInChunk + chunk.worldPos.x,pos.y + chunk.worldPos.y,posZInChunk + chunk.worldPos.z,liquid,false);
//			SetBlockAndNotify(chunk.world,posXInChunk + chunk.worldPos.x,pos.y + chunk.worldPos.y,posZInChunk + chunk.worldPos.z,liquid);
//			chunk.world.CheckAndRecalculateMesh(posXInChunk + chunk.worldPos.x,pos.y + chunk.worldPos.y,posZInChunk + chunk.worldPos.z,liquid);
		}

//		public void SetBlockAndNotify(World world,int x,int y,int z,Block block)
//		{
//			Block oldBlock = world.GetBlock(x,y,z);
//			world.SetBlock(x,y,z,block);
//			//当有玩家对物块进行操作时，同步物块数据
//			Chunk chunk = world.GetChunk(x,y,z);
//			if(chunk != null && chunk.isPopulationDataPrepared)
//			{
//				int tempX = x - chunk.worldPos.x;
//				int tempY = y - chunk.worldPos.y;
//				int tempZ = z - chunk.worldPos.z;
//				EventManager.SendEvent(EventMacro.BLOCK_DATA_UPDATE_AFTER_POPULATION,chunk.worldPos,tempX,tempY,tempZ,block);
//			}
//			NotifyAround(world,x,y,z,oldBlock,block);
//		}
//		
//		public void NotifyAround(World world,int x,int y,int z,Block oldBlock,Block newBlock)
//		{
//			IBlockNotify leftNotify = GetBlockNotify(world,x - 1,y,z);
//			leftNotify.Notify(world,x - 1,y,z,Direction.right,oldBlock,newBlock);
//			
//			IBlockNotify rightNotify = GetBlockNotify(world,x + 1,y,z);
//			rightNotify.Notify(world,x + 1,y,z,Direction.left,oldBlock,newBlock);
//			
//			IBlockNotify frontNotify = GetBlockNotify(world,x,y,z + 1);
//			frontNotify.Notify(world,x,y,z + 1, Direction.back,oldBlock,newBlock);
//			
//			IBlockNotify backNotify = GetBlockNotify(world,x,y,z - 1);
//			backNotify.Notify(world,x,y,z - 1,Direction.front,oldBlock,newBlock);
//			
//			IBlockNotify upNotify = GetBlockNotify(world,x,y + 1,z);
//			upNotify.Notify(world,x,y + 1,z,Direction.down,oldBlock,newBlock);
//			
//			IBlockNotify downNotify = GetBlockNotify(world,x,y - 1,z);
//			downNotify.Notify(world,x,y - 1,z,Direction.up,oldBlock,newBlock);
//		}
//		private IBlockNotify GetBlockNotify(World world,int x,int y,int z)
//		{
//			Block b = world.GetBlock(x,y,z);
//			return BlockAttributeCalculatorFactory.GetCalculator(b.BlockType);
//		}

		private void SpreadInLevelMap()
		{
			_holeList.Clear();
			int nextX;
			int nextZ;
			int nextPosXInChunk;
			int nextPosZInChunk;

			int curHoleLevel = int.MaxValue;
			while(_spreadQueue.Count > 0)
			{
				int curIndex = _spreadQueue.Dequeue();
				int z = curIndex / _spreadWidth;
				int x = curIndex % _spreadWidth;

				int posXInChunk = x - spreadMaxLevel + pos.x;
				int posZInChunk = z - spreadMaxLevel + pos.z;

				//如果已经找到洞了，并且当前循环等级不等于找到洞的等级，就不需要找下去了
				if(curHoleLevel != int.MaxValue && _levelMap[curIndex] != curHoleLevel)continue;

				//y - 1
				Block block = chunk.GetBlock(posXInChunk,pos.y - 1,posZInChunk);
				if(block.BlockType == BlockType.Air || block.BlockType == liquidType || _relationLiquidTypes.Contains(block.BlockType))
				{
					_holeList.Add(curIndex);
					curHoleLevel = _levelMap[curIndex];
					continue;
				}
//				if(block.BlockType == liquidType)
//				{
//					int blockLevel = block.ExtendId > spreadMaxLevel ? 0 : (int)block.ExtendId;
//					if(blockLevel > 0)
//					{
//						_holeList.Add(curIndex);
//						curHoleLevel = _levelMap[curIndex];
//					}
//					continue;
//				}

				if(_levelMap[curIndex] >= spreadMaxLevel)continue;

				//x - 1
				nextX = x - 1;
				nextZ = z;
				nextPosXInChunk = posXInChunk - 1;
				nextPosZInChunk = posZInChunk;
				SpreadInPos(nextPosXInChunk,nextPosZInChunk,nextX,nextZ,_levelMap[curIndex] + 1); 

				//x + 1
				nextX = x + 1;
				nextZ = z;
				nextPosXInChunk = posXInChunk + 1;
				nextPosZInChunk = posZInChunk;
				SpreadInPos(nextPosXInChunk,nextPosZInChunk,nextX,nextZ,_levelMap[curIndex] + 1); 


				//z - 1
				nextX = x;
				nextZ = z - 1;
				nextPosXInChunk = posXInChunk;
				nextPosZInChunk = posZInChunk - 1;
				SpreadInPos(nextPosXInChunk,nextPosZInChunk,nextX,nextZ,_levelMap[curIndex] + 1); 

				//z + 1
				nextX = x;
				nextZ = z + 1;
				nextPosXInChunk = posXInChunk;
				nextPosZInChunk = posZInChunk + 1;
				SpreadInPos(nextPosXInChunk,nextPosZInChunk,nextX,nextZ,_levelMap[curIndex] + 1); 

			}
		}

		private void SpreadInPos(int posXInChunk,int posZInChunk,int x,int z,int nextLevel)
		{
			int index = GetIndex(x,z);
			if(_levelMap[index] == int.MaxValue)
			{
				Block block = chunk.GetBlock(posXInChunk,pos.y,posZInChunk);
				if(block.BlockType == BlockType.Air)
				{
					_levelMap[index] = nextLevel;
					_spreadQueue.Enqueue(index);
				}
				//判断当前地图有水的时候该怎么做
				else if(block.BlockType == liquidType)
				{
					int blockLevel = block.ExtendId > spreadMaxLevel ? 0 : (int)block.ExtendId;
					if(blockLevel > nextLevel)
					{
						_levelMap[index] = nextLevel;
						_spreadQueue.Enqueue(index);
					}
				}
			}
			else
			{
				if(_levelMap[index] > nextLevel)
				{
					_levelMap[index] = nextLevel;
					_spreadQueue.Enqueue(index);
				}
			}
		}


		private int GetIndex(int x,int z)
		{
			return x + z * _spreadWidth;
		}
	}
}

