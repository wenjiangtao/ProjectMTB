using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
namespace MTB
{
	public class InputScriptData
	{
		public string name{get;private set;}
		public Dictionary<string,string> dataParam;
		public InputScriptData (XmlElement element)
		{
			name = element.GetAttribute("name");
			dataParam = new Dictionary<string, string>();
			IEnumerator enumrator = element.Attributes.GetEnumerator();
			while(enumrator.MoveNext())
			{
				XmlAttribute attr = enumrator.Current as XmlAttribute;
				dataParam.Add(attr.Name,attr.Value);
			}
		}
	}
}

