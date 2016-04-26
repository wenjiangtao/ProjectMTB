using System;
using System.Collections.Generic;
using UnityEngine;
namespace MTB
{
    public class PopulationControlGenerator
    {
        private IMTBRandom _random;
        private List<int> _biomes;
        private int heightCap;
        public PopulationControlGenerator()
        {
            _random = new MTBRandom();
            _biomes = new List<int>(20);
            heightCap = WorldConfig.Instance.heightCap;
        }

        public void Generate(Chunk chunk)
        {
            _biomes.Clear();
            _random.seed = chunk.chunkSeed;
            for (int x = 0; x < Chunk.chunkWidth; x++)
            {
                for (int z = 0; z < Chunk.chunkDepth; z++)
                {
                    int biomeId = chunk.GetBiomeId(x, z, true);
                    if (!_biomes.Contains(biomeId))
                    {
                        _biomes.Add(biomeId);
                    }
                }
            }
            for (int i = 0; i < _biomes.Count; i++)
            {
                BiomeConfig biomeConfig = WorldConfig.Instance.GetBiomeConfigById(_biomes[i]);
                GeneratePopulation(chunk, biomeConfig);
                GenerateEntity(chunk, biomeConfig);
            }
        }

        private void GeneratePopulation(Chunk chunk, BiomeConfig biomeConfig)
        {
            for (int i = 0; i < biomeConfig.populationParams.Count; i++)
            {
                PopulationParam populationParam = biomeConfig.populationParams[i];
                if (_random.Range(0, 100) < populationParam.productRate)
                {
                    IDecoration decoration = DecorationFactory.GetDecoration(populationParam.decorationType);
                  
                    for (int j = 0; j < populationParam.frequency; j++)
                    {
                        int x = _random.Range(0, Chunk.chunkWidth);
                        int z = _random.Range(0, Chunk.chunkDepth);
                        if (chunk.GetBiomeId(x, z, true) != biomeConfig.biomeId) continue;
                        DispatchDecoration(chunk, x, z, populationParam, decoration, biomeConfig.biomeId);
                    }
                }
            }
        }

        private void GenerateEntity(Chunk chunk, BiomeConfig biomeConfig)
        {
            for (int i = 0; i < biomeConfig.entityParams.Count; i++)
            {
                EntityParam entityParam = biomeConfig.entityParams[i];
                if (_random.Range(0, 100) < entityParam.productRate)
                {
                    for (int j = 0; j < entityParam.maxNum; j++)
                    {
                        int x = _random.Range(1, Chunk.chunkWidth-1);
                        int z = _random.Range(1, Chunk.chunkDepth-1);
                        if (chunk.GetBiomeId(x, z, true) != biomeConfig.biomeId) continue;
                        DispatchEntity(chunk, x, z, entityParam);
                    }
                }
            }
        }

        private void DispatchEntity(Chunk chunk, int x, int z, EntityParam entityParam)
        {
            int height = entityParam.maxEntityHeight < heightCap ? entityParam.maxEntityHeight : heightCap;
            for (int y = height - 1; y >= entityParam.minEntityHeight; y--)
            {
                BlockType curType = chunk.GetBlock(x, y, z).BlockType;
                if (curType != BlockType.Air && curType != BlockType.StillWater)
                {
                    List<CheckCondition> conditions = entityParam.checkConditions;
                    bool canDecorate = CheckConditionMeet(chunk, conditions, x, y, z);
                    //bool lightCondiciont = CheckLightCondition(chunk, entityParam.lightCondition, x, y, z);
                    if (canDecorate)
                    {
                        EntityData data = new EntityData();
                        data.id = entityParam.entityId;
                        data.type = EntityType.MONSTER;
                        data.pos = new Vector3(chunk.worldPos.x + x, chunk.worldPos.y + y + 1, chunk.worldPos.z + z);
                        chunk.AddEntityData(data);
                        return;
                    }
                }
            }
        }

