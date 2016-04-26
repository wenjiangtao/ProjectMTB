using System;
using System.Collections.Generic;
namespace MTB
{
	//某一个地方的物品管理器
	public class PlaceUserItemManager
	{
		public delegate void OnUserItemAddHandler(UserItem userItem);
		public delegate void OnUserItemRemoveHandler(UserItem userItem);
		public delegate void OnUserItemPropertyChangedHandler(UserItem userItem,string property,object oldValue,object newValue);

		public event OnUserItemAddHandler OnUserItemAdd;
		public event OnUserItemRemoveHandler OnUserItemRemove;
		public event OnUserItemPropertyChangedHandler OnUserItemPropertyChanged;

		public int place{get;private set;}
		private Dictionary<int,UserItem> _userItemMap;

		public PlaceUserItemManager (int place)
		{
			this.place = place;

			_userItemMap = new Dictionary<int, UserItem>();
		}

		public virtual void Init(params object[] param)
		{
		}

		public UserItem GetUserItem(int id)
		{
			UserItem userItem;
			_userItemMap.TryGetValue(id,out userItem);
			return userItem;
		}

		public List<UserItem> GetAllUserItem()
		{
			return new List<UserItem>(_userItemMap.Values);
		}

		public virtual void AddUserItem(UserItem userItem)
		{
			UserItem curUserItem = GetUserItem(userItem.id);
			userItem.place = this.place;
			if(curUserItem != null)
			{
				curUserItem.num += userItem.num;
			}
			else
			{
				userItem.On_PropertyChanged += HandleOn_PropertyChanged;;
				_userItemMap.Add(userItem.id,userItem);
				SendOnAddUserItemMessage(userItem);
			}
		}

		public virtual void RemoveUserItem(int id)
		{
			UserItem curUserItem = GetUserItem(id);
			if(curUserItem != null)
			{
				curUserItem.On_PropertyChanged -= HandleOn_PropertyChanged;
				_userItemMap.Remove(id);
				SendOnRemoveUserItemMessage(curUserItem);
			}
		}

		//使用时保证num少于等于当前物品的总数，否则不会移除任何物品
		public virtual void RemoveUserItem(int id,int num)
		{
			if(num <=0)return;
			UserItem curUserItem = GetUserItem(id);
			if(curUserItem != null)
			{
				if(curUserItem.num > num)
				{
					curUserItem.num -= num;
				}
				else if(curUserItem.num == num)
				{
					RemoveUserItem(id);
				}
			}
		}

		public virtual bool UseUserItem(int id,int num,params object[] param)
		{
			UserItem curUserItem = GetUserItem(id);
			if(curUserItem != null)
			{
				if(curUserItem.Use(num,param) < 0)return false;
				if(curUserItem.num <= 0)RemoveUserItem(id);
				return true;
			}
			return false;
		}

		void HandleOn_PropertyChanged (ChangedItem item, string property, object oldValue, object newValue)
		{
			SendOnPropertyChangedUserItemMessage((UserItem)item,property,oldValue,newValue);
		}

		private void SendOnAddUserItemMessage(UserItem userItem)
		{
			if(OnUserItemAdd != null)
			{
				OnUserItemAdd(userItem);
			}
		}

		private void SendOnRemoveUserItemMessage(UserItem userItem)
		{
			if(OnUserItemRemove != null)
			{
				OnUserItemRemove(userItem);
			}
		}

		private void SendOnPropertyChangedUserItemMessage(UserItem userItem,string property,object oldValue,object newValue)
		{
			if(OnUserItemPropertyChanged != null)
			{
				OnUserItemPropertyChanged(userItem,property,oldValue,newValue);
			}
		}
	}
}

