using System;
namespace MTB
{
	public class LayerBiomeBorder : Layer
	{
		//可以替换生物群落的信息，例如：id为a的生物群落是否可以替换id为b的生物群落,即bordersFrom[a][b] == true
		private bool[][] bordersFrom;
		//当前生物群落可以替换成哪种生物群落，例如：id为a的生物群落可以替换为id为b的生物群落，即bordersTo[a] = b
		private int[] bordersTo;
		
		public LayerBiomeBorder(long seed)
			:base(seed)
		{
			this.bordersFrom = new bool[WorldConfig.Instance.GetMaxBiomeCount()][];
			this.bordersTo = new int[WorldConfig.Instance.GetMaxBiomeCount()];
		}

		//borderBiomeConfig为当前的边界生物群落，replaceBiomeId当前的边界群落能替换成的生物群落id
		public void addBiome(BiomeConfig borderBiomeConfig, int replaceBiomeId)
		{
			this.bordersFrom[replaceBiomeId] = new bool[WorldConfig.Instance.GetMaxBiomeCount()];
			
			for (int i = 0; i < this.bordersFrom[replaceBiomeId].Length; i++)
			{
				BiomeConfig biomeConfig = WorldConfig.Instance.GetBiomeConfigOrNullById(i);
				this.bordersFrom[replaceBiomeId][i] = biomeConfig == null || !borderBiomeConfig.notBorderNear.Contains(biomeConfig.biomeId);
			}
			this.bordersTo[replaceBiomeId] = borderBiomeConfig.biomeId;
		}
		
		public override int[] getInts(ArraysCache cache, int x, int z, int xSize, int zSize)
		{
			int[] childInts = this.child.getInts(cache, x - 1, z - 1, xSize + 2, zSize + 2);
			int[] thisInts = cache.getArray(xSize * zSize);
			
			for (int zi = 0; zi < zSize; zi++)
			{
				for (int xi = 0; xi < xSize; xi++)
				{
					initChunkSeed(xi + x, zi + z);
					int selection = childInts[(xi + 1 + (zi + 1) * (xSize + 2))];
					
					int biomeId = GetBiomeFromLayer(selection);

					if (bordersFrom[biomeId] != null)
					{
						int northCheck = GetBiomeFromLayer(childInts[(xi + 1 + (zi) * (xSize + 2))]);
						int southCheck = GetBiomeFromLayer(childInts[(xi + 1 + (zi + 2) * (xSize + 2))]);
						int eastCheck = GetBiomeFromLayer(childInts[(xi + 2 + (zi + 1) * (xSize + 2))]);
						int westCheck = GetBiomeFromLayer(childInts[(xi + (zi + 1) * (xSize + 2))]);

						bool[] biomeFrom = bordersFrom[biomeId];
						if (biomeFrom[northCheck] && biomeFrom[eastCheck] && biomeFrom[westCheck] && biomeFrom[southCheck])
							if ((northCheck != biomeId) || (eastCheck != biomeId) || (westCheck != biomeId) || (southCheck != biomeId))
								selection = (selection & (IslandBit | RiverBits | IceBit)) | LandBit | bordersTo[biomeId];
					}
					
					thisInts[(xi + zi * xSize)] = selection;
					
				}
			}
			
			return thisInts;
		}
	}
}

