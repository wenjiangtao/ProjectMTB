using System;
namespace MTB
{
	public class LayerRiverInit : Layer
	{
		public LayerRiverInit(long seed, Layer childLayer)
			:base(seed)
		{
			this.child = childLayer;
		}
		
		public override int[] getInts(ArraysCache cache, int x, int z, int xSize, int zSize)
		{
			int[] childInts = this.child.getInts(cache, x, z, xSize, zSize);
			int[] thisInts = cache.getArray(xSize * zSize);
			
			for (int zi = 0; zi < zSize; zi++)
			{
				for (int xi = 0; xi < xSize; xi++)
				{
					initChunkSeed(zi + z, xi + x);           // reversed
					int currentPiece = childInts[(xi + zi * xSize)];
//					if (nextInt(2) == 0)
//						currentPiece |= RiverBitOne;
//					else
//						currentPiece |= RiverBitTwo;
					if(nextInt(2) == 0)
					{
						currentPiece |= RiverBits;
					}
					
					thisInts[(xi + zi * xSize)] = currentPiece;
				}
			}
			return thisInts;
		}
	}
}

