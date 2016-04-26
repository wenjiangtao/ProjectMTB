using System;
namespace MTB
{
	public class ArraysCacheManager
	{
		public ArraysCacheManager ()
		{
		}

		private static ArraysCache[] ARRAYS_CACHES = InitCache();

		private static ArraysCache[] InitCache()
		{
			ArraysCache[] temp = new ArraysCache[4];
			for (int i = 0; i < temp.Length; i++)
				temp[i] = new ArraysCache();
			return temp;
		}
		
		public static ArraysCache GetCache()
		{
			lock (ARRAYS_CACHES)
			{
				for (int i = 0; i < ARRAYS_CACHES.Length; i++) {
					if (ARRAYS_CACHES[i].isFree)
					{
						ARRAYS_CACHES[i].isFree = false;
						return ARRAYS_CACHES[i];
					}
				}
			}
			return null;
		}
		
		public static void ReleaseCache(ArraysCache cache)
		{
			lock(ARRAYS_CACHES)
			{
				cache.release();
			}
		}

	}
}

