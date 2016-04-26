using System;
namespace MTB
{
	public class Decoration_HollowTree :DecorationRemoveBase, IDecoration
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

			int treeHeight = treeTrunkHeight + treeLeaveHeight + treeLeaveOffset;
			int treeTrunkStartHeight = y;
			int treeTrunkNextHeight = y + treeTrunkHeight;
			int treeLeaveStartHeight = y + treeTrunkHeight + treeLeaveOffset;
			int treeLeaveNextHeight = treeLeaveStartHeight + treeLeaveHeight;

			bool isTrunkWidthEvenNum = (treeTrunkWidth % 2 == 0);
			bool isLeaveWidthEvenNum = (treeLeaveWidth % 2 == 0);
			//调整树叶宽度
			if(isTrunkWidthEvenNum != isLeaveWidthEvenNum)
			{
				treeLeaveWidth++;
				isLeaveWidthEvenNum = !isLeaveWidthEvenNum;
			}

			int halfTreeTrunkWidth = treeTrunkWidth / 2;
			int trunkWidthStartX;
			int trunkWidthStartZ;
			int trunkWidthNextX;
			int trunkWidthNextZ;
			trunkWidthStartX = x - halfTreeTrunkWidth;
			trunkWidthStartZ = z - halfTreeTrunkWidth;
			trunkWidthNextX = x + halfTreeTrunkWidth;
			trunkWidthNextZ = z + halfTreeTrunkWidth;
			if(isTrunkWidthEvenNum)
			{
				trunkWidthNextX -= 1;
				trunkWidthNextZ -= 1;
			}
			
			int halfTreeLeaveWidth = treeLeaveWidth / 2;
			int leaveWidthStartX;
			int leaveWidthStartZ;
			int leaveWidthNextX;
			int leaveWidthNextZ;
			leaveWidthStartX = x - halfTreeLeaveWidth;
			leaveWidthStartZ = z - halfTreeLeaveWidth;
			leaveWidthNextX = x + halfTreeLeaveWidth;
			leaveWidthNextZ = z + halfTreeLeaveWidth;
			if(isLeaveWidthEvenNum)
			{
				leaveWidthNextX -= 1;
				leaveWidthNextZ -= 1;
			}

