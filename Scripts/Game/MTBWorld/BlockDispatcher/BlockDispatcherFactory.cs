using System;
using System.Collections.Generic;
namespace MTB
{
	public class BlockDispatcherFactory
	{
		private static Dictionary<BlockType,BlockDispatcher> _map = InitMap();
		private static BlockDispatcher _normalDispatcher = new BlockDispatcher();
		public BlockDispatcherFactory ()
		{
		}

		private static Dictionary<BlockType,BlockDispatcher> InitMap()
		{
			Dictionary<BlockType,BlockDispatcher> map = new Dictionary<BlockType, BlockDispatcher>();
			map.Add(BlockType.FlowingWater,new BD_FlowingWater());
			map.Add(BlockType.Block_100,new BD_Block_100());
			return map;
		}

		public static BlockDispatcher GetBlockDispatcher(BlockType type)
		{
			BlockDispatcher d = null;
			_map.TryGetValue(type,out d);
			if(d == null)
			{
				return _normalDispatcher;
			}
			return d;
		}

		public static BlockDispatcher GetDefaultDispatcher()
		{
			return _normalDispatcher;
		}
	}
}

