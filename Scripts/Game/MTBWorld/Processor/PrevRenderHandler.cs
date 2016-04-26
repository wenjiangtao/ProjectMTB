using System;
using System.Collections.Generic;
namespace MTB
{
	public class PrevRenderHandler
	{
		private DataProcessorManager _manager;
//		private LightSpread _sunLightSpread;
//		private LightShrink _sunLightShrink;

		private SunLightSpread _sunLightSpread;
		private SunLightShrink _sunLightShrink;

//		private LightSpread _blockLightSpread;
//		private LightShrink _blockLightShrink;
		private BlockLightSpread _blockLightSpread;
		private BlockLightShrink _blockLightShrink;
		private List<Chunk> _chunkChangedList;
		public PrevRenderHandler (DataProcessorManager manager)
		{
			_manager = manager;
//			_sunLightSpread = new LightSpread(_manager.World);
//			_sunLightShrink = new LightShrink(_manager.World);

			_sunLightSpread = new SunLightSpread(_manager.World);
			_sunLightShrink = new SunLightShrink(_manager.World);

//			_blockLightSpread = new LightSpread(_manager.World,false);
//			_blockLightShrink = new LightShrink(_manager.World,false);
			_blockLightSpread = new BlockLightSpread(_manager.World);
			_blockLightShrink = new BlockLightShrink(_manager.World);
			_chunkChangedList = new List<Chunk>();
		}
		public List<Chunk> HandlePrevRenderChunk(PrevRenderChunk prevChunk)
		{
			_chunkChangedList.Clear();
			Chunk chunk = prevChunk.chunk;
			int x = prevChunk.x;
			int y = prevChunk.y;
			int z = prevChunk.z;
			Block b = prevChunk.block;
			_chunkChangedList = GetAroundChangeChunks(chunk,x,y,z,b,_chunkChangedList);
			_chunkChangedList = GetSunLightChangeChunks(chunk,x,y,z,b,_chunkChangedList);
			_chunkChangedList = GetBlockLightChangeChunks(chunk,x,y,z,b,_chunkChangedList);
			return _chunkChangedList;
		}

		private List<Chunk> GetBlockLightChangeChunks(Chunk chunk,int x,int y,int z,Block b,List<Chunk> list)
		{
			BlockAttributeCalculator calculator = BlockAttributeCalculatorFactory.GetCalculator(b.BlockType);
			int curBlockLightLevel = calculator.LightLevel(b.ExtendId);
			int blockLightLevel = chunk.GetBlockLight(x,y,z,true);
			if(curBlockLightLevel > blockLightLevel)
			{
				int index = (x * Chunk.chunkDepth + z) * Chunk.chunkHeight + y;
				LightSpreadNode node = NodeCache.Instance.GetSpreadNode(index,curBlockLightLevel,chunk);
				_blockLightSpread.AddSpreadNode(node);
				chunk.SetBlockLight(x,y,z,curBlockLightLevel,true);
			}
			else if(curBlockLightLevel < blockLightLevel)
			{
				int index = (x * Chunk.chunkDepth + z) * Chunk.chunkHeight + y;
				LightShrinkNode node = NodeCache.Instance.GetShrinkNode(index,blockLightLevel,curBlockLightLevel,chunk);
				_blockLightShrink.AddShrinkNode(node);
				chunk.SetBlockLight(x,y,z,curBlockLightLevel,true);
			}
			List<Chunk> spreadList =_blockLightSpread.SpreadInChunk(chunk);
			List<Chunk> shrinkList = _blockLightShrink.ShrinkInChunk(chunk);
			for (int i = 0; i < spreadList.Count; i++) {
				if(!list.Contains(spreadList[i]))
				{
					list.Add(spreadList[i]);
				}
			}
			for (int i = 0; i < shrinkList.Count; i++) {
				if(!list.Contains(shrinkList[i]))
				{
					list.Add(shrinkList[i]);
				}
			}
			return list;
		}