			if(!CheckCanGenerator(chunk,treeTrunkStartHeight,treeTrunkNextHeight,treeLeaveStartHeight,treeLeaveNextHeight,
			                      trunkWidthStartX,trunkWidthStartZ,trunkWidthNextX,trunkWidthNextZ,leaveWidthStartX,
			                      leaveWidthStartZ,leaveWidthNextX,leaveWidthNextZ))
			{
				return false;
			}
			CreateTree(chunk,random,halfTreeTrunkWidth / 2 + 1,halfTreeLeaveWidth / 2,treeTrunkStartHeight,treeTrunkNextHeight,treeLeaveStartHeight,treeLeaveNextHeight,
			           trunkWidthStartX,trunkWidthStartZ,trunkWidthNextX,trunkWidthNextZ,leaveWidthStartX,
			           leaveWidthStartZ,leaveWidthNextX,leaveWidthNextZ);
			return true;
		}

		#endregion

		public Decoration_HollowTree ()
		{
            _decorationType = DecorationType.HollowTree;
		}

		private bool CheckCanGenerator(Chunk chunk,int treeTrunkStartHeight,int treeTrunkNextHeight,
		                               int treeLeaveStartHeight,int treeLeaveNextHeight,int trunkWidthStartX,
		                               int trunkWidthStartZ,int trunkWidthNextX,int trunkWidthNextZ,
		                               int leaveWidthStartX,int leaveWidthStartZ,int leaveWidthNextX,int leaveWidthNextZ)
		{
			for (int cx = trunkWidthStartX; cx <= trunkWidthNextX; cx++) {
				for (int cz = trunkWidthStartZ; cz <= trunkWidthNextZ; cz++) {
					if(chunk.GetBlock(cx,treeTrunkStartHeight - 1,cz).BlockType == BlockType.Air)
					{
						return false;
					}
				}
			}
			for (int cy = treeTrunkStartHeight; cy < treeLeaveNextHeight; cy++) {
				if(cy < treeLeaveStartHeight)
				{
					for (int cx = trunkWidthStartX; cx <= trunkWidthNextX; cx++) {
						for (int cz = trunkWidthStartZ; cz <= trunkWidthNextZ; cz++) {
							if(chunk.GetBlock(cx,cy,cz).BlockType != BlockType.Air)
							{
								return false;
							}
						}
					}
					continue;
				}
				for (int cx = leaveWidthStartX; cx <= leaveWidthNextX; cx++) {
					for (int cz = leaveWidthStartZ; cz <= leaveWidthNextZ; cz++) {
						if(chunk.GetBlock(cx,cy,cz).BlockType != BlockType.Air)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		private void CreateTree(Chunk chunk,IMTBRandom random,int trunkRemoveLevel,int leaveRemoveLevel,
		                        	int treeTrunkStartHeight,int treeTrunkNextHeight,
		                               int treeLeaveStartHeight,int treeLeaveNextHeight,int trunkWidthStartX,
		                        		int trunkWidthStartZ,int trunkWidthNextX,int trunkWidthNextZ,
		                               int leaveWidthStartX,int leaveWidthStartZ,int leaveWidthNextX,int leaveWidthNextZ)
		{

			int leaveWidthOffset = 0;
			for (int cy = treeLeaveStartHeight; cy < treeLeaveNextHeight; cy++) {
				int startX = leaveWidthStartX + leaveWidthOffset;
				int nextX = leaveWidthNextX - leaveWidthOffset;
				int startZ = leaveWidthStartZ + leaveWidthOffset;
				int nextZ = leaveWidthNextZ - leaveWidthOffset;
				int removeLevel = leaveRemoveLevel - leaveWidthOffset;
				for (int cx = startX; cx <= nextX; cx++) {
					for (int cz = startZ; cz <= nextZ; cz++) {
						if(Math.Abs(cx - startX) + Math.Abs(cz - startZ) <= removeLevel)continue;
						if(Math.Abs(cx - startX) + Math.Abs(cz - nextZ) <= removeLevel)continue;
						if(Math.Abs(cx - nextX) + Math.Abs(cz - startZ) <= removeLevel)continue;
						if(Math.Abs(cx - nextX) + Math.Abs(cz - nextZ) <= removeLevel)continue;
						setBlock(chunk,cx,cy,cz,new Block(BlockType.Block_41,4));
					}
				}
				leaveWidthOffset++;
			}

			for (int cy = treeTrunkStartHeight; cy < treeTrunkNextHeight; cy++) {
				for (int cx = trunkWidthStartX; cx <= trunkWidthNextX; cx++) {
					for (int cz = trunkWidthStartZ; cz <= trunkWidthNextZ; cz++) {
						bool isSide = (cx == trunkWidthStartX || cx == trunkWidthNextX || cz == trunkWidthStartZ || cz == trunkWidthNextZ);
						bool isEqualLevel = false;
						bool isGreatLevel = true;
						int disLevel;
						disLevel = Math.Abs(cx - trunkWidthStartX) + Math.Abs(cz - trunkWidthStartZ);
						isEqualLevel |= ((disLevel == trunkRemoveLevel) || (disLevel == trunkRemoveLevel - 1));
						isGreatLevel &= (disLevel > trunkRemoveLevel);
						disLevel = Math.Abs(cx - trunkWidthStartX) + Math.Abs(cz - trunkWidthNextZ);
						isEqualLevel |= ((disLevel == trunkRemoveLevel) || (disLevel == trunkRemoveLevel - 1));
						isGreatLevel &= (disLevel > trunkRemoveLevel);
						disLevel = Math.Abs(cx - trunkWidthNextX) + Math.Abs(cz - trunkWidthStartZ);
						isEqualLevel |= ((disLevel == trunkRemoveLevel) || (disLevel == trunkRemoveLevel - 1));
						isGreatLevel &= (disLevel > trunkRemoveLevel);
						disLevel = Math.Abs(cx - trunkWidthNextX) + Math.Abs(cz - trunkWidthNextZ);
						isEqualLevel |= ((disLevel == trunkRemoveLevel) || (disLevel == trunkRemoveLevel - 1));
						isGreatLevel &= (disLevel > trunkRemoveLevel);
						if(isEqualLevel || (isSide && isGreatLevel))
						{
							setBlock(chunk,cx,cy,cz,new Block(BlockType.Block_19,4));
						}
					}
				}
			}
		}
	}
}

