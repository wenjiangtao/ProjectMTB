using System;
namespace MTB
{
	public class Decoration_DragonFruit :DecorationRemoveBase, IDecoration
	{
		#region IDecoration implementation

		public bool Decorade (Chunk chunk, int x, int y, int z, IMTBRandom random)
		{
			int treeHeight = random.Range(5,8);
			int treeWidth = treeHeight / 2 - 1;
			if(!CheckCanGenerator(x,y,z,chunk,treeHeight,treeWidth))
			{
				return false;
			}
			CreateTree(x,y,z,chunk,treeHeight,treeWidth,random);
			return true;
		}

		#endregion

		public Decoration_DragonFruit ()
		{
		}

		private bool CheckCanGenerator(int x,int y,int z,Chunk chunk,int treeHeight,int treeWidth)
		{
			if(chunk.GetBlock(x,y,z).BlockType != BlockType.Air)
			{
				return false;
			}
			for (int cy = y + 1; cy < y + treeHeight; cy++) {
				for (int cx = x - treeWidth; cx <= x + treeWidth; cx++) {
					for (int cz = z - treeWidth; cz <= z + treeWidth; cz++) {
						if(chunk.GetBlock(cx,cy,cz).BlockType != BlockType.Air)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		private void CreateTree(int x,int y,int z,Chunk chunk,int treeHeight,int treeWidth,IMTBRandom random)
		{
			int leaveWidth = random.Range(1,3);
			for (int cx = x - treeWidth; cx <= x + treeWidth; cx++) {
				for (int cz = z - treeWidth; cz <= z + treeWidth; cz++) {
					int xDis = Math.Abs(cx - x);
					int zDis = Math.Abs(cz - z);
					int dis =  xDis+ zDis;
					int curHeight = treeHeight - dis * 2;
					int yMaxHeight = y + dis + curHeight;
					if(xDis <= leaveWidth || zDis <= leaveWidth)
					{
						setBlock(chunk,cx,y + treeHeight - 1,cz,new Block(BlockType.Block_45));
					}
					if(curHeight < 2)continue;
					for (int cy = y + dis; cy < yMaxHeight; cy++) {
						if(cy != yMaxHeight - 1 || dis != 0)
						{
						    setBlock(chunk,cx,cy,cz,new Block(BlockType.Block_44));
						}
					}
				}
			}
		}

	}
}

