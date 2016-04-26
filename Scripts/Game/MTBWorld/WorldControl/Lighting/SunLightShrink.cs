using System;
using System.Collections.Generic;
namespace MTB
{
	public class SunLightShrink
	{
		private World _world;
		private Queue<LightShrinkNode> _lightBfsQueue;
		private List<Chunk> _changedList;
		
		private SunLightSpread _lightSpread;
		public SunLightShrink (World world)
		{
			_world = world;
			_lightBfsQueue = new Queue<LightShrinkNode>();
			_changedList = new List<Chunk>();
			_lightSpread = new SunLightSpread(_world);
		}
		
		public void AddShrinkNode(LightShrinkNode node)
		{
			_lightBfsQueue.Enqueue(node);
		}
		
		public void ClearNode()
		{
			_lightBfsQueue.Clear();
			_lightSpread.ClearNode();
		}

		public List<Chunk> ShrinkInChunk(Chunk chunk)
		{
			int nextX;
			int nextY;
			int nextZ;
			Chunk nextChunk;
			_changedList.Clear();
			while(_lightBfsQueue.Count > 0)
			{
				LightShrinkNode node = _lightBfsQueue.Dequeue();
				int y = node.index % Chunk.chunkHeight;
				int temp = node.index / Chunk.chunkHeight;
				int z = temp % Chunk.chunkDepth;
				int x = temp / Chunk.chunkDepth;
				
				Chunk nodeChunk = node.chunk;
				
				int prevLightLevel = node.prevLightLevel;
				
				int curLightLevel = node.lightLevel;


				NodeCache.Instance.SaveShrinkNode(node);

				//y - 1
				nextY = y - 1;
				if(nextY >= 0)
				{
					SpreadInPosDown(x,nextY,z,nodeChunk,prevLightLevel,curLightLevel);
				}

				nextX = x - 1;
				nextChunk = nodeChunk;
				if(nextX < 0)
				{
					nextChunk = _world.GetChunk(nextX + nodeChunk.worldPos.x,y + nodeChunk.worldPos.y,z + nodeChunk.worldPos.z);
					nextX = Chunk.chunkWidth - 1;
				}
				
				if(nextChunk != null && (nextChunk.isLightDataPrepared || nextChunk.worldPos.EqualOther(chunk.worldPos)))
				{
					ShrinkHorizontalInPos(nextX,y,z,nextChunk, prevLightLevel,curLightLevel);
				}
//				
				//x + 1
				nextX = x + 1;
				nextChunk = nodeChunk;
				if(nextX >= Chunk.chunkWidth)
				{
					nextChunk = _world.GetChunk(nextX + nodeChunk.worldPos.x,y + nodeChunk.worldPos.y,z + nodeChunk.worldPos.z);
					nextX = 0;
				}
				
				if(nextChunk != null && (nextChunk.isLightDataPrepared || nextChunk.worldPos.EqualOther(chunk.worldPos)))
				{
					ShrinkHorizontalInPos(nextX,y,z,nextChunk,prevLightLevel,curLightLevel);
				}
				
				//z - 1
				nextZ = z - 1;
				nextChunk = nodeChunk;
				if(nextZ < 0)
				{
					nextChunk = _world.GetChunk(x + nodeChunk.worldPos.x,y + nodeChunk.worldPos.y,nextZ + nodeChunk.worldPos.z);
					nextZ = Chunk.chunkDepth - 1;
				}
				if(nextChunk != null && (nextChunk.isLightDataPrepared || nextChunk.worldPos.EqualOther(chunk.worldPos)))
				{
					ShrinkHorizontalInPos(x,y,nextZ,nextChunk,prevLightLevel,curLightLevel);
				}
				
				//z + 1
				nextZ = z + 1;
				nextChunk = nodeChunk;
				if(nextZ >= Chunk.chunkDepth)
				{
					nextChunk = _world.GetChunk(x + nodeChunk.worldPos.x,y + nodeChunk.worldPos.y,nextZ + nodeChunk.worldPos.z);
					nextZ = 0;
				}
				if(nextChunk != null && (nextChunk.isLightDataPrepared || nextChunk.worldPos.EqualOther(chunk.worldPos)))
				{
					ShrinkHorizontalInPos(x,y,nextZ,nextChunk,prevLightLevel,curLightLevel);
				}
				
				// y + 1
				nextY = y + 1;
				if(nextY < Chunk.chunkHeight)
				{
					ShrinkInPosUp(x,nextY,z,nodeChunk,prevLightLevel,curLightLevel);
				}
			}

			List<Chunk> spreadList = _lightSpread.SpreadInChunk(chunk);
			for (int i = 0; i < spreadList.Count; i++) {
				if(!_changedList.Contains(spreadList[i]))
				{
					_changedList.Add(spreadList[i]);
				}
			}
			return _changedList;
		}
		
