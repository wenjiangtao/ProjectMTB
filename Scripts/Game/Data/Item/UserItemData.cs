using System;
using LitJson;
namespace MTB
{
	public class UserItemData
	{
		public int id{get;private set;}
		public int num{get;private set;}
		public UserItemData (JsonData data)
		{
			this.id = (int)data["id"];
			this.num = (int)data["num"];
		}
	}
}