        private void DispatchDecoration(Chunk chunk, int x, int z, PopulationParam populationParam, IDecoration decoration, int curBiomeId)
        {
            Decorade(chunk, x, z, populationParam, decoration);
            int size = _random.Range(populationParam.minSize, populationParam.maxSize);
            for (int i = 0; i < size; i++)
            {
                int dis = _random.Range(populationParam.minParentDis, populationParam.maxParentDis);
                int minX = Math.Max(0, x - dis);
                int maxX = Math.Min(Chunk.chunkWidth, x + dis) + 1;
                int minZ = Math.Max(0, z - dis);
                int maxZ = Math.Min(Chunk.chunkDepth, z + dis) + 1;
                int nextX = _random.Range(minX, maxX);
                if (nextX < 0 || nextX > Chunk.chunkWidth - 1) continue;
                int nextZ = _random.Range(minZ, maxZ);
                if (nextZ < 0 || nextZ > Chunk.chunkDepth - 1) continue;
                if (chunk.GetBiomeId(nextX, nextZ) != curBiomeId) continue;
				Decorade(chunk, nextX, nextZ, populationParam, decoration);

            }
        }

        private void Decorade(Chunk chunk, int x, int z, PopulationParam populationParam, IDecoration decoration)
        {
            if (decoration == null || chunk == null)
                return;
            int height = populationParam.maxDecorationHeight < heightCap ? populationParam.maxDecorationHeight : heightCap;
            //找出适合当前装饰品的高度
            for (int y = height - 1; y >= populationParam.minDecorationHeight; y--)
            {
                BlockType curType = chunk.GetBlock(x, y, z).BlockType;
                if (curType != BlockType.Air && curType != BlockType.StillWater)
                {
                    List<CheckCondition> conditions = populationParam.checkConditions;
                    bool canDecorate = CheckConditionMeet(chunk, conditions, x, y, z);
                    if (canDecorate && _random.Range(0, 100) < populationParam.heightGenerateRate)
                    {
						if(decoration.Decorade(chunk, x, y + 1, z, _random))
						{
							return;
						}
                    }
                }
            }
        }

        private bool CheckConditionMeet(Chunk chunk, List<CheckCondition> conditions, int x, int y, int z)
        {
            if (conditions.Count > 0)
            {
                for (int i = 0; i < conditions.Count; i++)
                {
                    Block conditionBlock = chunk.GetBlock(x + conditions[i].offsetX, y + 1 + conditions[i].offsetY, z + conditions[i].offsetZ);
                    for (int j = 0; j < conditions[i].conditionBlocks.Count; j++)
                    {
                        if (!conditionBlock.EqualOther(conditions[i].conditionBlocks[j]))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private bool CheckLightCondition(Chunk chunk, int condition, int x, int y, int z)
        {
            int light = chunk.GetBlockLight(x, y + 1, z);
            if (light >= condition)
                return true;
            return false;
        }
    }

    [Serializable]
    public class PopulationParam
    {
        public DecorationType decorationType;
        //每个聚落最小数量
        public int minSize = 1;
        //每个聚落最大数量
        public int maxSize = 1;
        //块中有多少个聚落
        public int frequency = 1;
        //子装饰品离母装饰品的最小距离
        public int minParentDis = 1;
        //子装饰品离母装饰品的最大距离
        public int maxParentDis = 1;
        [Range(0, 100)]
        public int productRate = 100;

        public int maxDecorationHeight = Chunk.chunkHeight;
        public int minDecorationHeight = 0;

        //表示在同一位置x,z不同高度的时候产生的概率
        [Range(0, 100)]
        public int heightGenerateRate = 100;

        public List<CheckCondition> checkConditions = new List<CheckCondition>();
    }
    [System.Serializable]
    public class EntityParam
    {
        public string name;
        public int entityId;

        public int maxNum;

        [Range(0, 100)]
        public int productRate = 100;

        public int maxEntityHeight = Chunk.chunkHeight;
        public int minEntityHeight = 0;
        public int lightCondition = 4;
        public List<CheckCondition> checkConditions = new List<CheckCondition>();
    }

    [Serializable]
    public class CheckCondition
    {
        public int offsetX;
        public int offsetY;
        public int offsetZ;
        public List<Block> conditionBlocks = new List<Block>();
    }
}

