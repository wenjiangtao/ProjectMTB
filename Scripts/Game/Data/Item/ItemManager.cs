using System;
using System.Xml;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
namespace MTB
{
	public class ItemManager
	{
		private static string ItemPath = "Data/Item/Items";
		private static string InitUserItemPath = "Data/Item/UserItems";
		private static ItemManager _instance;
		public static ItemManager Instance{get{
				if(_instance == null)_instance = new ItemManager();
				return _instance;
			}}
		private Dictionary<int,Item> _itemMap;
		private Dictionary<int,Item> _blockIdToItemMap;
		private List<UserItemData> userItemDatas;
		public ItemManager ()
		{
			_itemMap = new Dictionary<int, Item>();
			_blockIdToItemMap = new Dictionary<int, Item>();
			userItemDatas = new List<UserItemData>();
		}

		public void Init()
		{
			XmlDocument itemXml = new XmlDocument();
			itemXml.LoadXml(Resources.Load(ItemPath).ToString());
			XmlNodeList list = itemXml.DocumentElement.GetElementsByTagName("Model");
			for (int i = 0; i < list.Count; i++) {
				Item item = new Item(list[i] as XmlElement);
				_itemMap.Add(item.id,item);
				int blockId = BlockData.GetBlockId(item.sceneBlockType,item.sceneBlockExtendId);
				if(blockId > 0)
				{
					_blockIdToItemMap.Add(blockId,item);
				}
			}

			string userItemInfo = Resources.Load(InitUserItemPath).ToString();
			JsonData jsonData = JsonMapper.ToObject(userItemInfo);
			for (int i = 0; i < jsonData.Count; i++) {
				UserItemData data = new UserItemData(jsonData[i]);
				userItemDatas.Add(data);
			}
		}

		public int GetItemCount()
		{
			return _itemMap.Count;
		}

		public List<UserItemData> GetInitUserItemData()
		{
			return userItemDatas;
		}

		public Item GetItem(int id)
		{
			Item item;
			_itemMap.TryGetValue(id,out item);
			return item;
		}

		public Item GetItemByBlockId(int id)
		{
			Item item;
			_blockIdToItemMap.TryGetValue(id,out item);
			return item;
		}

		public Item GetItemByBlockType(byte blockType,byte extendId)
		{
			int id = BlockData.GetBlockId(blockType,extendId);
			return GetItemByBlockId(id);
		}
	}
}

