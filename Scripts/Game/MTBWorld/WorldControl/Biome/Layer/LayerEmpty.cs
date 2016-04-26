using System;
namespace MTB
{
	public class LayerEmpty : Layer
	{
		public LayerEmpty(long seed)
			:base(seed)
		{
		}
		 
		public override int[] getInts(ArraysCache cache, int x, int z, int xSize, int zSize)
		{
			int[] thisInts = cache.getArray(xSize * zSize);
			for (int i = 0; i < thisInts.Length; i++)
				thisInts[i] = 0;
			return thisInts;
		}
	}
}

