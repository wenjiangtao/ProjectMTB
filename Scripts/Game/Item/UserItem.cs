using System;
namespace MTB
{
	//用户物品
	public class UserItem : ChangedItem
	{
		public const string PLACE = "place";
		public const string NUM = "num";

		public UserItem (Item item,int place,int num)
		{
			this.item = item;
			SetProperty(PLACE,place);
			SetProperty(NUM,num);
		}

		public Item item{get;private set;}

		public int place{
			get{return (int)GetProperty(PLACE);}
			set{SetProperty(PLACE,value);}
		}

		public int num{
			get{return (int)GetProperty(NUM);}
			set{SetProperty(NUM,value);}
		}

		public virtual int Use(int num,object[] param = null)
		{
			if(this.num >= num)
			{
				this.num -= num;
				return num;
			}
			return -1;
		}

//		public int TryAddNum(int num)
//		{
//			int result = this.num + num;
//			int oldNum = this.item;
//			if(result > item.maxNumPerSlot)
//			{
//				result = item.maxNumPerSlot;
//			}
//			if(result > oldNum)
//				SetProperty(NUM,result);
//			return result - oldNum;
//		}

		public virtual UserItem ToClone()
		{
			return new UserItem(item,place,num);
		}

		public int id{get{return item.id;}}
	}

}

