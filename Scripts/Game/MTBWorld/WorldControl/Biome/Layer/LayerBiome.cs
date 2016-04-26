using System;
using System.Collections.Generic;
namespace MTB
{
	public class LayerBiome : Layer
	{
		private int depth;
		
		public LayerBiome(long seed, Layer childLayer, int depth)
			:base(seed)
		{
			this.child = childLayer;
			this.depth = depth;
		}
		
		public override int[] getInts(ArraysCache cache, int x, int z, int xSize, int zSize)
		{
			int[] childInts = this.child.getInts(cache, x, z, xSize, zSize);
			int[] thisInts = cache.getArray(xSize * zSize);
			
			for (int i = 0; i < zSize; i++)
			{
				for (int j = 0; j < xSize; j++)
				{
					initChunkSeed(j + x, i + z);
					int currentPiece = childInts[(j + i * xSize)];
					
					if ((currentPiece & BiomeGroupBits) != 0 && (currentPiece & BiomeBits) == 0)    // has biomegroup bits but not biome bits
					{
						BiomeGroupConfig biomeGroupConfig = WorldConfig.Instance.GetBiomeGroupConfigById((currentPiece & BiomeGroupBits) >> BiomeGroupShift);
						Dictionary<int,BiomeConfig> possibleBiomes = biomeGroupConfig.GetBiomeRarityMap(depth);
						if (possibleBiomes.Count > 0)
						{
							int newBiomeRarity = nextInt(biomeGroupConfig.getMaxRarity());
							foreach (var item in possibleBiomes) {
								if(newBiomeRarity < item.Key)
								{
									if(item.Value != null)
									{
										currentPiece |= item.Value.biomeId;
									}
									break;
								}
							}
						}
					}
					thisInts[(j + i * xSize)] = currentPiece;
				}
			}
			return thisInts;
		}
	}
}

