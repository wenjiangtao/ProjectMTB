using System;
namespace MTB
{
	public class Decoration_DeadWood :DecorationRemoveBase, IDecoration
	{
		#region IDecoration implementation

		public bool Decorade (Chunk chunk, int x, int y, int z, IMTBRandom random)
		{
			int treeTrunkWidth = random.Range(1,3);
			int treeTrunkHeight = random.Range(2,5);

			for (int cx = x; cx < x + treeTrunkWidth; cx++) {
				for (int cz = z; cz < z + treeTrunkWidth; cz++) {
					if(chunk.GetBlock(cx,y - 1,cz).BlockType == BlockType.Air)
					{
						return false;
					}
				}
			}
			for (int cy = y; cy < y + treeTrunkHeight; cy++) {
				for (int cx = x; cx < x + treeTrunkWidth; cx++) {
					for (int cz = z; cz < z + treeTrunkWidth; cz++) {
						if(chunk.GetBlock(cx,cy,cz).BlockType != BlockType.Air)
						{
							return false;
						}
					}
				}
			}
			for (int cy = y; cy < y + treeTrunkHeight; cy++) {
				for (int cx = x; cx < x + treeTrunkWidth; cx++) {
					for (int cz = z; cz < z + treeTrunkWidth; cz++) {
						setBlock(chunk,cx,cy,cz,new Block(BlockType.Block_19,6));
					}
				}
			}
			return true;
		}

		#endregion

		public Decoration_DeadWood ()
		{
		}
	}
}

