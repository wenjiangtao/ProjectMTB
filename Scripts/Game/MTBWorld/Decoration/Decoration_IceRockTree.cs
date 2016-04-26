using System;
namespace MTB
{
	public class Decoration_IceRockTree :DecorationRemoveBase, IDecoration
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

		public Decoration_IceRockTree ()
		{
            _decorationType = DecorationType.IceRockTree;
		}

		private bool CheckCanGenerator(int x,int y,int z,Chunk chunk,int treeTrunkWidth,int treeTrunkHeight,int treeHalfLeaveWidth,int treeLeaveHeight,int treeLeaveOffset)
		{
			int treeHeight = treeTrunkHeight + treeLeaveHeight + treeLeaveOffset;
			int treeCrownStartHeight = treeTrunkHeight + treeLeaveOffset;
			for (int cy = y; cy < y + treeHeight; cy++) {
				if(cy - y < treeCrownStartHeight)
				{
					if(chunk.GetBlock(x,cy,z).BlockType != BlockType.Air)
					{
						return false;
					}
					continue;
				}
				for (int cx = x - treeHalfLeaveWidth; cx <= z + treeHalfLeaveWidth; cx++) {
					for (int cz = z - treeHalfLeaveWidth; cz <=z + treeHalfLeaveWidth; cz++) {
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
			int rate = 10;

			int nextXWidth = x + treeHalfLeaveWidth;
			int nextZWidth = z + treeHalfLeaveWidth;
			int startXWidth = x - treeHalfLeaveWidth;
			int startZWidth = z - treeHalfLeaveWidth;
			for (int cy = treeLeaveStartHeight; cy < treeTopHeight; cy++) {
				for (int cx = startXWidth; cx <= nextXWidth; cx++) {
					for (int cz = startZWidth; cz <= nextZWidth; cz++) {
						int dis = Math.Abs(cx - x) + Math.Abs(cz - z);

						if(random.Range(0,100) < rate * (dis * 1.5f))
						{
							continue;
						}
						setBlock(chunk,cx,cy,cz,new Block(BlockType.Block_41,5));
					}
				}
				
//				for (int cx = startXWidth; cx <= nextXWidth; cx++) {
//					if(chunk.GetBlock(cx,cy,startZWidth).BlockType == BlockType.Air)continue;
//					if(random.Range(0,100) < rate)chunk.SetBlock(cx,cy,startZWidth,new Block(BlockType.Air));
//					else break;
//				}
//				for (int cx = startXWidth; cx <= nextXWidth; cx++) {
//					if(chunk.GetBlock(cx,cy,nextZWidth).BlockType == BlockType.Air)continue;
//					if(random.Range(0,100) < rate)chunk.SetBlock(cx,cy,nextZWidth,new Block(BlockType.Air));
//					else break;
//				}
//				for (int cx = nextXWidth; cx >= startXWidth; cx--) {
//					if(chunk.GetBlock(cx,cy,startZWidth).BlockType == BlockType.Air)continue;
//					if(random.Range(0,100) < rate)chunk.SetBlock(cx,cy,startZWidth,new Block(BlockType.Air));
//					else break;
//				}
//				for (int cx = nextXWidth; cx >= startXWidth; cx--) {
//					if(chunk.GetBlock(cx,cy,nextZWidth).BlockType == BlockType.Air)continue;
//					if(random.Range(0,100) < rate)chunk.SetBlock(cx,cy,nextZWidth,new Block(BlockType.Air));
//					else break;
//				}
//				
//				for (int cz = startZWidth; cz <= nextZWidth; cz++) {
//					if(chunk.GetBlock(startXWidth,cy,cz).BlockType == BlockType.Air)continue;
//					if(random.Range(0,100) < rate)chunk.SetBlock(startXWidth,cy,cz,new Block(BlockType.Air));
//					else break;
//				}
//				
//				for (int cz = startZWidth; cz <= nextZWidth; cz++) {
//					if(chunk.GetBlock(nextXWidth,cy,cz).BlockType == BlockType.Air)continue;
//					if(random.Range(0,100) < rate)chunk.SetBlock(nextXWidth,cy,cz,new Block(BlockType.Air));
//					else break;
//				}
//				for (int cz = nextZWidth; cz >= startZWidth; cz--) {
//					if(chunk.GetBlock(startXWidth,cy,cz).BlockType == BlockType.Air)continue;
//					if(random.Range(0,100) < rate)chunk.SetBlock(startXWidth,cy,cz,new Block(BlockType.Air));
//					else break;
//				}
//				for (int cz = nextZWidth; cz >= startZWidth; cz--) {
//					if(chunk.GetBlock(nextXWidth,cy,cz).BlockType == BlockType.Air)continue;
//					if(random.Range(0,100) < rate)chunk.SetBlock(nextXWidth,cy,cz,new Block(BlockType.Air));
//					else break;
//				}
			}
			for (int cy = y; cy < y + treeTrunkHeight - 1; cy++) {
				setBlock(chunk,x,cy,z,new Block(BlockType.Block_19,5));
			}
			return true;
		}


	}
}

