using System;
namespace MTB
{
	//使陆地失真
	public class LayerLandRandom : Layer
	{
		public LayerLandRandom(long seed, Layer childLayer)
			:base(seed)
		{
			this.child = childLayer;
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
					int nwCheck = childInts[(xi + 0 + (zi) * xSize0)] & LandBit;
					int neCheck = childInts[(xi + 2 + (zi) * xSize0)] & LandBit;
					int swCheck = childInts[(xi + 0 + (zi + 2) * xSize0)] & LandBit;
					int seCheck = childInts[(xi + 2 + (zi + 2) * xSize0)] & LandBit;
					int centerCheck = childInts[(xi + 1 + (zi + 1) * xSize0)] & LandBit;
					initChunkSeed(xi + x, zi + z);
					initGroupSeed(xi + x, zi + z);
					thisInts[(xi + zi * xSize)] = childInts[(xi + 1 + (zi + 1) * xSize0)] | LandBit;

					//如果当前不是陆地而周围有陆地，那么有1/3几率将当前变为陆地
					if ((centerCheck == 0) && ((nwCheck != 0) || (neCheck != 0) || (swCheck != 0) || (seCheck != 0)))
					{
						if (nextInt(3) != 0)
							thisInts[(xi + zi * xSize)] ^= LandBit;
						//如果当前是陆地，而周围有水，那么有4/5概率将当前陆地变为水
					} else if ((centerCheck > 0) && ((nwCheck == 0) || (neCheck == 0) || (swCheck == 0) || (seCheck == 0)))
					{
						if (nextInt(5) == 0)
							thisInts[(xi + zi * xSize)] ^= LandBit;
						//否则，什么都不做
					} else if (centerCheck == 0)
						thisInts[(xi + zi * xSize)] ^= LandBit;
				}
			}
			return thisInts;
		}
	}
}

