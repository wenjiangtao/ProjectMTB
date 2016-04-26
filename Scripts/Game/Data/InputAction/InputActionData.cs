using System;
using System.Collections.Generic;
using System.Xml;
namespace MTB
{
	public class InputActionData
	{
		public int actionId{get;private set;}
		public InputType inputType;
		public int inputValue;
		public List<InputScriptData> inputConditions{get;private set;}
		public InputActionData (XmlElement element)
		{
			actionId = Convert.ToInt32(element.GetAttribute("actionId"));
			inputType = (InputType)Convert.ToInt32(element.GetAttribute("inputType"));
			inputValue = Convert.ToInt32(element.GetAttribute("inputValue"));
			XmlNodeList nodeList = element.GetElementsByTagName("InputCondition");
			inputConditions = new List<InputScriptData>();
			foreach (XmlElement item in nodeList) {
				inputConditions.Add(new InputScriptData(item));
			}
		}
	}
}

