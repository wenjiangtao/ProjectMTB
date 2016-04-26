using System;
using System.Collections;
using System.Collections.Generic;
namespace MTB
{
	public class Decoration_RoughSkinTree :DecorationRemoveBase, IDecoration
	{
		#region IDecoration implementation

		public bool Decorade (Chunk chunk, int x, int y, int z, IMTBRandom random)
		{
            MTBPlantData data = MTBPlantDataManager.Instance.getData((int)_decorationType);
            int[] trunktemp = data.chunkWidth;
            int treeTrunkWidth = trunktemp.Length > 1 ? random.Range(trunktemp[0], trunktemp[1]) : trunktemp[0];
            trunktemp = data.chunkHeight;
            int treeTrunkHeight = trunktemp.Length > 1 ? random.Range(trunktemp[0], trunktemp[1]) : trunktemp[0];
            trunktemp = data.leafWidth;
            int treeLeaveWidth = trunktemp.Length > 1 ? random.Range(trunktemp[0], trunktemp[1]) : trunktemp[0];
            trunktemp = data.leafHeight;
            int treeLeaveHeight = trunktemp.Length > 1 ? random.Range(trunktemp[0], trunktemp[1]) : trunktemp[0];
            int treeLeaveOffset = data.leafOffset;

			if(!CheckCanGenerator(x,y,z,chunk,treeTrunkWidth,treeTrunkHeight,treeLeaveWidth,treeLeaveHeight,treeLeaveOffset))
			{
				return false;
			}
			return CreateTree(x,y,z,chunk,treeTrunkWidth,treeTrunkHeight,treeLeaveWidth,treeLeaveHeight,treeLeaveOffset,random);
		}

		#endregion

		public Decoration_RoughSkinTree ()
		{
            _decorationType = DecorationType.RoughSkinTree;
		}

