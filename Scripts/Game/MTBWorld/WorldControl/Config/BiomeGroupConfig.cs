using System;
using System.Collections.Generic;
using UnityEngine;
namespace MTB
{
	[Serializable]
	public class BiomeGroupConfig
	{
		public string name;
		public bool isUsing = true;
		//0~255
		public int groupId;
		[Range(0,100)]
		public int groupRarity;
		//0~worldConfig.generateDepth
		public int groupSize;

		public List<int> biomeIds = new List<int>();

		private Dictionary<int,BiomeConfig> _map;

		private int _maxRarity = 0;

		private int _maxDepth = 0;

        //天气配置
        public WeatherSetting WeatherSettingParam = new WeatherSetting();

		public BiomeGroupConfig()
		{
			_map = new Dictionary<int, BiomeConfig>();
		}

		public void Regist(BiomeConfig biomeConfig)
		{
			if(!_map.ContainsKey(biomeConfig.biomeId))
			{
				_map.Add(biomeConfig.biomeId,biomeConfig);
				if(biomeConfig.biomeSize > _maxDepth)_maxDepth = biomeConfig.biomeSize;
				return;
			}
			throw new Exception("已配置id为" + biomeConfig.biomeId + "的生物群落!");
		}

		public BiomeConfig GetBiomeConfig(int id)
		{
			BiomeConfig config;
			_map.TryGetValue(id,out config);
			return config;
		}

		Dictionary<int,BiomeConfig> map = new Dictionary<int, BiomeConfig>();
		public Dictionary<int,BiomeConfig> GetBiomeRarityMap(int depth)
		{
			_maxRarity = 0;
			map.Clear();
			foreach (var item in _map) {
				if(item.Value.biomeSize == depth || depth < 0)
				{
					_maxRarity += item.Value.biomeRarity;
					map.Add(_maxRarity,item.Value);
				}
			}

			if(_maxRarity < map.Count * 100 && _maxDepth != depth)
			{
				_maxRarity = map.Count * 100;
				map.Add(_maxRarity,null);
			}
			return map;
		}

		public bool HasGroupConfigInDepth(int depth)
		{
			foreach (var item in _map) {
				if(item.Value.biomeSize == depth || depth < 0)
				{
					return true;
				}
			}
			return false;
		}

		public int getMaxRarity()
		{
			return _maxRarity;
		}
	}
}

