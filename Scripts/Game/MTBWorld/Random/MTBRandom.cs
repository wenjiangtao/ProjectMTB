using System;
namespace MTB
{
	public class MTBRandom : IMTBRandom
	{
		#region IMTBRandom implementation

		public int Range (int min, int max)
		{
			return (int)nextInt((long)min,(long)max);
		}

		public long seed{get{return _seed;}set{_seed = value;}}

		#endregion
		private Random _random;
		private long _seed;

		public MTBRandom (int seed)
		{
			_seed = seed;
		}

		protected long nextInt(long x)
		{
			if(x <= 0)return 0;
			long i = ((_seed >> 24) % x);
			if (i < 0)
				i += x;
			_seed *= (_seed * 6364136223846793005L + 1442695040888963407L);
			return i;
		}
		
		protected long nextInt(long min,long max)
		{
			long sub = max - min;
			if(sub <= 0)return min;
			long i = ((_seed >> 24) % sub);
			if (i < 0)
				i += sub;
			_seed *= (_seed * 6364136223846793005L + 1442695040888963407L);
			return i + min;
		}


		public MTBRandom() : this(0)
		{
		}
	}
}

