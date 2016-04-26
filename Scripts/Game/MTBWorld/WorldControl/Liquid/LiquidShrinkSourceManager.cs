using System;
using System.Collections.Generic;
namespace MTB
{
	public class LiquidShrinkSourceManager
	{
		public LiquidShrinkSourceManager ()
		{
		}

		private static Queue<LiquidShrinkSource> _cache = new Queue<LiquidShrinkSource>(20);
		
		public static LiquidShrinkSource GetShrinkSource()
		{
			if(_cache.Count > 0)
			{
				return _cache.Dequeue();
			}
			else
			{
				return new LiquidShrinkSource();
			}
		}
		
		public static void SaveShrinkSource(LiquidShrinkSource source)
		{
			source.Reset();
			_cache.Enqueue(source);
		}
	}
}

