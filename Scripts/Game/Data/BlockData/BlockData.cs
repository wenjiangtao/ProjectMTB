using System;
using System.Collections.Generic;
using System.Xml;
namespace MTB
{
	public class BlockData
	{
		public int id{get;private set;}
		public byte type{get;private set;}
		public byte extendId{get;private set;}
		public string name{get;private set;}
		public List<NormalProduction> normalProductions{get;private set;}
		public List<SpecialTypeProduction> specialTypeProductions{get;private set;}
		private Dictionary<int,SpecialTypeProduction> specialTypeProductionMAP;
		public List<SpecialIDProduction> specialIDProductions{get;private set;}
		private Dictionary<int,SpecialIDProduction> specialIDProductionMap;
		public int hardness{get;private set;}
		public int normalMinePower{get;private set;}
		public List<SpecialTypeMinePower> specialTypeMinePowers{get;private set;}
		private Dictionary<int,SpecialTypeMinePower> specialTypeMinePowerMap;
		public List<SpecialIDMinePower> specialIDMinePowers{get;private set;}
		private Dictionary<int,SpecialIDMinePower> specialIDMinePowerMap;
		private static char[] splits = {'[','#',']'};

		public static int GetBlockId(byte type,byte extendId)
		{
			return (type << 4) + extendId;
		}

		public BlockData (XmlElement element)
		{
			string idStr = element.GetElementsByTagName("id")[0].InnerText;
			string[] idStrArr = idStr.Split('-');
			type = Convert.ToByte(idStrArr[0]);
			if(idStrArr.Length > 1)
				extendId = Convert.ToByte(idStrArr[1]);
			else
				extendId = 0;
			id = GetBlockId(type,extendId); 
			name = element.GetElementsByTagName("name")[0].InnerText;

			string normalProductionStr = element.GetElementsByTagName("normalProduction")[0].InnerText;
			string[] normalProductionStrArr = normalProductionStr.Split(splits,StringSplitOptions.RemoveEmptyEntries);
			normalProductions = new List<NormalProduction>();
			for (int i = 0; i < normalProductionStrArr.Length; i++) {
				normalProductions.Add(new NormalProduction(normalProductionStrArr[i]));
			}

			string specialTypeProductionStr = element.GetElementsByTagName("specialTypeProduction")[0].InnerText;
			string[] specialTypeProductionStrArr = specialTypeProductionStr.Split(splits,StringSplitOptions.RemoveEmptyEntries);
			specialTypeProductions = new List<SpecialTypeProduction>();
			specialTypeProductionMAP = new Dictionary<int, SpecialTypeProduction>();
			for (int i = 0; i < specialTypeProductionStrArr.Length; i++) {
				SpecialTypeProduction production = new SpecialTypeProduction(specialTypeProductionStrArr[i]);
				specialTypeProductions.Add(production);
				specialTypeProductionMAP.Add(production.type,production);
			}

			string specialIDProductionStr = element.GetElementsByTagName("specialIDProduction")[0].InnerText;
			string[] specialIDProductionStrArr = specialIDProductionStr.Split(splits,StringSplitOptions.RemoveEmptyEntries);
			specialIDProductions = new List<SpecialIDProduction>();
			specialIDProductionMap = new Dictionary<int, SpecialIDProduction>();
			for (int i = 0; i < specialIDProductionStrArr.Length; i++) {
				SpecialIDProduction production = new SpecialIDProduction(specialIDProductionStrArr[i]);
				specialIDProductions.Add(production);
				specialIDProductionMap.Add(production.usedItemId,production);
			}

			hardness = Convert.ToInt32(element.GetElementsByTagName("hardness")[0].InnerText);
			normalMinePower = Convert.ToInt32(element.GetElementsByTagName("normalMinePower")[0].InnerText);
		
			string specialTypeMinePowerStr = element.GetElementsByTagName("specialTypeMinePower")[0].InnerText;
			string[] specialTypeMinePowerStrArr = specialTypeMinePowerStr.Split(splits,StringSplitOptions.RemoveEmptyEntries);
			specialTypeMinePowers = new List<SpecialTypeMinePower>();
			specialTypeMinePowerMap = new Dictionary<int, SpecialTypeMinePower>();
			for (int i = 0; i < specialTypeMinePowerStrArr.Length; i++) {
				SpecialTypeMinePower minePower = new SpecialTypeMinePower(specialTypeMinePowerStrArr[i]);
				specialTypeMinePowers.Add(minePower);
				specialTypeMinePowerMap.Add(minePower.type,minePower);
			}

			string specialIDMinePowerStr = element.GetElementsByTagName("specialIDMinePower")[0].InnerText;
			string[] specialIDMinePowerStrArr = specialIDMinePowerStr.Split(splits,StringSplitOptions.RemoveEmptyEntries);
			specialIDMinePowers = new List<SpecialIDMinePower>();
			specialIDMinePowerMap = new Dictionary<int, SpecialIDMinePower>();
			for (int i = 0; i < specialIDMinePowerStrArr.Length; i++) {
				SpecialIDMinePower minePower = new SpecialIDMinePower(specialIDMinePowerStrArr[i]);
				specialIDMinePowers.Add(minePower);
				specialIDMinePowerMap.Add(minePower.itemId,minePower);
			}
		}

