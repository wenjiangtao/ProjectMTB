using System;
using System.Collections.Generic;
using UnityEngine;
namespace MTB
{
	public class DropController : Singleton<DropController>
	{
		private const string resPath = "UI/Icon/ItemIcons/";
		private const string DropItemPrefab = "Prefabs/DropItem";
		private DropItem _dropItemPrefab;
		private Dictionary<int,Texture2D> _resMap;
		private Dictionary<int,DropItem> _dropItemMap;
		private bool needCheckDrop;
		public void Init()
		{
			_resMap = new Dictionary<int, Texture2D>();
			_dropItemMap = new Dictionary<int, DropItem>();
			_dropItemPrefab = Resources.Load<DropItem>(DropItemPrefab);
			needCheckDrop = !WorldConfig.Instance.isCreateMode;
		}

		public void Drop(int handId,WorldPos pos,Block block)
		{
			if(needCheckDrop)
			{
				BlockData blockData = BlockDataManager.Instance.GetBlockData((byte)block.BlockType,block.ExtendId);
				if(blockData == null)return;
				Item item  = ItemManager.Instance.GetItem(handId);
				if(item != null)
				{
					SpecialIDProduction idProduction = blockData.GetSpecialIdProduction(item.id);
					if(idProduction != null)
					{
						DropItemObj(pos,idProduction.itemId,idProduction.num);
					}
					SpecialTypeProduction typeProduction = blockData.GetSpecialTypeProduction(item.type);
					if(typeProduction != null)
					{
						DropItemObj(pos,typeProduction.itemId,typeProduction.num);
					}
				}
				List<NormalProduction> productions = blockData.normalProductions;
				for (int i = 0; i < productions.Count; i++) {
					DropItemObj(pos,productions[i].itemId,productions[i].num);
				}
			}
		}

		private Texture2D GetItemTexture(int id)
		{
			Texture2D tex;
			_resMap.TryGetValue(id,out tex);
			if(tex == null)
			{
				tex = Resources.Load<Sprite>(resPath + "Icon_Item_" + id).texture;
			}
			return tex;
		}

		private void DropItemObj(WorldPos pos,int itemId,int num)
		{
			UserItem userItem = UserItemFactory.GenerateUserItem(itemId,num,(int)ItemPlaceType.Scene);
			Texture2D tex = GetItemTexture(itemId);
			float x = UnityEngine.Random.Range(0.2f,0.8f);
			float z = UnityEngine.Random.Range(0.2f,0.8f);
			DropItem dropItem = GameObject.Instantiate(_dropItemPrefab,new Vector3(pos.x + x,pos.y + 0.5f,pos.z + z),Quaternion.identity) as DropItem;
			dropItem.transform.parent = this.transform;
			dropItem.Init(userItem,tex);
			dropItem.On_PickUp += HandleOn_PickUp;
			dropItem.On_SelfDisappear += HandleOn_SelfDisappear;
			_dropItemMap.Add(dropItem.GetInstanceID(),dropItem);
		}

		void HandleOn_SelfDisappear (DropItem dropItem)
		{
			dropItem.On_SelfDisappear -= HandleOn_SelfDisappear;
			dropItem.On_PickUp -= HandleOn_PickUp;
			_dropItemMap.Remove(dropItem.GetInstanceID());
			GameObject.Destroy(dropItem.gameObject);
		}

		void HandleOn_PickUp (DropItem dropItem, GameObject player)
		{
			if(BackpackItemManager.Instance.GetLeaveNum(dropItem.userItem) < 0)return;
			BackpackItemManager.Instance.AddUserItem(dropItem.userItem);
			dropItem.On_PickUp -= HandleOn_PickUp;
			dropItem.FlyToPlayer(player);
		}

		void OnDestroy()
		{
			foreach (var item in _dropItemMap) {
				item.Value.On_PickUp -= HandleOn_PickUp;
				item.Value.On_SelfDisappear -= HandleOn_SelfDisappear;
			}
			_dropItemMap.Clear();
			_resMap.Clear();
			_dropItemPrefab = null;
		}
	}
}

