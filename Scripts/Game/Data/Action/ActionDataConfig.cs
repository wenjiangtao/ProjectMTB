using System;
using System.Collections.Generic;
using System.Xml;
namespace MTB
{
	public class ActionDataConfig
	{

		private Dictionary<int,GameObjectActionData> _map;
		public ActionDataConfig (XmlElement element)
		{
			_map = new Dictionary<int, GameObjectActionData>();
			XmlNodeList list = element.GetElementsByTagName("GameObject");
			for (int i = 0; i < list.Count; i++) {
				GameObjectActionData data = new GameObjectActionData(list[i] as XmlElement);
				_map.Add(data.id,data);
			}
		}

		public GameObjectActionData GetGameObjectActionDataById(int id)
		{
			GameObjectActionData data;
			_map.TryGetValue(id,out data);
			if(data == null) throw new Exception("不存在id为" + id + "的GameObjectActionData");
			return data;
		}
	}
}

