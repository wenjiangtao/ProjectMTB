using System;
namespace MTB
{
	public class LayeredBiomeGenerator
	{
		private Layer _unZoomLayer;
		private Layer _biomeLayer;

		public LayeredBiomeGenerator ()
		{
			_unZoomLayer = LayerFactory.GetBaseLayer();
			_biomeLayer = LayerFactory.GetVoronoiLayer(_unZoomLayer);
		}

		public int[] GetBiomesUnZoomed(int[] biomeArray,int x,int z,int x_size,int z_size)
		{
			if((biomeArray == null) || biomeArray.Length < x_size * z_size)
			{
				biomeArray = new int[x_size * z_size];
			}

			ArraysCache cache = ArraysCacheManager.GetCache();
			int[] result = this._unZoomLayer.getInts(cache,x,z,x_size,z_size);
			ArraysCacheManager.ReleaseCache(cache);

			Array.Copy(result,0,biomeArray,0,x_size * z_size);
//			int totalLength = x_size * z_size;
//			for (int i = 0; i < totalLength; i++) {
//				int tempX = i % x_size;
//				int tempZ = i / x_size;
//
//				biomeArray[tempX,tempZ] = result[i];
//			}
			return biomeArray;

		}

		public int[] GetBiomes(int[] biomeArray,int x,int z,int x_size,int z_size)
		{
			if((biomeArray == null) || biomeArray.Length < x_size * z_size)
			{
				biomeArray = new int[x_size * z_size];
			}

			ArraysCache cache = ArraysCacheManager.GetCache();
			int[] result = this._biomeLayer.getInts(cache,x,z,x_size,z_size);
			ArraysCacheManager.ReleaseCache(cache);

			Array.Copy(result,0,biomeArray,0,x_size * z_size);

//			for (int i = 0; i < result.Length; i++) {
//				int tempX = i % x_size;
//				int tempZ = i / x_size;
//				
//				biomeArray[tempX,tempZ] = result[i];
//			}
			return biomeArray;

		}


	}
}

