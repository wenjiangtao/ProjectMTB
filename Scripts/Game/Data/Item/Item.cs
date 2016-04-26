using System;
using System.Xml;
namespace MTB
{
	public class Item
	{
		public int id{get;private set;}
		public string name{get;private set;}
		public int maxNumPerSlot{get;private set;}
		public int type{get;private set;}
		public int itemType{get;private set;}
		public int itemSubType{get;private set;}
		public byte sceneBlockType{get;private set;}
		public byte sceneBlockExtendId{get;private set;}
		public int attack{get;private set;}
		public int durable{get;private set;}
		public string descript{get;private set;}

		public static int GetItemType(int itemType,int itemSubType)
		{
			return (itemType << 8) + itemSubType;
		}

		public Item (XmlElement element)
		{
			this.id = Convert.ToInt32(element.GetElementsByTagName("id")[0].InnerText);
			this.name = element.GetElementsByTagName("name")[0].InnerText;
			this.maxNumPerSlot = Convert.ToInt32(element.GetElementsByTagName("maxNumPerSlot")[0].InnerText);
			string typeStr = element.GetElementsByTagName("type")[0].InnerText;
			string[] typeStrArr = typeStr.Split('_');
			itemType = Convert.ToInt32(typeStrArr[0]);
			if(typeStrArr.Length > 1)
				itemSubType = Convert.ToInt32(typeStrArr[1]);
			else
				itemSubType = 0;
			type = GetItemType(itemType,itemSubType);
			string sceneBlock = element.GetElementsByTagName("sceneBlock")[0].InnerText;
			string[] sceneBlockArr = sceneBlock.Split('-');
			sceneBlockType = Convert.ToByte(sceneBlockArr[0]);
			if(sceneBlockArr.Length > 1)
				sceneBlockExtendId = Convert.ToByte(sceneBlockArr[1]);
			else
				sceneBlockExtendId = 0;
			attack = Convert.ToInt32(element.GetElementsByTagName("attack")[0].InnerText);
			durable = Convert.ToInt32(element.GetElementsByTagName("durable")[0].InnerText);
			descript = element.GetElementsByTagName("descript")[0].InnerText;
		}
	}
}

