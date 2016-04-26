using System;
using System.Xml;
using UnityEngine;
using System.Collections.Generic;
namespace MTB
{
	public class InputActionDataManager
	{
		private static string PlayerActionPath = "Data/Action/Player_Input_Action_Config";
		private static InputActionDataManager _instance;
		public static InputActionDataManager Instance{get{
				if(_instance == null)_instance = new InputActionDataManager();
				return _instance;
			}}
		private Dictionary<int,GameObjectInputActionData> _map;
		public InputActionDataManager ()
		{

		}

		public void Init()
		{
			XmlDocument playerXml = new XmlDocument();
			playerXml.LoadXml(Resources.Load(PlayerActionPath).ToString());
			_map = new Dictionary<int, GameObjectInputActionData>();
			XmlNodeList list = playerXml.DocumentElement.GetElementsByTagName("GameObject");
			for (int i = 0; i < list.Count; i++) {
				GameObjectInputActionData data = new GameObjectInputActionData(list[i] as XmlElement);
				_map.Add(data.id,data);
			}
		}

		public GameObjectInputActionData GetData(int id)
		{
			GameObjectInputActionData data;
			_map.TryGetValue(id,out data);
			if(data == null) throw new Exception("不存在id为" + id + "的GameObjectInputActionData");
			return data;
		}

	}
}

