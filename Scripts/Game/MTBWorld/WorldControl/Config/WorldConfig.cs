using System;
using System.Collections.Generic;
using UnityEngine;
namespace MTB
{
    public class WorldConfig : Singleton<WorldConfig>
    {
        public int seed = 0;
        public Vector3 birthPlace;
		public float birthRotateY;
        public bool saveBlock = true;
        public bool loadInMap = true;
        public string savedPath { get; private set; }
        public int extendChunkWidth = 10;
        public int playerViewWidth = 6;
        public bool isCreateMode = true;
        public bool hasBorder = false;
        public int borderLeft;
        public int borderRight;
        public int borderBack;
        public int borderFront;

        public int GenerateDepth = 9;
        //0~generateDepth
        public int LandSize = 3;
        [Range(1, 100)]
        public int LandRarity = 70;
        //0~generateDepth - LandSize
        public int LandFuzzy = 2;

        //基础地形高度
        public int baseTerrainHeight = 64;

        //水平分形与垂直分形，决定地形的形状
        public float fractureHorizontal = 0.3f;
        public float fractureVertical = 0.6f;
        //世界水的一般配置
        public int waterLevelMax = 64;
        public int waterLevelMin = 10;
        //最大可填充高度
        public int heightCap = 240;

        //太阳光照强度
        [NonSerialized]
        public int maxLightLevel = 15;
        [Range(0, 1)]
        public float lightEdgeReduce = 0.134217728f;
        //=============洞穴参数，供策划配置参数使用，后面会根据各个生物群落进行相关配置=============

        //单个洞穴群水平延伸范围
        [Range(1, 100)]
        public int CaveHorizontallyExtending = 8;

        //洞穴群密集度
        [Range(1, 100)]
        public int GlobalCaveIntensity = 5;

        //全局洞穴稀有度
        [Range(1, 100)]
        public int GlobalCaveRarity = 99;
        //局部洞穴稀有度
        [Range(1, 100)]
        public int AreaCaveRarity = 3;
        //洞穴生成海拔上限
        [Range(0, 256)]
        public int CaveMaxAltitude = 150;
        //洞穴生成海拔下限
        [Range(0, 256)]
        public int CaveMinAltitude = 0;
        //洞穴顶部剔除
        [Range(0, 256)]
        public int CaveHighCut = 150;
        //洞穴底部剔除
        [Range(0, 256)]
        public int CaveLowCut = 0;

        [Range(0, 100)]
        public int AreaCaveSum = 16;

        [Range(0, 100)]
        public int AreaRandomCaveRate = 10;

        [Range(0, 50)]
        public int AreaRandomCaveMaxSize = 5;

        [Range(0, 50)]
        public int AreaRandomCaveMinSize = 3;

        //是否启用洞穴
        public bool CaveEnable = true;
        //======================================================================================

        //世界所有的生物群落配置
        public List<BiomeConfig> biomeConfigs = new List<BiomeConfig>();
        //世界群组配置
        public List<BiomeGroupConfig> biomeGroupConfigs = new List<BiomeGroupConfig>();

        //存储当前边界生物群落的id
        public List<int> borderBiomeIds = new List<int>();

        //存储当前“岛屿”生物群落周围的生物群落的id
        public List<int> isleBiomeIds = new List<int>();
        //		//0~generateDepth
        //		public int riverRarity = 0;
        //		//riverRarity~generateDepth
        //		public int riverSize = 2;

        private Dictionary<int, BiomeGroupConfig> _biomeGroupConfigMap;

        private Dictionary<int, BiomeConfig> _biomeConfigMap;

        private int _maxRarity = 0;

        private int _maxDepth = 0;

        private int _maxBiomeId = 0;

        private int _maxSmoothRadius = 0;

