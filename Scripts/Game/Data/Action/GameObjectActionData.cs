using System;
using System.Collections.Generic;
using System.Xml;
namespace MTB
{
	public class GameObjectActionData 
	{
		public int id{get;private set;}
		public int defaultId{get;private set;}
		private Dictionary<int,ActionData> _actionMap;
		private List<ActionData> _listActionDatas;

		public GameObjectActionData (XmlElement element)
		{
			id = Convert.ToInt32(element.GetAttribute("id"));
			defaultId = Convert.ToInt32(element.GetAttribute("defaultId"));
			XmlNodeList list = element.ChildNodes;
			_actionMap = new Dictionary<int, ActionData>();
			_listActionDatas = new List<ActionData>();
			for (int i = 0; i < list.Count; i++) {
				ActionData data = new ActionData(list[i] as XmlElement);
				_actionMap.Add(data.id,data);
				_listActionDatas.Add(data);
			}
		}

		public ActionData GetActionData(int id)
		{
			ActionData data;
			_actionMap.TryGetValue(id,out data);
			return data;
		}

		public List<ActionData> GetListActionData()
		{
			return _listActionDatas;
		}
	}
}

