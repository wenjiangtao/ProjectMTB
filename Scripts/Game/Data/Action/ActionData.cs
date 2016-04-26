using System;
using System.Xml;
using System.Collections.Generic;
namespace MTB
{
	public class ActionData
	{
		public int id{get;private set;}
		public string name{get;private set;}

		public string animName{get;private set;}
		public int animLayer{get;private set;}

		public ScriptData action{get;private set;}
		public List<ScriptData> doConditions{get;private set;}
		public List<ScriptData> cancelConditions{get;private set;}

		public ActionData (XmlElement element)
		{
			id = Convert.ToInt32(element.GetAttribute("id"));
			name = element.GetAttribute("name");
			animName =element.GetAttribute("animName");
			animLayer = Convert.ToInt32(element.GetAttribute("animLayer"));
			XmlNodeList nodeList;
			nodeList = element.GetElementsByTagName("ActionScript");
			action = nodeList.Count > 0 ? new ScriptData(nodeList[0] as XmlElement) : new ScriptData("BaseActionScript");

			nodeList = element.GetElementsByTagName("DoConditionScript");
			doConditions = new List<ScriptData>();
			foreach (XmlElement item in nodeList) {
				doConditions.Add(new ScriptData(item));
			}
			if(doConditions.Count <= 0)doConditions.Add(new ScriptData("BaseDoConditionScript"));
//			doConditions = nodeList.Count > 0 ? new ScriptData(nodeList[0] as XmlElement) : new ScriptData("BaseDoConditionScript");

			nodeList = element.GetElementsByTagName("CancelConditionScript");
			cancelConditions = new List<ScriptData>();
			foreach (XmlElement item in nodeList) {
				cancelConditions.Add(new ScriptData(item));
			}
			if(cancelConditions.Count <= 0)cancelConditions.Add(new ScriptData("BaseCancelConditionScript"));
//			cancelConditions = nodeList.Count > 0 ? new ScriptData(nodeList[0] as XmlElement) : new ScriptData("BaseCancelConditionScript");
		}
	}
}

