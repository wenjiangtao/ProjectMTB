using System;
namespace MTB
{
	public class Decoration_CrystalCluster :DecorationRemoveBase, IDecoration
	{
		#region IDecoration implementation

		public bool Decorade (Chunk chunk, int x, int y, int z, IMTBRandom random)
		{
			int height = random.Range(3,8);
			int width = 2;
			for (int cx = 0; cx < width; cx++) {
				for (int cz = 0; cz < width; cz++) {
					if(chunk.GetBlock(cx + x,y - 1,cz + z).BlockType == BlockType.Air)
						return false;
				}
			}
			for (int cy = y; cy < y + height; cy++) {
				for (int cx = 0; cx < width; cx++) {
					for (int cz = 0; cz < width; cz++) {
						if(chunk.GetBlock(x + cx,cy,z + cz).BlockType != BlockType.Air)
						{
							return false;
						}
					}
				}
			}
			for (int cy = y; cy < y + height; cy++) {
				for (int cx = 0; cx < width; cx++) {
					for (int cz = 0; cz < width; cz++) {
						if(chunk.GetBlock(x + cx,cy,z + cz).BlockType == BlockType.Air)
						{
							setBlock(chunk,x + cx,cy,z + cz,new Block(BlockType.Block_31));
						}
					}
				}
			}
			return true;
		}

		#endregion

		public Decoration_CrystalCluster ()
		{
		}
	}
}