		private List<Chunk> GetSunLightChangeChunks(Chunk chunk,int x,int y,int z,Block b,List<Chunk> list)
		{
			List<Chunk> spreadList = null; 
			List<Chunk> shrinkList = null;
			BlockAttributeCalculator calculator = BlockAttributeCalculatorFactory.GetCalculator(b.BlockType);
			int lightDamp = calculator.LightDamp(b.ExtendId);
			int height = chunk.GetHeight(x,z);
			int lightLevel = chunk.GetSunLight(x,y,z,true);
			int curLightLevel;
			if(y >= height - 1)
			{
				for (int ty = y; ty >= height; ty--) {
					Block nextBlock = chunk.GetBlock(x,ty,z);
					BlockAttributeCalculator nextCalculator = BlockAttributeCalculatorFactory.GetCalculator(nextBlock.BlockType);
					int nextLightDamp = nextCalculator.LightDamp(nextBlock.ExtendId);
					if(nextLightDamp > 0)
					{
						height = ty + 1;
						//更新高度
						chunk.SetHeight(x,z,height,true);
						break;
					}
				}
				curLightLevel = WorldConfig.Instance.maxLightLevel - lightDamp;
				if(curLightLevel < 0)curLightLevel = 0;
			}
			else
			{
				int leftSunLight = chunk.GetSunLight(x - 1,y,z);
				int rightSunLight = chunk.GetSunLight(x + 1,y,z);
				int topSunLight = chunk.GetSunLight(x,y + 1,z);
				int bottomSunLight = chunk.GetSunLight(x,y - 1,z);
				int frontSunLight = chunk.GetSunLight(x,y,z + 1);
				int backSunLight = chunk.GetSunLight(x,y, z - 1);
				
				int maxSunLight = GetMax(leftSunLight,rightSunLight,topSunLight,bottomSunLight,
				                         frontSunLight,backSunLight);
				curLightLevel = maxSunLight - lightDamp - 1;
				if(curLightLevel < 0)curLightLevel = 0;
			}

			if(curLightLevel < lightLevel)
			{
				chunk.SetSunLight(x,y,z,curLightLevel,true);
				int index = (x * Chunk.chunkDepth + z) * Chunk.chunkHeight + y;
				LightShrinkNode node = NodeCache.Instance.GetShrinkNode(index,lightLevel,curLightLevel,chunk);
				_sunLightShrink.AddShrinkNode(node);
				shrinkList = _sunLightShrink.ShrinkInChunk(chunk);
			}
			else if(curLightLevel > lightLevel)
			{
				chunk.SetSunLight(x,y,z,curLightLevel,true);
				int index = (x * Chunk.chunkDepth + z) * Chunk.chunkHeight + y;
				LightSpreadNode node = NodeCache.Instance.GetSpreadNode(index,curLightLevel,chunk);
				_sunLightSpread.AddSpreadNode(node);
				spreadList = _sunLightSpread.SpreadInChunk(chunk);
			}

			if(spreadList != null)
			{
				for (int i = 0; i < spreadList.Count; i++) {
					if(!list.Contains(spreadList[i]))
					{
						list.Add(spreadList[i]);
					}
				}
			}
			if(shrinkList != null)
			{
				for (int i = 0; i < shrinkList.Count; i++) {
					if(!list.Contains(shrinkList[i]))
					{
						list.Add(shrinkList[i]);
					}
				}
			}
			return list;
		}

//		private List<Chunk> GetSunLightChangeChunks(Chunk chunk,int x,int y,int z,Block b,List<Chunk> list)
//		{
//
//			List<Chunk> spreadList = new List<Chunk>(); 
//			List<Chunk> shrinkList = new List<Chunk>();
//			BlockAttributeCalculator calculator = BlockAttributeCalculatorFactory.GetCalculator(b.BlockType);
//			int lightDamp = calculator.LightDamp(b.ExtendId);
//			int height = chunk.GetHeight(x,z);
//			if(lightDamp == 0)
//			{
//				if(y < height)
//				{
//					int sunLightLevel = WorldConfig.Instance.maxLightLevel;
//					bool isMaxHeight = false;
//					for (int ty = height - 1; ty >= 0; ty--) {
//						int prevSunLightLevel = chunk.GetSunLight(x,ty,z,true);
//						if(ty < y && prevSunLightLevel == 0)break;
//
//						Block nextBlock = chunk.GetBlock(x,ty,z);
//						BlockAttributeCalculator nextCalculator = BlockAttributeCalculatorFactory.GetCalculator(nextBlock.BlockType);
//						int nextLightDamp = nextCalculator.LightDamp(nextBlock.ExtendId);
//						if(nextLightDamp > 0 && !isMaxHeight)
//						{
//							//更新高度
//							chunk.SetHeight(x,z,ty + 1,true);
//							isMaxHeight = true;
//						}
//						sunLightLevel = sunLightLevel - nextLightDamp;
//
//						if(sunLightLevel < 0)sunLightLevel = 0;
//						chunk.SetSunLight(x,ty,z,sunLightLevel,true);
//						int index = (x * Chunk.chunkDepth + z) * Chunk.chunkHeight + ty;
//						//如果当前变亮了，那么传播光照
//						if(prevSunLightLevel < sunLightLevel)
//						{
//							LightSpreadNode node = new LightSpreadNode(index,sunLightLevel,chunk);
//							_sunLightSpread.AddSpreadNode(node);
//						}
//						else
//						{
//							LightShrinkNode node = new LightShrinkNode(index,prevSunLightLevel,sunLightLevel,chunk);
//							_sunLightShrink.AddShrinkNode(node);
//						}
//						spreadList = _sunLightSpread.SpreadInChunk(chunk);
//						shrinkList = _sunLightShrink.ShrinkInChunk(chunk);
//					}
//				}
//			}
//			else
//			{
//				if(y >= height)
//				{
//					int sunLightLevel = WorldConfig.Instance.maxLightLevel;
//					Block nextBlock = chunk.GetBlock(x,y,z);
//					BlockAttributeCalculator nextCalculator = BlockAttributeCalculatorFactory.GetCalculator(nextBlock.BlockType);
//					sunLightLevel = sunLightLevel - nextCalculator.LightDamp(nextBlock.ExtendId);
//					if(sunLightLevel < 0)
//					{
//						sunLightLevel = 0;
//					}
//					int prevSunLight = chunk.GetSunLight(x,y,z,true);
//					chunk.SetSunLight(x,y,z,sunLightLevel,true);
//					int index = (x * Chunk.chunkDepth + z) * Chunk.chunkHeight + y;
//					LightShrinkNode node = new LightShrinkNode(index,prevSunLight,sunLightLevel,chunk);
//					_sunLightShrink.AddShrinkNode(node);
//					chunk.SetHeight(x,z,y + 1,true);
//					shrinkList = _sunLightShrink.ShrinkInChunk(chunk);
//					int ty;
//					for (ty = y - 1; ty >= 0; ty--) {
//						if(chunk.GetSunLight(x,ty,z,true) <= 0)
//						{
//							break;
//						}
//						else
//						{
//							_sunLightSpread.AddSpreadNode(GetSunLightSpreadNode(x - 1,ty,z,chunk));
//							_sunLightSpread.AddSpreadNode(GetSunLightSpreadNode(x + 1,ty,z,chunk));
//							_sunLightSpread.AddSpreadNode(GetSunLightSpreadNode(x,ty,z - 1,chunk));
//							_sunLightSpread.AddSpreadNode(GetSunLightSpreadNode(x,ty,z + 1,chunk));
//						}
//					}
//
//					spreadList = _sunLightSpread.SpreadInChunk(chunk);
//				}
//				else
//				{
//					int leftSunLight = chunk.GetSunLight(x - 1,y,z);
//					int rightSunLight = chunk.GetSunLight(x + 1,y,z);
//					int topSunLight = chunk.GetSunLight(x,y + 1,z);
//					int bottomSunLight = chunk.GetSunLight(x,y - 1,z);
//					int frontSunLight = chunk.GetSunLight(x,y,z + 1);
//					int backSunLight = chunk.GetSunLight(x,y, z - 1);
//
//
//					int maxSunLight = GetMax(leftSunLight,rightSunLight,topSunLight,bottomSunLight,
//					                         frontSunLight,backSunLight);
//					int prevSunLightLevel = chunk.GetSunLight(x,y,z,true);
//
//					int curSunLightLevel;
//					if(maxSunLight > prevSunLightLevel)
//					{
//						curSunLightLevel = maxSunLight - lightDamp - 1;
//					}
//					else
//					{
//						curSunLightLevel = 0;
//					}
//					chunk.SetSunLight(x,y,z,curSunLightLevel,true);
//					int index = (x * Chunk.chunkDepth + z) * Chunk.chunkHeight + y;
//					LightShrinkNode node = new LightShrinkNode(index,prevSunLightLevel,curSunLightLevel,chunk);
//					_sunLightShrink.AddShrinkNode(node);
//
//					shrinkList = _sunLightShrink.ShrinkInChunk(chunk);
//				}
//			}
//
//			for (int i = 0; i < spreadList.Count; i++) {
//				if(!list.Contains(spreadList[i]))
//				{
//					list.Add(spreadList[i]);
//				}
//			}
//			for (int i = 0; i < shrinkList.Count; i++) {
//				if(!list.Contains(shrinkList[i]))
//				{
//					list.Add(shrinkList[i]);
//				}
//			}
//			return list;
//		}

