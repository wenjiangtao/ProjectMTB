using System;
namespace MTB
{
	public class LayerOutputDefault : Layer
	{
//		private int[] riverBiomes;
		public LayerOutputDefault (long seed, Layer childLayer)
			:base(seed)
		{
			this.child = childLayer;
//			riverBiomes = new int[WorldConfig.Instance.GetMaxBiomeCount()];
//			for (int i = 0; i < riverBiomes.Length; i++) {
//				BiomeConfig biomeConfig = WorldConfig.Instance.GetBiomeConfigOrNullById(i);
//				if(biomeConfig == null || !biomeConfig.riversEnabled)
//				{
//					riverBiomes[i] = -1;
//				}
//				else
//				{
//					riverBiomes[i] = biomeConfig.riverId;
//				}
//			}
		}

		public override int[] getInts (ArraysCache cache, int x, int z, int xSize, int zSize)
		{
			int[] childInts = this.child.getInts(cache, x, z, xSize, zSize);
			int[] thisInts = cache.getArray(xSize * zSize);
			
			int currentPiece;
			int cachedId;
			for (int zi = 0; zi < zSize; zi++)
			{
				for (int xi = 0; xi < xSize; xi++)
				{
					currentPiece = childInts[(xi + zi * xSize)];
					
					if ((currentPiece & LandBit) != 0)
						cachedId = currentPiece & BiomeBits;
					else
						cachedId = DefaultBiome.Ocean.id;
					currentPiece = cachedId;

//					if(riverBiomes[cachedId] != -1 && (currentPiece & RiverBits) != 0)
//					{
//						currentPiece = riverBiomes[cachedId];
//					}
//					else
//					{
//						currentPiece = cachedId;
//					}

					thisInts[(xi + zi * xSize)] = currentPiece;
				}
			}
			return thisInts;
		}
	}
}

