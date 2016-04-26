using System;
namespace MTB
{
	//可以看做是岛屿的产生
	public class LayerBiomeInBiome : Layer
	{
		public BiomeConfig biomeConfig;
		public int chance = 10;
		//当前岛屿生物群落周围是否有海洋
		public bool inOcean = false;
		
		public bool[] biomeIsles = new bool[1024];
		
		public LayerBiomeInBiome(long seed, Layer childLayer)
			:base(seed)
		{
			this.child = childLayer;
			for (int i = 0; i < biomeIsles.Length; i++)
				biomeIsles[i] = false;
		}
		
		public override int[] getInts(ArraysCache cache, int x, int z, int xSize, int zSize)
		{
			int x0 = x - 1;
			int z0 = z - 1;
			int xSize0 = xSize + 2;
			int zSize0 = zSize + 2;
			int[] childInts = this.child.getInts(cache, x0, z0, xSize0, zSize0);
			int[] thisInts = cache.getArray(xSize * zSize);
			
			for (int zi = 0; zi < zSize; zi++)
			{
				for (int xi = 0; xi < xSize; xi++)
				{
					initChunkSeed(xi + x, zi + z);
					int selection = childInts[(xi + 1 + (zi + 1) * xSize0)];
					
					bool spawn = false;
					if (inOcean)
					{
						int nwCheck = childInts[(xi + 0 + (zi) * xSize0)] & LandBit;
						int neCheck = childInts[(xi + 2 + (zi) * xSize0)] & LandBit;
						int swCheck = childInts[(xi + 0 + (zi + 2) * xSize0)] & LandBit;
						int seCheck = childInts[(xi + 2 + (zi + 2) * xSize0)] & LandBit;


						if (((selection & LandBit) == 0) && (nwCheck == 0) && (neCheck == 0) && (swCheck == 0) && (seCheck == 0) && nextInt(100) < chance)
						{
							selection = (selection & IceBit) | (selection & RiverBits) | LandBit | biomeConfig.biomeId | IslandBit;
							spawn = true;
						}
					}
					if (!spawn)
					{
						int nwCheck = childInts[(xi + 0 + (zi) * xSize0)] & BiomeBits;
						int neCheck = childInts[(xi + 2 + (zi) * xSize0)] & BiomeBits;
						int swCheck = childInts[(xi + 0 + (zi + 2) * xSize0)] & BiomeBits;
						int seCheck = childInts[(xi + 2 + (zi + 2) * xSize0)] & BiomeBits;

						if (biomeIsles[(selection & BiomeBits)] && biomeIsles[nwCheck] && biomeIsles[neCheck] && biomeIsles[swCheck] && biomeIsles[seCheck] && nextInt(100) < chance)
							selection = (selection & LandBit) | (selection & IceBit) | (selection & RiverBits) | biomeConfig.biomeId | IslandBit;
					}
					thisInts[(xi + zi * xSize)] = selection;
				}
			}
			return thisInts;
		}
	}
}