		private void ShrinkHorizontalInPos(int x,int y,int z,Chunk chunk,int prevLightLevel,int curLightLevel)
		{
			//当前光照强度为最大值时不收缩
			if(curLightLevel >= WorldConfig.Instance.maxLightLevel)return;
			//当前高度大于光照高度，不再收缩,直接从旁边扩散
			if(y >= chunk.GetHeight(x,z,true))
			{
				int lightLevel= chunk.GetSunLight(x,y,z);
				int nextIndex = (x * Chunk.chunkDepth + z) * Chunk.chunkHeight + y; 
				_lightSpread.AddSpreadNode(NodeCache.Instance.GetSpreadNode(nextIndex,lightLevel,chunk));
				return;
			}
			Block b = chunk.GetBlock(x,y,z,true);
			BlockAttributeCalculator calculator = BlockAttributeCalculatorFactory.GetCalculator(b.BlockType);
			int lightDamp = calculator.LightDamp(b.ExtendId);
			if(lightDamp < WorldConfig.Instance.maxLightLevel)
			{
				int lightLevel= chunk.GetSunLight(x,y,z);
				if(lightLevel > 0)
				{
					int temp = prevLightLevel - lightDamp;
					//如果前一个物块比当前物块的太阳光亮，那么减弱当前物块的亮度
					if(temp > lightLevel)
					{
						int nextLightLevel = curLightLevel - lightDamp - 1;
						if(nextLightLevel < 0)
						{
							nextLightLevel = 0;
						}
						//如果最终结果没有发生改变，那么不收缩
						if(nextLightLevel == lightLevel)return;
						chunk.SetSunLight(x,y,z,nextLightLevel,true);
						if(!_changedList.Contains(chunk))
						{
							_changedList.Add(chunk);
						}
						int nextIndex = (x * Chunk.chunkDepth + z) * Chunk.chunkHeight + y; 
						_lightBfsQueue.Enqueue(NodeCache.Instance.GetShrinkNode(nextIndex,lightLevel,nextLightLevel,chunk));
					}
					//如果前一个物块比当前物块的太阳光暗，那么增强前一个物块的亮度
					else if(temp < lightLevel)
					{
						int nextIndex = (x * Chunk.chunkDepth + z) * Chunk.chunkHeight + y; 
						_lightSpread.AddSpreadNode(NodeCache.Instance.GetSpreadNode(nextIndex,lightLevel,chunk));
					}
				}
			}
		}

