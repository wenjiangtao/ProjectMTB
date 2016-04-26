using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
namespace MTB
{
	public class BlockDataManager
	{
		private static string BlockPath = "Data/Block/Blocks";
		private static BlockDataManager _instance;
		public static BlockDataManager Instance{get{
				if(_instance == null)_instance = new BlockDataManager();
				return _instance;
			}}
		private Dictionary<int,BlockData> _blocksMap;
		public BlockDataManager ()
		{
			_blocksMap = new Dictionary<int, BlockData>();
		}
		
		public void Init()
		{
			XmlDocument blocksXml = new XmlDocument();
			blocksXml.LoadXml(Resources.Load(BlockPath).ToString());
			XmlNodeList list = blocksXml.DocumentElement.GetElementsByTagName("Model");
			for (int i = 0; i < list.Count; i++) {
				BlockData blockData = new BlockData(list[i] as XmlElement);
				_blocksMap.Add(blockData.id,blockData);
			}
		}
		
		public BlockData GetBlockData(byte type,byte extendId)
		{
			int id = BlockData.GetBlockId(type,extendId);
			return GetBlockData(id);
		}
		public BlockData GetBlockData(int id)
		{
			BlockData blockData;
			_blocksMap.TryGetValue(id,out blockData);
			return blockData;
		}
	}
}

