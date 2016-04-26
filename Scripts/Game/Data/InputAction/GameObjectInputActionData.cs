using System;
using System.Collections.Generic;
using System.Xml;
namespace MTB
{
	public class GameObjectInputActionData
	{
		public int id{get;private set;}
		public List<InputActionData> listActionData{get;private set;}
		public GameObjectInputActionData (XmlElement element)
		{
			id = Convert.ToInt32(element.GetAttribute("id"));
			XmlNodeList nodeList = element.GetElementsByTagName("InputAction");
			listActionData = new List<InputActionData>();
			foreach (XmlElement item in nodeList) {
				var data = new InputActionData(item);
				listActionData.Add(data);
			}
		}
	}
}

