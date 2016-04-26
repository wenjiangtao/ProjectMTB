using System;
using System.Collections.Generic;
using UnityEngine;
namespace MTB
{
    [Serializable]
    public class BiomeConfig
    {
        public string name;
        //0~1023
        public int biomeId;
		[NonSerialized]
		public int groupId = int.MinValue;
        //以下是生物群落配置
        //0~WorldConfig.GenerateDepth
        public int biomeSize = 3;
        [Range(1, 100)]
        public int biomeRarity = 80;

        //当作为边界生物群落时的大小
        public int biomeSizeWhenBorder;

        //当前生物群落是那些生物群落的边界
        public List<int> biomeIsBorder = new List<int>();
        //当前生物群落不是那些生物群落的边界
        public List<int> notBorderNear = new List<int>();

        //如果当前生物群落是“岛屿”，即在某些生物群落中央，这个表示岛屿的大小
        public int biomeSizeWhenIsle;
        //“岛屿”生物群落出现的几率（罕见度）
        [Range(1, 100)]
        public int biomeRarityWhenIsle;
        //能够围绕当前生物群落的其他生物群落
        public List<int> IsleInBiome = new List<int>();


        //以下是生物群落地形控制
        [Range(-10, 10)]
        public float biomeHeight = 0f;
        [Range(0, 10f)]
        public float biomeVolatility = 0f;

        //		[Range(-1f,1.5f)]
        public float maxAverageDepth = 0f;
        //		[Range(-1.5f,1f)]
        public float maxAverageHeight = 0f;

        //挥发度1系数
        [Range(0, 1f)]
        public float volatility1 = 0.1f;
        //挥发度2系数
        [Range(0, 1f)]
        public float volatility2 = 0.5f;
        //挥发度1权重
        [Range(0f, 1f)]
        public float volatilityWeight1 = 0.3f;
        //挥发度2权重
        [Range(0f, 1f)]
        public float volatilityWeight2 = 0.6f;

        //过渡半径
        public int smoothRadius = 4;

        public bool disableNotchHeightControl = false;
        //用户自定义高度矩阵
        public float[] heightMatrix = new float[33];

        public bool useWorldWaterLevel = true;
        public int waterLevelMax = 64;
        public int waterLevelMin = 10;
        //地形填充物块
        public Block stoneBlock = new Block(BlockType.Block_1);
        public Block surfaceBlock = new Block(BlockType.Block_1);
        public Block groundBlock = new Block(BlockType.Block_1);
        public Block waterBlock = new Block(BlockType.StillWater);
        //是否移除表面岩石物块
        public bool removeSurfaceStone = false;

        //生物群落装饰品参数
        public List<PopulationParam> populationParams = new List<PopulationParam>();
        //生物群落实体对象参数（怪物什么的）
        public List<EntityParam> entityParams = new List<EntityParam>();
        //		public bool riversEnabled = false;
        //		public int riverId = -1;

        //其他
        public Color color;


        //===============================生物群落中洞穴的配置==============================
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
        //是否使用单独的洞穴配置
        public bool UseBiomeCave = false;
        //是否启用洞穴
        public bool CaveEnable = false;
        //======================================================================================


        public void Init(WorldConfig worldConfig)
        {
            if (useWorldWaterLevel)
            {
                this.waterLevelMax = worldConfig.waterLevelMax;
                this.waterLevelMin = worldConfig.waterLevelMin;
            }
        }
    }
}

