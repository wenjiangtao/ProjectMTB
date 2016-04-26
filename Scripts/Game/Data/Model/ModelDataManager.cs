using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace MTB
{
	public class ModelDataManager
	{
		private static string ModelPath = "Data/Model/";
		private static ModelDataManager _instance;
		public static ModelDataManager Instance{get{
				if(_instance == null)_instance = new ModelDataManager();
				return _instance;
			}}
		private Dictionary<int,ModelData> _map;
		public ModelDataManager ()
		{
			_map = new Dictionary<int, ModelData>();
		}

		public void Init()
		{
			LoadModelData(100);
		}

		public ModelData GetModelData(int id)
		{
			ModelData data;
			_map.TryGetValue(id,out data);
			if(data == null)
			{
				throw new Exception("不存在id为:" + id + "的模型数据");
			}
			return data;
		}

		private ModelData LoadModelData(int id)
		{
			ModelData data;
			string path =ModelPath + "Item_" + id;
			UnityEngine.Object obj = Resources.Load(path);
			data = new ModelData(obj.ToString());
			_map.Add(id,data);
			return data;
		}

	}
}