		private LightSpreadNode GetSunLightSpreadNode(int x,int y,int z,Chunk chunk)
		{
			int level = chunk.GetSunLight(x,y,z);
			int index = (x * Chunk.chunkDepth + z) * Chunk.chunkHeight + y;
			return NodeCache.Instance.GetSpreadNode(index,level,chunk);
		}

		private int GetMax(int a,int b,int c,int d,int e,int f)
		{
			int temp = Math.Max(Math.Max(a,b),Math.Max(c,d));
			int temp1 = Math.Max(e,f);
			return Math.Max(temp,temp1);
		}

		private List<Chunk> GetAroundChangeChunks(Chunk chunk,int x,int y,int z,Block b,List<Chunk> list)
		{

			//如果当前是填充物块，那么先计算当前网格数据
			if(b.BlockType != BlockType.Air)
			{
				list.Add(chunk);
			}
			if(chunk.isTerrainDataPrepared && chunk.isPopulationDataPrepared)
			{
				CheckEqualAndUpdate(x, 0,chunk.worldPos.x - 1,0,chunk.worldPos.z,list);
				CheckEqualAndUpdate(x,Chunk.chunkWidth - 1,chunk.worldPos.x + Chunk.chunkWidth,0,chunk.worldPos.z,list);
				
				CheckEqualAndUpdate(z, 0,chunk.worldPos.x,0,chunk.worldPos.z - 1,list);
				CheckEqualAndUpdate(z,Chunk.chunkDepth - 1,chunk.worldPos.x,0,chunk.worldPos.z + Chunk.chunkDepth,list);
				
			}
			//如果当前是挖掉物块，那么后计算当前网格数据
			if(!(b.BlockType != BlockType.Air))
			{
				list.Add(chunk);
			}
			return list;
		}

		private void CheckEqualAndUpdate(int a,int b,int x,int y,int z,List<Chunk> list)
		{
			if( a == b)
			{
				Chunk chunk = _manager.World.GetChunk(x,y,z);
				if(chunk != null && chunk.isPopulationDataPrepared)
				{
					list.Add(chunk);
				}
			}
		}
	}


}

