using System;
using System.Collections.Generic;
namespace MTB
{
	public class MTBCompressFactory
	{
		private static Dictionary<MTBCompressType,IMTBCompress> _map = GetCompressMap();
		public MTBCompressFactory ()
		{
		}

		private static Dictionary<MTBCompressType,IMTBCompress> GetCompressMap()
		{
			Dictionary<MTBCompressType,IMTBCompress> map = new Dictionary<MTBCompressType, IMTBCompress>();
			map.Add(MTBCompressType.None,new NoneMTBCompress());
			map.Add(MTBCompressType.GZip,new GZipMTBCompress());
			map.Add(MTBCompressType.ZLib,new ZLibMTBCompress());
			return map;
		}

		public static IMTBCompress GetCompress(MTBCompressType type)
		{
			IMTBCompress compress = null;
			_map.TryGetValue(type,out compress);
			if(compress == null)throw new Exception("不存在压缩格式为" + type + "的压缩方法！");
			return compress;
		}
	}
}

