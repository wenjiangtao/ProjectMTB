using System;
namespace MTB
{
	public class Decoration_SporeTree :DecorationRemoveBase, IDecoration
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
            int treeHalfLeaveWidth = trunktemp.Length > 1 ? random.Range(trunktemp[0], trunktemp[1]) : trunktemp[0];
            trunktemp = data.leafHeight;
            int treeLeaveHeight = trunktemp.Length > 1 ? random.Range(trunktemp[0], trunktemp[1]) : trunktemp[0];
            int treeLeaveOffset = data.leafOffset;

			if(!CheckCanGenerator(x,y,z,chunk,treeTrunkWidth,treeTrunkHeight,treeHalfLeaveWidth,treeLeaveHeight,treeLeaveOffset))
			{
				return false;
			}
			return CreateTree(x,y,z,chunk,treeTrunkWidth,treeTrunkHeight,treeHalfLeaveWidth,treeLeaveHeight,treeLeaveOffset,random);
		}

		#endregion

		public Decoration_SporeTree ()
		{
            _decorationType = DecorationType.SporeTree;
		}

		private bool CheckCanGenerator(int x,int y,int z,Chunk chunk,int treeTrunkWidth,int treeTrunkHeight,int treeHalfLeaveWidth,int treeLeaveHeight,int treeLeaveOffset)
		{
			int treeHeight = treeTrunkHeight + treeLeaveHeight + treeLeaveOffset;
			int treeCrownStartHeight = treeTrunkHeight + treeLeaveOffset;
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
				int nextXWidth = x + treeHalfLeaveWidth + 1;
				int nextZWidth = z + treeHalfLeaveWidth + 1;
				for (int cx = x - treeHalfLeaveWidth; cx <= nextXWidth; cx++) {
					for (int cz = z - treeHalfLeaveWidth; cz <= nextZWidth; cz++) {
						if(chunk.GetBlock(cx,cy,cz).BlockType != BlockType.Air)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		private bool CreateTree(int x,int y,int z,Chunk chunk,int treeTrunkWidth,int treeTrunkHeight,int treeHalfLeaveWidth,int treeLeaveHeight,int treeLeaveOffset,IMTBRandom random)
		{
			int treeLeaveStartHeight = y + treeTrunkHeight + treeLeaveOffset - 1;
			int treeTopHeight = treeLeaveStartHeight + treeLeaveHeight;
			int startXWidth = x - treeHalfLeaveWidth;
			int startZWidth = z - treeHalfLeaveWidth;
			int nextXWidth = x + treeHalfLeaveWidth + 1;
			int nextZWidth = z + treeHalfLeaveWidth + 1;
			for (int cy = treeLeaveStartHeight; cy < treeTopHeight; cy++) {
				for (int cx = startXWidth; cx <= nextXWidth; cx++) {
					for (int cz = startZWidth; cz <= nextZWidth; cz++) {
						setBlock(chunk,cx,cy,cz,new Block(BlockType.Block_41,7));
					}
				}
			}
			int randomNum = random.Range(1,treeHalfLeaveWidth * treeHalfLeaveWidth * 2);
			int maxHeight = treeTrunkHeight + treeLeaveOffset;
			if(maxHeight > 1)
			{
				for (int i = 0; i < randomNum; i++) {
					int randX = random.Range(startXWidth,nextXWidth + 1);
					int randZ = random.Range(startZWidth,nextZWidth + 1);
					if((randX < x - 1 || randX > x + 2) || (randZ < z - 1 || randZ > z + 2))
					{
						if(chunk.GetBlock(randX,treeLeaveStartHeight - 1,randZ).BlockType != BlockType.Air)continue;
						int num = random.Range(0,maxHeight - 1);
						for (int j = 0; j < num; j++) {
							setBlock(chunk,randX,treeLeaveStartHeight - 1 - j,randZ,new Block(BlockType.Block_41,7));
						}
					}
				}
			}

			for (int cy = y; cy < y + treeTrunkHeight - 1; cy++) {
				for (int cx = x; cx < x + treeTrunkWidth; cx++) {
					for (int cz = z; cz < z + treeTrunkWidth; cz++) {
						if(y < treeLeaveStartHeight && random.Range(0,100) < 5)
						{
							setBlock(chunk,cx,cy,cz,new Block(BlockType.Block_84));
						}
						else
						{
							setBlock(chunk,cx,cy,cz,new Block(BlockType.Block_19,7));
						}
					}
				}
			}
			return true;
		}
	}
}

