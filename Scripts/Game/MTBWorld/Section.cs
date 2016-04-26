using System;
using System.IO;
namespace MTB
{
	public class Section : IDataSerialize,IDisposable
	{
		#region IDataSerialize implementation

		public void Serialize (Stream stream)
		{
			stream.Write(blockTypes,0,blockTypes.Length);
			extendIds.WriteBytes(stream);
			sunLight.WriteBytes(stream);
			blockLight.WriteBytes(stream);
			Serialization.WriteIntToStream(stream,visibleBlocks);
			Serialization.WriteIntToStream(stream,specialBlocks);
		}

		public void Deserialize (Stream stream)
		{
			stream.Read(blockTypes,0,blockTypes.Length);
			extendIds.ReadBytes(stream);
			sunLight.ReadBytes(stream);
			blockLight.ReadBytes(stream);
			visibleBlocks = Serialization.ReadIntFromStream(stream);
			specialBlocks = Serialization.ReadIntFromStream(stream);
		}

		#endregion

		#region IDisposable implementation

		public void Dispose ()
		{
			visibleBlocks = 0;
			specialBlocks = 0;
		}

		#endregion

		public const int sectionHeight = 16;
		public readonly int totalBlocks;
		public Chunk chunk;
		public int chunkOffsetY;
		private byte[] blockTypes;
		private NibbleArray extendIds;
		private NibbleArray sunLight;
		private NibbleArray blockLight;

		//维护一份可视化物块的数量
		private int visibleBlocks;
		private int specialBlocks;

		public Section()
		{
			totalBlocks = Chunk.chunkWidth * Chunk.chunkDepth * sectionHeight;
			sunLight = new NibbleArray(totalBlocks,4);
			blockLight = new NibbleArray(totalBlocks,4);
			blockTypes = new byte[totalBlocks];
			extendIds = new NibbleArray(totalBlocks,4);
		}
		//清除光照数据
		public void InitData()
		{
			Array.Clear(blockTypes,0,blockTypes.Length);
			extendIds.Clear();
			sunLight.Clear();
			blockLight.Clear();
//			for (int x = 0; x < Chunk.chunkWidth; x++) {
//				for (int y = 0; y < sectionHeight; y++) {
//					for (int z = 0; z < Chunk.chunkDepth; z++) {
//						sunLight.SetData(x,y,z,0);
//						blockLight.SetData(x,y,z,0);
//						SetRealBlock(x,y,z,Block.AirBlock);
//					}
//				}
//			}
		}

		public void ClearLight()
		{
			sunLight.Clear();
			blockLight.Clear();
		}

		public Block GetBlock(int x,int y,int z,bool isInRange = false)
		{
			if(!isInRange && !IsInRange(x,y,z))
			{
				return chunk.GetBlock(x,chunkOffsetY + y,z);
			}
			return GetRealBlock(x,y,z);
		}
		public void SetBlock(int x,int y,int z,Block block,bool isInRange = false)
		{
			if(!isInRange && !IsInRange(x,y,z))
			{
				chunk.SetBlock(x,chunkOffsetY + y,z,block);
			}
			else
			{
				Block b = GetRealBlock(x,y,z);
				BlockAttributeCalculator old = BlockAttributeCalculatorFactory.GetCalculator(b.BlockType);
				BlockRenderType oldRenderType = old.GetBlockRenderType(b.ExtendId);
				if(oldRenderType != BlockRenderType.None)
				{
					visibleBlocks--;
				}

				if(oldRenderType == BlockRenderType.Part)
				{
					specialBlocks--;
				}
				BlockAttributeCalculator cur = BlockAttributeCalculatorFactory.GetCalculator(block.BlockType);
				BlockRenderType curRenderType = cur.GetBlockRenderType(block.ExtendId);
				if(curRenderType != BlockRenderType.None)
				{
					visibleBlocks++;
				}
				if(curRenderType == BlockRenderType.Part)
				{
					specialBlocks++;
				}
				SetRealBlock(x,y,z,block);
				chunk.isUpdate = true;
			}
		}

		public SectionVisible AllCanVisible()
		{
			if(specialBlocks == 0)
			{
				if(visibleBlocks == 0)
				{
					return SectionVisible.AllInvisible;
				}
				else if(visibleBlocks == totalBlocks)
				{
					return SectionVisible.AllVisible;
				}
			}
			return SectionVisible.SomeVisible;
		}

		public int GetSunLight(int x,int y,int z,bool isInRange = false)
		{
			if(!isInRange && !IsInRange(x,y,z))
			{
				return chunk.GetSunLight(x,y + chunkOffsetY,z);
			}
			return sunLight.GetData(x,y,z);
		}

		public void SetSunLight(int x,int y,int z,int value,bool isInRange = false)
		{
			if(!isInRange && !IsInRange(x,y,z))
			{
				chunk.SetSunLight(x,y + chunkOffsetY,z,value);
			}
			else
			{
				sunLight.SetData(x,y,z,value);
				chunk.isUpdate = true;
			}
		}

		public int GetBlockLight(int x,int y,int z,bool isInRange = false)
		{
			if(!isInRange && !IsInRange(x,y,z))
			{
				return chunk.GetBlockLight(x,y + chunkOffsetY,z);
			}
			return blockLight.GetData(x,y,z);
		}

		public void SetBlockLight(int x,int y,int z,int value,bool isInRange = false)
		{
			if(!isInRange && !IsInRange(x,y,z))
			{
				chunk.SetBlockLight(x,y + chunkOffsetY,z,value);
			}
			else
			{
				blockLight.SetData(x,y,z,value);
				chunk.isUpdate = true;
			}
		}

		private bool IsInRange(int x,int y,int z)
		{
			if(x < 0 || x >= Chunk.chunkWidth || y < 0 || y >= sectionHeight || z < 0 || z >= Chunk.chunkDepth)
			{
				return false;
			}
			return true;
		}

		private Block GetRealBlock(int x,int y,int z)
		{
			int index = ((x * Chunk.chunkDepth) + z) * sectionHeight + y;
			BlockType type = (BlockType)blockTypes[index];
			byte extendId = (byte)extendIds.GetData(x,y,z);
			return new Block(type,extendId);
		}

		private void SetRealBlock(int x,int y,int z,Block block)
		{
			int index = ((x * Chunk.chunkDepth) + z) * sectionHeight + y;
			blockTypes[index] = (byte)block.BlockType;
			extendIds.SetData(x,y,z,block.ExtendId);
		}
	}

	public enum SectionVisible
	{
		AllVisible,
		AllInvisible,
		SomeVisible
	}
}

