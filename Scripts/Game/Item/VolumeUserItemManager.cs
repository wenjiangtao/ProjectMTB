using System;
using System.Collections.Generic;
namespace MTB
{
	//有容量限制的物品管理
	public class VolumeUserItemManager : PlaceUserItemManager
	{
		public delegate void OnVolumeChangedHandler(int key,int pos);
		public event OnVolumeChangedHandler On_VolumeChanged;
		private Dictionary<int,VolumeUserItem> _volumeMap;
		public VolumeUserItemManager (int place)
			:base(place)
		{
			_volumeMap = new Dictionary<int, VolumeUserItem>();
		}

		protected void RegisterVolume(int size,int key)
		{
			VolumeUserItem volume = new VolumeUserItem(size,key);
			if(!_volumeMap.ContainsKey(key))
			{
				volume.On_VolumeChanged += HandleOn_VolumeChanged;
				_volumeMap.Add(key,volume);
			}
		}

		void HandleOn_VolumeChanged (int key,int pos)
		{
			if(On_VolumeChanged != null)
			{
				On_VolumeChanged(key,pos);
			}
		}

		protected VolumeUserItem GetVolumeUserItem(int key)
		{
			VolumeUserItem volume;
			_volumeMap.TryGetValue(key,out volume);
			return volume;
		}

		protected virtual int GetVolumeKey(int id)
		{
			return 0;
		}

		public int GetLeaveNum(UserItem userItem)
		{
			VolumeUserItem volume = GetVolumeUserItem(GetVolumeKey(userItem.id));
			return volume.GetLeaveNum(userItem);
		}

		public int GetLeaveNum(int pos,UserItem userItem)
		{
			VolumeUserItem volume = GetVolumeUserItem(GetVolumeKey(userItem.id));
			return volume.GetLeaveNum(pos,userItem);
		}

		//保证当前容量能容纳添加数量，否则不会添加
		public override void AddUserItem (UserItem userItem)
		{
			VolumeUserItem volume = GetVolumeUserItem(GetVolumeKey(userItem.id));
			if(volume.GetLeaveNum(userItem) >= 0)
			{
				volume.AddUserItem(userItem);
				base.AddUserItem (userItem);
			}
		}

		//保证当前格能容纳添加数量，否则不会添加
		public void AddUserItem(int pos,UserItem userItem)
		{
			VolumeUserItem volume = GetVolumeUserItem(GetVolumeKey(userItem.id));
			if(volume.GetLeaveNum(pos,userItem) >= 0)
			{
				volume.AddUserItem(pos,userItem);
				base.AddUserItem(userItem);
			}
		}

		public override void RemoveUserItem (int id)
		{
			VolumeUserItem volume = GetVolumeUserItem(GetVolumeKey(id));
			volume.RemoveUserItemById(id,GetUserItem(id).num);
			base.RemoveUserItem (id);
		}
		//保证移除数量少于等于当前拥有量，否则不会移除
		public override void RemoveUserItem (int id, int num)
		{
			VolumeUserItem volume = GetVolumeUserItem(GetVolumeKey(id));
			if(num <= GetUserItem(id).num)
			{
				volume.RemoveUserItemById(id,num);
				base.RemoveUserItem (id, num);
			}
		}

		public void RemoveUserItemByPos(int pos,int id)
		{
			VolumeUserItem volume = GetVolumeUserItem(GetVolumeKey(id));
			volume.RemoveUserItemByPos(id,volume.GetItemNumInPos(pos));
			base.RemoveUserItem (id,volume.GetItemNumInPos(pos));
		}

		//保证移除数量少于等于当前拥有的数量，否则不会移除
		public void RemoveUserItemByPos(int pos,int id,int num)
		{
			VolumeUserItem volume = GetVolumeUserItem(GetVolumeKey(id));
			if(volume.GetItemNumInPos(num) >= num)
			{
				volume.RemoveUserItemByPos(pos,num);
				base.RemoveUserItem(id,num);
			}
		}

		public override bool UseUserItem (int id, int num, params object[] param)
		{
			if(base.UseUserItem(id,num,param))
			{
				VolumeUserItem volume = GetVolumeUserItem(GetVolumeKey(id));
				volume.RemoveUserItemById(id,num);
				return true;
			}
			return false;
		}

		public void Swap(int key,int posA,int posB)
		{
			VolumeUserItem volume = GetVolumeUserItem(key);
			if(volume != null)
			{
				volume.Swap(posA,posB);
			}
		}

		public int GetSize(int key)
		{
			VolumeUserItem volume = GetVolumeUserItem(key);
			if(volume != null)return volume.size;
			return 0;
		}

