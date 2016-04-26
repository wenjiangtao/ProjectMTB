using System;
namespace MTB
{
	public class LayerIce : Layer
	{
		public int rarity = 10;
		
		public LayerIce(long seed, Layer childLayer)
			:base(seed)
		{
			this.child = childLayer;
		}
		
		public LayerIce(long seed, Layer childLayer, int _rarity)
			:base(seed)
		{
			this.child = childLayer;
			this.rarity = 101 - _rarity;
		}
		
		public override int[] getInts(ArraysCache cache, int x, int z, int xSize, int zSize)
		{
			int[] childInts = this.child.getInts(cache, x, z, xSize, zSize);
			int[] thisInts = cache.getArray(xSize * zSize);
			
			for (int zi = 0; zi < zSize; zi++)
			{
				for (int xi = 0; xi < xSize; xi++)
				{
					initChunkSeed(z + zi, x + xi);      // reversed
					thisInts[(xi + zi * xSize)] = (nextInt(rarity) == 0 ? (childInts[(xi + zi * xSize)] | IceBit) : childInts[(xi + zi * xSize)]);
				}
			}
			return thisInts;
		}

	}
}