		public SpecialTypeProduction GetSpecialTypeProduction(int type)
		{
			SpecialTypeProduction production;
			specialTypeProductionMAP.TryGetValue(type,out production);
			return production;
		}

		public SpecialIDProduction GetSpecialIdProduction(int id)
		{
			SpecialIDProduction production;
			specialIDProductionMap.TryGetValue(id,out production);
			return production;
		}

		public SpecialTypeMinePower GetSpecialTypeMinePower(int type)
		{
			SpecialTypeMinePower power;
			specialTypeMinePowerMap.TryGetValue(type,out power);
			return power;
		}

		public SpecialIDMinePower GetSpecialIDMinePower(int id)
		{
			SpecialIDMinePower power;
			specialIDMinePowerMap.TryGetValue(id,out power);
			return power;
		}
	}

	public class NormalProduction
	{
		public int itemId{get;private set;}
		public int num{get;private set;}
		public NormalProduction(string str)
		{
			string[] arr = str.Split(':');
			itemId = Convert.ToInt32(arr[0]);
			num = Convert.ToInt32(arr[1]);
		}
	}

	public class SpecialTypeProduction
	{
		public int type{get;private set;}
		public int itemType{get;private set;}
		public int itemSubType{get;private set;}
		public int itemId{get;private set;}
		public int num{get;private set;}
		public SpecialTypeProduction(string str)
		{
			string[] arr = str.Split(':');
			string typeStr = arr[0];
			string[] typeStrArr = typeStr.Split('_');
			itemType = Convert.ToInt32(typeStrArr[0]);
			if(typeStrArr.Length > 1)
				itemSubType = Convert.ToInt32(typeStrArr[1]);
			else
				itemSubType = 0;
			type = Item.GetItemType(itemType,itemSubType);
			itemId = Convert.ToInt32(arr[1]);
			num = Convert.ToInt32(arr[2]);
		}
	}

	public class SpecialIDProduction
	{
		public int usedItemId{get;private set;}
		public int itemId{get;private set;}
		public int num{get;private set;}
		public SpecialIDProduction(string str)
		{
			string[] arr = str.Split(':');
			usedItemId = Convert.ToInt32(arr[0]);
			itemId = Convert.ToInt32(arr[1]);
			num = Convert.ToInt32(arr[2]);
		}
	}

	public class SpecialTypeMinePower
	{
		public int type{get;private set;}
		public int itemType{get;private set;}
		public int itemSubType{get;private set;}
		public int power{get;private set;}
		public SpecialTypeMinePower(string str)
		{
			string[] arr = str.Split(':');
			string typeStr = arr[0];
			string[] typeStrArr = typeStr.Split('_');
			itemType = Convert.ToInt32(typeStrArr[0]);
			if(typeStrArr.Length > 1)
				itemSubType = Convert.ToInt32(typeStrArr[1]);
			else
				itemSubType = 0;
			type = Item.GetItemType(itemType,itemSubType);
			power = Convert.ToInt32(arr[1]);
		}
	}

	public class SpecialIDMinePower
	{
		public int itemId{get;private set;}
		public int power{get;private set;}
		public SpecialIDMinePower(string str)
		{
			string[] arr = str.Split(':');
			itemId = Convert.ToInt32(arr[0]);
			power = Convert.ToInt32(arr[1]);
		}
	}

}

