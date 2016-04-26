using System;
namespace MTB
{
	public class LayerZoomFuzzy : LayerZoom
	{
		public LayerZoomFuzzy(long seed, Layer childLayer)
			:base(seed,childLayer)
		{
		}
		
		protected override int getRandomOf4(int a, int b, int c, int d)
		{
			return this.getRandomInArray(a, b, c, d);
		}
	}
}