        void Awake()
        {
            Instance = this;

            _biomeGroupConfigMap = new Dictionary<int, BiomeGroupConfig>();
            _biomeConfigMap = new Dictionary<int, BiomeConfig>();
            if (biomeGroupConfigs == null) return;
            if (biomeConfigs == null) return;
            for (int i = 0; i < biomeGroupConfigs.Count; i++)
            {
                for (int j = 0; j < biomeConfigs.Count; j++)
                {
                    if (biomeGroupConfigs[i].biomeIds.Contains(biomeConfigs[j].biomeId))
                    {
                        biomeGroupConfigs[i].Regist(biomeConfigs[j]);
                        SetBiomeConfigGroupId(biomeConfigs[j], biomeGroupConfigs[i].groupId);
                    }
                }
                if (!_biomeGroupConfigMap.ContainsKey(biomeGroupConfigs[i].groupId))
                {
                    if (biomeGroupConfigs[i].isUsing)
                    {
                        _biomeGroupConfigMap.Add(biomeGroupConfigs[i].groupId, biomeGroupConfigs[i]);
                    }
                }
                else
                {
                    throw new Exception("已存在id为:" + biomeGroupConfigs[i].groupId + "的生物群落组!");
                }
                if (biomeGroupConfigs[i].groupSize > _maxDepth && biomeGroupConfigs[i].isUsing)
                {
                    _maxDepth = biomeGroupConfigs[i].groupSize;
                }
            }

            for (int j = 0; j < biomeConfigs.Count; j++)
            {
                biomeConfigs[j].Init(this);
                if (!_biomeConfigMap.ContainsKey(biomeConfigs[j].biomeId))
                {
                    if (biomeConfigs[j].biomeId > _maxBiomeId) _maxBiomeId = biomeConfigs[j].biomeId;
                    _biomeConfigMap.Add(biomeConfigs[j].biomeId, biomeConfigs[j]);
                }
                else
                {
                    throw new Exception("已存在id为:" + biomeConfigs[j].biomeId + "的生物群落!");
                }
                if (biomeConfigs[j].smoothRadius > _maxSmoothRadius)
                {
                    _maxSmoothRadius = biomeConfigs[j].smoothRadius;
                }
            }

            GetBiomeConfigById(0).groupId = 0;
            for (int j = 0; j < biomeConfigs.Count; j++)
            {
                if (borderBiomeIds.Contains(biomeConfigs[j].biomeId))
                {
                    for (int i = 0; i < biomeConfigs[j].biomeIsBorder.Count; i++)
                    {
                        BiomeConfig parentBorderBiome = GetBiomeConfigById(biomeConfigs[j].biomeIsBorder[i]);
                        SetBiomeConfigGroupIdByParentBiomeConfig(biomeConfigs[j], parentBorderBiome);
                    }
                }
                if (isleBiomeIds.Contains(biomeConfigs[j].biomeId))
                {
                    for (int i = 0; i < biomeConfigs[j].IsleInBiome.Count; i++)
                    {
                        BiomeConfig parentIslandBiome = GetBiomeConfigById(biomeConfigs[j].IsleInBiome[i]);
                        SetBiomeConfigGroupIdByParentBiomeConfig(biomeConfigs[j], parentIslandBiome);
                    }
                }
            }
        }

        public void InitSavedPath(string worldConfigstr, string worldName = "")
        {
            this.name = worldConfigstr;
            savedPath = GameConfig.Instance.WorldSavedPath + "/" + worldConfigstr + "_" + (worldName == "" ? GameConfig.Instance.defaultWorldName : worldName) + "_" + seed;
        }

