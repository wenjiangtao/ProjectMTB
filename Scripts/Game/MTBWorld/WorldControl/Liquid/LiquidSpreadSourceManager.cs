using System;
using System.Collections;
using System.Collections.Generic;
namespace MTB
{
	public class LiquidSpreadSourceManager
	{
		private static Queue<LiquidSpreadSource> _cache = new Queue<LiquidSpreadSource>(20);

		public static LiquidSpreadSource GetSpreadSource()
		{
			if(_cache.Count > 0)
			{
				return _cache.Dequeue();
			}
			else
			{
				return new LiquidSpreadSource();
			}
		}

		public static void SaveSpreadSource(LiquidSpreadSource source)
		{
			source.Reset();
			_cache.Enqueue(source);
		}
	}
}

