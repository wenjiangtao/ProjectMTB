using System;
using System.Collections.Generic;
namespace MTB
{
	public class LayerBiomeGroups : Layer
	{
		private int depth;
		
		public LayerBiomeGroups(long seed,Layer childLayer, int depth)
			:base(seed)
		{
			this.child = childLayer;
			this.depth = depth;
		}
		
		public override int[] getInts(ArraysCache arraysCache, int x, int z, int x_size, int z_size)
		{
			int[] childInts = this.child.getInts(arraysCache, x, z, x_size, z_size);
			int[] thisInts = arraysCache.getArray(x_size * z_size);
			
			for (int i = 0; i < z_size; i++)
			{
				for (int j = 0; j < x_size; j++)
				{
					initGroupSeed(j + x, i + z);
					int currentPiece = childInts[(j + i * x_size)];               
					
					if ((currentPiece & LandBit) != 0 && (currentPiece & BiomeGroupBits) == 0)
					{
						Dictionary<int,BiomeGroupConfig> possibleGroups = WorldConfig.Instance.GetBiomeGroupRarityMap(depth);
						int newGroupRarity = nextGroupInt(WorldConfig.Instance.GetMaxRarity());
						foreach (var item in possibleGroups) {
							if(newGroupRarity < item.Key)
							{
								if(item.Value != null)
								{
									currentPiece |= (item.Value.groupId << BiomeGroupShift);
								}
								break;
							}
						}
					}
					thisInts[(j + i * x_size)] = currentPiece;
				}
			}
			
			return thisInts;
		}
	}
}

