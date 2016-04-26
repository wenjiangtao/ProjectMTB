using System;
using System.Collections;
using System.Collections.Generic;
namespace MTB
{
	public class LiquidShrinkSource
	{
		public delegate void ShrinkPlaneFinishHandler(LiquidShrinkSource source);
		public event ShrinkPlaneFinishHandler OnShrinkPlaneFinish;
		public BlockType liquidType{get;set;}
		public Block air = new Block(BlockType.Air);
		
		public WorldPos pos{get;set;}
		public Chunk chunk{get;set;}
		public int curShrinkLevel{get;set;}
		private int _shrinkWidth;
		private int _shrinkMaxLevel = int.MinValue;
		public int shrinkMaxLevel{get{
				return _shrinkMaxLevel;
			}
			set{
				if(_shrinkMaxLevel != value)
				{
					_shrinkMaxLevel = value;
					_shrinkWidth = _shrinkMaxLevel * 2 + 1;
					_levelMap = new int[_shrinkWidth * _shrinkWidth];
				}
				ClearMap();
			}}
//		private List<BlockType> _relationLiquidTypes;
		private Queue<int> _shrinkQueue;
//		private List<int> _holeList;
		private int[] _levelMap;
		private List<WorldPos> _nextShrinkSourceList;
		private Task _task;
		private List<int> _curShrinkList;
		public LiquidShrinkSource ()
		{
			_curShrinkList = new List<int>();
			_shrinkQueue = new Queue<int>();
			_nextShrinkSourceList = new List<WorldPos>();
		}

		private void ClearMap()
		{
			for (int i = 0; i < _levelMap.Length; i++) {
				_levelMap[i] = int.MinValue;
			}
		}

		public List<WorldPos> GetNextSpreadSourceList()
		{
			return _nextShrinkSourceList;
		}

		public void Reset()
		{
			liquidType = BlockType.Null;
			chunk = null;
			curShrinkLevel = 0;
			_curShrinkList.Clear();
			_shrinkQueue.Clear();
			_nextShrinkSourceList.Clear();
			ClearMap();
			_task = null;
		}

		public void StartShrink()
		{
			_task = new Task(BeginShrink());
			_task.OnFinished += HandleOnFinished;
			TaskManager.Instance.AddTask(_task);
		}
		
		void HandleOnFinished ()
		{
			_task.OnFinished -= HandleOnFinished;
			if(OnShrinkPlaneFinish != null)
			{
				OnShrinkPlaneFinish(this);
			}
		}

		private IEnumerator BeginShrink()
		{
			int startIndex = GetIndex(_shrinkMaxLevel,_shrinkMaxLevel);
			_levelMap[startIndex] = curShrinkLevel;
			_shrinkQueue.Enqueue(startIndex);
			while(curShrinkLevel < shrinkMaxLevel)
			{
				yield return new TaskWaitForSeconds(0.3f);
				ShrinkInLevelMap();
				curShrinkLevel++;
				for (int i = 0; i < _curShrinkList.Count; i++) {
					ClearLiquidBlock(_curShrinkList[i]);
					_shrinkQueue.Enqueue(_curShrinkList[i]);
				}
				if(_shrinkQueue.Count <= 0)break;
			}
			yield return null;
		}

		private void ClearLiquidBlock(int index)
		{
			int posXInChunk = index % _shrinkWidth - shrinkMaxLevel + pos.x;
			int posZInChunk = index / _shrinkWidth - shrinkMaxLevel + pos.z;

//			chunk.world.SetBlock(posXInChunk + chunk.worldPos.x,pos.y + chunk.worldPos.y,posZInChunk + chunk.worldPos.z,air);
//			chunk.world.CheckAndRecalculateMesh(posXInChunk + chunk.worldPos.x,pos.y + chunk.worldPos.y,posZInChunk + chunk.worldPos.z,air);
			BlockDispatcher dispatcher = BlockDispatcherFactory.GetDefaultDispatcher();
			dispatcher.SetBlock(chunk.world,posXInChunk + chunk.worldPos.x,pos.y + chunk.worldPos.y,posZInChunk + chunk.worldPos.z,air,false);
		}


		private void ShrinkInLevelMap()
		{
//			_holeList.Clear();
			_curShrinkList.Clear();
			int nextX;
			int nextZ;
			int nextPosXInChunk;
			int nextPosZInChunk;
			int curHoleLevel = int.MinValue;
			while(_shrinkQueue.Count > 0)
			{
				int curIndex = _shrinkQueue.Dequeue();
				int z = curIndex / _shrinkWidth;
				int x = curIndex % _shrinkWidth;
				
				int posXInChunk = x - _shrinkMaxLevel + pos.x;
				int posZInChunk = z - _shrinkMaxLevel + pos.z;

				//如果已经找到洞了，并且当前循环等级不等于找到洞的等级，就不需要找下去了
//				if(curHoleLevel != int.MinValue && _levelMap[curIndex] != curHoleLevel)continue;

				//y - 1
				Block block = chunk.GetBlock(posXInChunk,pos.y - 1,posZInChunk);
				if(block.BlockType == liquidType && block.ExtendId > shrinkMaxLevel)
				{
					_nextShrinkSourceList.Add(new WorldPos(chunk.worldPos.x + posXInChunk,pos.y - 1,chunk.worldPos.z + posZInChunk));
				}

				if(_levelMap[curIndex] > curShrinkLevel)continue;

				//x - 1
				nextX = x - 1;
				nextZ = z;
				nextPosXInChunk = posXInChunk - 1;
				nextPosZInChunk = posZInChunk;
				ShrinkInPos(nextPosXInChunk,nextPosZInChunk,nextX,nextZ,_levelMap[curIndex]); 
				
				//x + 1
				nextX = x + 1;
				nextZ = z;
				nextPosXInChunk = posXInChunk + 1;
				nextPosZInChunk = posZInChunk;
				ShrinkInPos(nextPosXInChunk,nextPosZInChunk,nextX,nextZ,_levelMap[curIndex]); 
				
				
				//z - 1
				nextX = x;
				nextZ = z - 1;
				nextPosXInChunk = posXInChunk;
				nextPosZInChunk = posZInChunk - 1;
				ShrinkInPos(nextPosXInChunk,nextPosZInChunk,nextX,nextZ,_levelMap[curIndex]); 
				
				//z + 1
				nextX = x;
				nextZ = z + 1;
				nextPosXInChunk = posXInChunk;
				nextPosZInChunk = posZInChunk + 1;
				ShrinkInPos(nextPosXInChunk,nextPosZInChunk,nextX,nextZ,_levelMap[curIndex]); 

			}
		}
		
		private void ShrinkInPos(int posXInChunk,int posZInChunk,int x,int z,int preLevel)
		{
			int index = GetIndex(x,z);
			if(_levelMap[index] == int.MinValue)
			{
				Block block = chunk.GetBlock(posXInChunk,pos.y,posZInChunk);
				if(block.BlockType == liquidType)
				{
					int blockLevel = block.ExtendId > shrinkMaxLevel ? 0 : (int)block.ExtendId;
					if(preLevel < blockLevel)
					{
						_levelMap[index] = blockLevel;
						_curShrinkList.Add(index);
						_shrinkQueue.Enqueue(index);
					}
				}
			}
		}

		private int GetIndex(int x,int z)
		{
			return x + z * _shrinkWidth;
		}
	}
}

