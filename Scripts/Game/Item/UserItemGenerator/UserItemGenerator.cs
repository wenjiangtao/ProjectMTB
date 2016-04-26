using System;
namespace MTB
{
	public class UserItemGenerator : IUserItemGenerator
	{
		#region IUserItemGenerator implementation

		public UserItem Generate (params object[] param)
		{
			Item item = (Item)param[0];
			int num = (int)param[1];
			int place = (int)param[2];
			return new UserItem(item,place,num);
		}

		#endregion

		public UserItemGenerator ()
		{
		}
	}
}