		public int GetId(int key,int pos)
		{
			VolumeUserItem volume = GetVolumeUserItem(key);
			if(volume != null)return volume.GetItemIdInPos(pos);
			return 0;
		}

		public int GetNum(int key,int pos)
		{
			VolumeUserItem volume = GetVolumeUserItem(key);
			if(volume != null)return volume.GetItemNumInPos(pos);
			return 0;
		}

		protected class VolumeUserItem
		{
			public delegate void OnVolumeChangeHandler(int key,int pos);
			public event OnVolumeChangeHandler On_VolumeChanged;
			public int key{get;private set;}
			public int size{get;private set;}
			private int[] userItemIds;
			private int[] userItemNums;
			public VolumeUserItem(int size,int key)
			{
				this.size = size;
				this.key = key;
				userItemIds = new int[size];
				userItemNums = new int[size];
			}
			//如果将该物品放入指定地点，还能剩余多少的堆叠量
			public int GetLeaveNum(int pos,UserItem userItem)
			{
				if(userItemIds[pos] == 0)
				{
					return userItem.item.maxNumPerSlot - userItem.num;
				}
				else if(userItemIds[pos] == userItem.id)
				{
					return userItem.item.maxNumPerSlot - userItemNums[pos] - userItem.num;
				}
				else
				{
					return 0 - userItem.num;
				}
			}
			//如果将该物品放入，还能剩余多少数量的堆叠量
			public int GetLeaveNum(UserItem userItem)
			{
				int leaveNum = 0;
				for (int i = 0; i < size; i++) {
					if(userItemIds[i] == userItem.id || userItemIds[i] == 0)
					{
						leaveNum += (userItem.item.maxNumPerSlot - userItemNums[i]);
					}
				}
				return leaveNum - userItem.num;
			}
			//在调用此方法时，需要调用CanAdd(UserItem userItem)保证能放入
			public void AddUserItem(UserItem userItem)
			{
				int leaveAddNum = userItem.num;
				for (int i = 0; i < size; i++) {
					if(userItemIds[i] == 0)userItemIds[i] = userItem.id;
					if(userItemIds[i] == userItem.id)
					{
						leaveAddNum -= userItem.item.maxNumPerSlot - userItemNums[i];
						if(leaveAddNum <= 0)
						{
							userItemNums[i] += userItem.num;
							SendChangedMessage(key,i);
							break;
						}
						else
						{
							userItemNums[i] = userItem.item.maxNumPerSlot;
							SendChangedMessage(key,i);
						}
					}
				}
			}
			//在调用此方法是，需要调用GetLeaveNum(int pos,UserItem userItem)保证能放入
			public void AddUserItem(int pos,UserItem userItem)
			{
				userItemIds[pos] = userItem.id;
				userItemNums[pos] = userItem.num;
				SendChangedMessage(key,pos);
			}

			//使用时，保证num的数量少于等于当前volume的总量
			public void RemoveUserItemById(int id,int num)
			{
				for (int i = size - 1; i >= 0; i--) {
					if(userItemIds[i] == id)
					{
						if(userItemNums[i] <= num)
						{
							num -= userItemNums[i];
							userItemIds[i] = 0;
							userItemNums[i] = 0;
							SendChangedMessage(key,i);
						}
						else
						{
							userItemNums[i] -= num;
							num = 0;
							SendChangedMessage(key,i);
						}
						if(num <= 0)break;
					}
				}
			}
			//使用时，保证num少于等于位置为pos的数量
			public void RemoveUserItemByPos(int pos,int num)
			{
				if(num <= 0)return;
				if(userItemNums[pos] <= num)
				{
					userItemIds[pos] = 0;
					userItemNums[pos] = 0;
				}
				else
				{
					userItemNums[pos] -= num;
				}
				SendChangedMessage(key,pos);
			}
			
			public void Swap(int posA,int posB)
			{
				Swap(userItemIds,posA,posB);
				Swap(userItemNums,posA,posB);
				SendChangedMessage(key,posA);
				SendChangedMessage(key,posB);
			}
			
			private void Swap(int[] arr,int posA,int posB)
			{
				int temp = arr[posA];
				arr[posA] = arr[posB];
				arr[posB] = temp;
			}
			
			public int GetItemIdInPos(int pos)
			{
				return userItemIds[pos];
			}
			
			public int GetItemNumInPos(int pos)
			{
				return userItemNums[pos];
			}

			private void SendChangedMessage(int key,int pos)
			{
				if(On_VolumeChanged != null)
				{
					On_VolumeChanged(key,pos);
				}
			}
		}
	}


}