		private void ShrinkInPosUp(int x,int y,int z,Chunk chunk,int prevLightLevel,int curLightLevel)
		{
			//当前光照强度为最大值时不收缩
			if(curLightLevel >= WorldConfig.Instance.maxLightLevel)return;
			//当前高度大于光照高度，不再收缩,直接从旁边扩散
			if(y >= chunk.GetHeight(x,z,true))return;

			Block b = chunk.GetBlock(x,y,z,true);
			BlockAttributeCalculator calculator = BlockAttributeCalculatorFactory.GetCalculator(b.BlockType);
			int lightDamp = calculator.LightDamp(b.ExtendId);
			if(lightDamp < WorldConfig.Instance.maxLightLevel)
			{
				int lightLevel= chunk.GetSunLight(x,y,z);
				if(lightLevel > 0)
				{
					int temp = prevLightLevel - lightDamp;
					//如果前一个物块比当前物块的太阳光亮，那么减弱当前物块的亮度
					if(temp > lightLevel)
					{
						int nextLightLevel = curLightLevel - lightDamp - 1;
						if(nextLightLevel < 0)
						{
							nextLightLevel = 0;
						}
						if(nextLightLevel == lightLevel)return;
						chunk.SetSunLight(x,y,z,nextLightLevel,true);
						if(!_changedList.Contains(chunk))
						{
							_changedList.Add(chunk);
						}
						int nextIndex = (x * Chunk.chunkDepth + z) * Chunk.chunkHeight + y; 
						_lightBfsQueue.Enqueue(NodeCache.Instance.GetShrinkNode(nextIndex,lightLevel,nextLightLevel,chunk));
					}
					//如果前一个物块比当前物块的太阳光暗，那么增强前一个物块的亮度
					else if(temp < lightLevel)
					{
						int nextIndex = (x * Chunk.chunkDepth + z) * Chunk.chunkHeight + y; 
						_lightSpread.AddSpreadNode(NodeCache.Instance.GetSpreadNode(nextIndex,lightLevel,chunk));
					}
				}
			}
		}
		
		
		private void SpreadInPosDown(int x,int y,int z,Chunk chunk,int prevLightLevel,int curLightLevel)
		{
			if(curLightLevel >= WorldConfig.Instance.maxLightLevel)return;
			//当前高度大于光照高度，往下收缩，不管亮度是否大于自己
			if(y >= chunk.GetHeight(x,z,true))return;
			Block b = chunk.GetBlock(x,y,z,true);
			BlockAttributeCalculator calculator = BlockAttributeCalculatorFactory.GetCalculator(b.BlockType);
			int lightDamp = calculator.LightDamp(b.ExtendId);
			if(lightDamp < WorldConfig.Instance.maxLightLevel)
			{
				int lightLevel= chunk.GetSunLight(x,y,z);
				if(lightLevel > 0)
				{
					int temp = prevLightLevel - lightDamp;
					//向下收缩，考虑到都为15的情况，相等的情况下也收缩掉
					if(temp > lightLevel || (temp <= WorldConfig.Instance.maxLightLevel && lightLevel == WorldConfig.Instance.maxLightLevel))
					{
						int nextLightLevel = curLightLevel - lightDamp - 1;
						if(nextLightLevel < 0)
						{
							nextLightLevel = 0;
						}
						if(nextLightLevel == lightLevel)return;
						chunk.SetSunLight(x,y,z,nextLightLevel,true);
						if(!_changedList.Contains(chunk))
						{
							_changedList.Add(chunk);
						}
						int nextIndex = (x * Chunk.chunkDepth + z) * Chunk.chunkHeight + y; 
						if(temp <= WorldConfig.Instance.maxLightLevel && lightLevel == WorldConfig.Instance.maxLightLevel)
							lightLevel = nextLightLevel;
						_lightBfsQueue.Enqueue(NodeCache.Instance.GetShrinkNode(nextIndex,lightLevel,nextLightLevel,chunk));
					}
					//如果前一个物块比当前物块的太阳光暗，那么增强前一个物块的亮度
					else if(temp < lightLevel)
					{
						int nextIndex = (x * Chunk.chunkDepth + z) * Chunk.chunkHeight + y; 
						_lightSpread.AddSpreadNode(NodeCache.Instance.GetSpreadNode(nextIndex,lightLevel,chunk));
					}
				}
			}
		}
	}
}

