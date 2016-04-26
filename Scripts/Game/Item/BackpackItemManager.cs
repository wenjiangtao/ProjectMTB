using System;
using System.Collections.Generic;
namespace MTB
{
	public class BackpackItemManager : VolumeUserItemManager
	{
		private static BackpackItemManager _instance;
		public static BackpackItemManager Instance{get{
				if(_instance == null)_instance = new BackpackItemManager();
				return _instance;
			}}
		public BackpackItemManager ()
			:base((int)ItemPlaceType.Backpack)
		{
			int count = ItemManager.Instance.GetItemCount();
			int size = count < 30 ? 30 : (count / 6 + 1) * 6;
//			RegisterVolume(size,Item.GetItemType((int)ItemType.Block,(int)ItemBlockType.terrain));
//			RegisterVolume(size,Item.GetItemType((int)ItemType.Block,(int)ItemBlockType.plant));
//			RegisterVolume(size,Item.GetItemType((int)ItemType.CollectTool,(int)ItemCollectToolType.defalut));
			RegisterVolume(size,0);
		}

		public override void Init (params object[] param)
		{
			//初始化用户的材料信息
			List<UserItemData> list = ItemManager.Instance.GetInitUserItemData();
			for (int i = 0; i < list.Count; i++) {
				UserItemData data = list[i];
				UserItem userItem = UserItemFactory.GenerateUserItem(data.id,data.num,this.place);
				AddUserItem(userItem);
			}
		}

		protected override int GetVolumeKey (int id)
		{
//			Item item = ItemManager.Instance.GetItem(id);
//			if(item.itemType == (int)ItemType.Block)
//			{
//				return item.type;
//			}
//			else if(item.itemType == (int)ItemType.CollectTool)
//			{
//				return Item.GetItemType(item.itemType,(int)ItemCollectToolType.defalut);
//			}
			return 0;
		}
	}
}

