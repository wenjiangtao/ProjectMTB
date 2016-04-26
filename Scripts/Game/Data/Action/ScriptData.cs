using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
namespace MTB
{
	public class ScriptData
	{
		public string name{get;private set;}
		public string netName{get;private set;}
		public Dictionary<string,string> dataParam;
		public ScriptData (XmlElement element)
		{
			name = element.GetAttribute("name");
			if(element.HasAttribute("netName"))
			{
				netName = element.GetAttribute("netName");
			}
			else
			{
				netName = "BaseActionScript";
			}
			dataParam = new Dictionary<string, string>();
			IEnumerator enumrator = element.Attributes.GetEnumerator();
			while(enumrator.MoveNext())
			{
				XmlAttribute attr = enumrator.Current as XmlAttribute;
				dataParam.Add(attr.Name,attr.Value);
			}
		}

		public ScriptData(string name)
		{
			this.name = name;
			netName = "BaseActionScript";
			dataParam = new Dictionary<string, string>();
		}
	}
}