        private void SetBiomeConfigGroupIdByParentBiomeConfig(BiomeConfig biomeConfig, BiomeConfig parentBiomeConfig)
        {
            if (parentBiomeConfig.groupId == int.MinValue)
            {
                if (borderBiomeIds.Contains(parentBiomeConfig.biomeId))
                {
                    for (int i = 0; i < parentBiomeConfig.biomeIsBorder.Count; i++)
                    {
                        BiomeConfig parentBorderBiome = GetBiomeConfigById(parentBiomeConfig.biomeIsBorder[i]);
                        SetBiomeConfigGroupIdByParentBiomeConfig(parentBiomeConfig, parentBorderBiome);
                    }
                }
                if (isleBiomeIds.Contains(parentBiomeConfig.biomeId))
                {
                    for (int j = 0; j < parentBiomeConfig.IsleInBiome.Count; j++)
                    {
                        BiomeConfig parentIslandBiome = GetBiomeConfigById(parentBiomeConfig.IsleInBiome[j]);
                        SetBiomeConfigGroupIdByParentBiomeConfig(parentBiomeConfig, parentIslandBiome);
                    }
                }
            }
            if (biomeConfig.groupId != int.MinValue && biomeConfig.groupId != parentBiomeConfig.groupId)
            {
                throw new Exception("生物群落id为" + biomeConfig.biomeId + "已存在组id分配,当前所属组id为:" + biomeConfig.groupId + ",想要为其分配组id为：" + parentBiomeConfig.groupId);
            }
            biomeConfig.groupId = parentBiomeConfig.groupId;
        }

        private void SetBiomeConfigGroupId(BiomeConfig biomeConfig, int parentGroupId)
        {
            if (biomeConfig.groupId == int.MinValue)
            {
                biomeConfig.groupId = parentGroupId;
            }
            else
            {
                throw new Exception("生物群落id为" + biomeConfig.biomeId + "已存在组id分配,当前所属组id为:" + biomeConfig.groupId + ",想要为其分配组id为：" + parentGroupId);
            }
        }

        private Dictionary<int, BiomeGroupConfig> map = new Dictionary<int, BiomeGroupConfig>();
        public Dictionary<int, BiomeGroupConfig> GetBiomeGroupRarityMap(int depth)
        {
            _maxRarity = 0;
            map.Clear();
            for (int i = 0; i < biomeGroupConfigs.Count; i++)
            {
                if (biomeGroupConfigs[i].groupSize == depth && biomeGroupConfigs[i].isUsing)
                {
                    _maxRarity += biomeGroupConfigs[i].groupRarity;
                    map.Add(_maxRarity, biomeGroupConfigs[i]);
                }
            }
            //这里不需要保证所有的陆地都被选在完吧！！(没有选到的区域会变成海洋)
            if (_maxRarity < map.Count * 100/* && _maxDepth != depth*/)
            {
                _maxRarity = map.Count * 100;
                map.Add(_maxRarity, null);
            }
            return map;
        }

        public bool HasGroupConfigInDepth(int depth)
        {
            for (int i = 0; i < biomeGroupConfigs.Count; i++)
            {
                if (biomeGroupConfigs[i].groupSize == depth && biomeGroupConfigs[i].isUsing)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasBiomeConfigInDepth(int depth)
        {
            for (int i = 0; i < biomeGroupConfigs.Count; i++)
            {
                if (biomeGroupConfigs[i].HasGroupConfigInDepth(depth) && biomeGroupConfigs[i].isUsing)
                {
                    return true;
                }
            }
            return false;
        }

        public int GetMaxRarity()
        {
            return _maxRarity;
        }

        public int GetMaxBiomeCount()
        {
            return _maxBiomeId + 1;
        }

        public BiomeGroupConfig GetBiomeGroupConfigById(int groupId)
        {
            if (_biomeGroupConfigMap.ContainsKey(groupId))
            {
                return _biomeGroupConfigMap[groupId];
            }
            throw new Exception("不存在id为：" + groupId + "的生物群落组！");
        }

        public BiomeConfig GetBiomeConfigById(int biomeId)
        {
            if (_biomeConfigMap.ContainsKey(biomeId))
            {
                return _biomeConfigMap[biomeId];
            }
            throw new Exception("不存在id为：" + biomeId + "的生物群落！");
        }

        public BiomeConfig GetBiomeConfigOrNullById(int biomeId)
        {
            if (_biomeConfigMap.ContainsKey(biomeId))
            {
                return _biomeConfigMap[biomeId];
            }
            return null;
        }

        public int GetMaxSmoothRadius()
        {
            return _maxSmoothRadius;
        }
    }
}

