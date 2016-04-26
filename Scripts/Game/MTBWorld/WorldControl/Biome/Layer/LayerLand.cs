using System;
namespace MTB
{
	public class LayerLand : Layer
	{ 
		public int rarity = 4;
		
		public LayerLand(long seed, Layer childLayer,int rarity)
			:base(seed)
		{
			child = childLayer;
			this.rarity = rarity;
		}

		public override int[] getInts(ArraysCache cache, int x, int z, int xSize, int zSize)
		{
			int[] childInts = this.child.getInts(cache, x, z, xSize, zSize);
			int[] thisInts = cache.getArray(xSize * zSize);
			
			for (int zi = 0; zi < zSize; zi++)
			{
				for (int xi = 0; xi < xSize; xi++)
				{
					initChunkSeed(x + xi, z + zi);
					if (nextInt(100) < rarity)
						thisInts[(xi + zi * xSize)] = childInts[(xi + zi * xSize)] | LandBit;
					else
						thisInts[(xi + zi * xSize)] = childInts[(xi + zi * xSize)];
				}
			}
			return thisInts;
		}
	}
}