		private bool CheckCanGenerator(int x,int y,int z,Chunk chunk,int treeTrunkWidth,int treeTrunkHeight,int treeLeaveWidth,int treeLeaveHeight,int treeLeaveOffset)
		{
			int treeHeight = treeTrunkHeight + treeLeaveHeight + treeLeaveOffset;
			int treeCrownStartHeight = treeTrunkHeight + treeLeaveOffset;
			int halfTreeLeaveWidth = treeLeaveWidth / 2;
			for (int cx = x; cx < x + treeTrunkWidth; cx++) {
				for (int cz = z; cz < z + treeTrunkWidth; cz++) {
					if(chunk.GetBlock(cx,y - 1,cz).BlockType == BlockType.Air)
					{
						return false;
					}
				}
			}
			for (int cy = y; cy < y + treeHeight; cy++) {
				if(cy - y < treeCrownStartHeight)
				{
					for (int cx = x; cx < x + treeTrunkWidth; cx++) {
						for (int cz = z; cz < z + treeTrunkWidth; cz++) {
							if(chunk.GetBlock(cx,cy,cz).BlockType != BlockType.Air)
							{
								return false;
							}
						}
					}
					continue;
				}
				int nextXWidth = x + halfTreeLeaveWidth + 1;
				int nextZWidth = z + halfTreeLeaveWidth + 1;
				for (int cx = x - halfTreeLeaveWidth; cx <= nextXWidth; cx++) {
					for (int cz = z - halfTreeLeaveWidth; cz <= nextZWidth; cz++) {
						if(chunk.GetBlock(cx,cy,cz).BlockType != BlockType.Air)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		private bool CreateTree(int x,int y,int z,Chunk chunk,int treeTrunkWidth,int treeTrunkHeight,int treeLeaveWidth,int treeLeaveHeight,int treeLeaveOffset,IMTBRandom random)
		{
			int treeLeaveStartHeight = y + treeTrunkHeight + treeLeaveOffset - 1;
			int halfTreeLeaveWidth = treeLeaveWidth / 2;
			int treeTopHeight = treeLeaveStartHeight + treeLeaveHeight;
			int leaveWidth = halfTreeLeaveWidth;
			int baseRate = 15;
			for (int cy = treeLeaveStartHeight; cy < treeTopHeight; cy++) {
				int nextXWidth = x + leaveWidth + 1;
				int nextZWidth = z + leaveWidth + 1;
				int startXWidth = x - leaveWidth;
				int startZWidth = z - leaveWidth;
				for (int cx = startXWidth; cx <= nextXWidth; cx++) {
					for (int cz = startZWidth; cz <= nextZWidth; cz++) {
						setBlock(chunk,cx,cy,cz,new Block(BlockType.Block_41,3));
					}
				}
				int rate = (int)(baseRate * (1f + ((float)leaveWidth * 2.5f) / halfTreeLeaveWidth));

				if(leaveWidth > 0)
				{
					for (int cx = startXWidth; cx <= nextXWidth; cx++) {
						if(chunk.GetBlock(cx,cy,startZWidth).BlockType == BlockType.Air)continue;
						if(random.Range(0,100) < rate)setBlock(chunk,cx,cy,startZWidth,new Block(BlockType.Air));
						else break;
					}
					for (int cx = startXWidth; cx <= nextXWidth; cx++) {
						if(chunk.GetBlock(cx,cy,nextZWidth).BlockType == BlockType.Air)continue;
						if(random.Range(0,100) < rate)setBlock(chunk,cx,cy,nextZWidth,new Block(BlockType.Air));
						else break;
					}
					for (int cx = nextXWidth; cx >= startXWidth; cx--) {
						if(chunk.GetBlock(cx,cy,startZWidth).BlockType == BlockType.Air)continue;
						if(random.Range(0,100) < rate)setBlock(chunk,cx,cy,startZWidth,new Block(BlockType.Air));
						else break;
					}
					for (int cx = nextXWidth; cx >= startXWidth; cx--) {
						if(chunk.GetBlock(cx,cy,nextZWidth).BlockType == BlockType.Air)continue;
						if(random.Range(0,100) < rate)setBlock(chunk,cx,cy,nextZWidth,new Block(BlockType.Air));
						else break;
					}

					for (int cz = startZWidth; cz <= nextZWidth; cz++) {
						if(chunk.GetBlock(startXWidth,cy,cz).BlockType == BlockType.Air)continue;
						if(random.Range(0,100) < rate)setBlock(chunk,startXWidth,cy,cz,new Block(BlockType.Air));
						else break;
					}

					for (int cz = startZWidth; cz <= nextZWidth; cz++) {
						if(chunk.GetBlock(nextXWidth,cy,cz).BlockType == BlockType.Air)continue;
						if(random.Range(0,100) < rate)setBlock(chunk,nextXWidth,cy,cz,new Block(BlockType.Air));
						else break;
					}
					for (int cz = nextZWidth; cz >= startZWidth; cz--) {
						if(chunk.GetBlock(startXWidth,cy,cz).BlockType == BlockType.Air)continue;
						if(random.Range(0,100) < rate)setBlock(chunk,startXWidth,cy,cz,new Block(BlockType.Air));
						else break;
					}
					for (int cz = nextZWidth; cz >= startZWidth; cz--) {
						if(chunk.GetBlock(nextXWidth,cy,cz).BlockType == BlockType.Air)continue;
						if(random.Range(0,100) < rate)setBlock(chunk,nextXWidth,cy,cz,new Block(BlockType.Air));
						else break;
					}
				}
				
				if(random.Range(0,100) > 20)
				{
					leaveWidth--;
				}
				if(leaveWidth < 0)break;
			}
			for (int cy = y; cy < y + treeTrunkHeight - 1; cy++) {
				for (int cx = x; cx < x + treeTrunkWidth; cx++) {
					for (int cz = z; cz < z + treeTrunkWidth; cz++) {
						setBlock(chunk,cx,cy,cz,new Block(BlockType.Block_19,3));
					}
				}
			}
			return true;
		}
	}
}

