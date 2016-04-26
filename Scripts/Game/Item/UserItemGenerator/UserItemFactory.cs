using System;
using System.Collections.Generic;
namespace MTB
{
	public class UserItemFactory
	{
		public UserItemFactory ()
		{
		}

		private static Dictionary<int,IUserItemGenerator> _map = InitMap();
		private static IUserItemGenerator _defaultGenerator;
		
		private static Dictionary<int,IUserItemGenerator> InitMap()
		{
			Dictionary<int,IUserItemGenerator> map = new Dictionary<int, IUserItemGenerator>();
			_defaultGenerator = new UserItemGenerator();
			return map;
		}
		
		public static UserItem GenerateUserItem(int id,int num,int place,params object[] param)
		{
			Item item = ItemManager.Instance.GetItem(id);
			IUserItemGenerator generator;
			_map.TryGetValue(item.itemType,out generator);
			if(generator != null)
			{
				return generator.Generate(item,num,place,param);
			}
			return _defaultGenerator.Generate(item,num,place,param);
		}
	}
}

