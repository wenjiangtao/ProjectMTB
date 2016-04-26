using System;
namespace MTB
{
	public class LayerBiomeTest : Layer
	{
		private int depth;
		public LayerBiomeTest (long seed,Layer childLayer)
			:base(seed)
		{
			this.child = childLayer;
		}

		public override int[] getInts (ArraysCache cache, int x, int z, int xSize, int zSize)
		{
			int[] childInts = this.child.getInts(cache, x, z, xSize, zSize);
			int[] thisInts = cache.getArray(xSize * zSize);
			
			for (int zi = 0; zi < zSize; zi++)
			{
				for (int xi = 0; xi < xSize; xi++)
				{
					initChunkSeed(x + xi, z + zi);
					int currentPiece = childInts[(xi + zi * xSize)];

					if((currentPiece & LandBit) != 0 && (currentPiece & BiomeBits) == 0)
					{
						if (nextInt(2) == 0)
							thisInts[(xi + zi * xSize)] = (currentPiece | 1);
						else
							thisInts[(xi + zi * xSize)] = (currentPiece | 2);
					}
					else
						thisInts[(xi + zi * xSize)] = currentPiece;

				}
			}
			return thisInts;
		}
	}
}

