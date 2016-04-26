using System;
using UnityEngine;

namespace MTB
{
    public abstract class CaveGenBase
    {
        protected int _seed;
        protected System.Random _random;
        private int _worldInt1;
        private int _worldInt2;

        //=============每次产生当前chunk的洞穴时，先把当前chunk所属生物群落的洞穴配置数据读出来============
        //单个洞穴群水平延伸范围
        protected int CaveHorizontallyExtending = 8;
        //洞穴群密集度
        protected int GlobalCaveIntensity = 5;
        //全局洞穴稀有度
        protected int GlobalCaveRarity = 99;
        //局部洞穴稀有度
        protected int AreaCaveRarity = 3;
        //洞穴生成海拔上限
        protected int CaveMaxAltitude = 150;
        //洞穴生成海拔下限
        protected int CaveMinAltitude = 0;
        //洞穴顶部剔除
        protected int CaveHighCut = 150;
        //洞穴底部剔除
        protected int CaveLowCut = 0;

        protected int AreaCaveSum = 16;

        protected int AreaRandomCaveRate = 10;

        protected int AreaRandomCaveMaxSize = 5;

        protected int AreaRandomCaveMinSize = 3;
        //是否启用洞穴
        protected bool CaveEnable = true;



        public CaveGenBase(int seed)
        {
            _seed = seed;
            this._random = new System.Random(_seed);
            _worldInt1 = this._random.Next(100);
            _worldInt2 = this._random.Next(100);

        }

        public void generate(Chunk chunk)
        {
            GetCurBiomeCaveConfig(chunk);
            if (!CaveEnable)
                return;
            int i = CaveHorizontallyExtending;
            int chunkx = chunk.worldPos.x / Chunk.chunkWidth;
            int chunkz = chunk.worldPos.z / Chunk.chunkDepth;

            for (int x = chunkx - i; x <= chunkx + i; x++)
            {
                for (int z = chunkz - i; z <= chunkz + i; z++)
                {
                    int d3 = x * _worldInt1 + _worldInt2;
                    int d4 = z * _worldInt2 + _worldInt1;
                    this._random = new System.Random(d3 * d4 * _seed);
                    generateChunk(new Vector3(x, 0, z), chunk);
                }
            }
        }

        private void GetCurBiomeCaveConfig(Chunk chunk)
        {
            int biomeId = chunk.world.GetBiomeId(chunk.worldPos.x, chunk.worldPos.z);
            biomeId = biomeId == -1 ? 0 : biomeId;
            bool isUseBiomeConfig = WorldConfig.Instance.biomeConfigs[biomeId].UseBiomeCave;

            CaveHorizontallyExtending = isUseBiomeConfig ? WorldConfig.Instance.biomeConfigs[biomeId].CaveHorizontallyExtending : WorldConfig.Instance.CaveHorizontallyExtending;
            GlobalCaveIntensity = isUseBiomeConfig ? WorldConfig.Instance.biomeConfigs[biomeId].GlobalCaveIntensity : WorldConfig.Instance.GlobalCaveIntensity;
            GlobalCaveRarity = isUseBiomeConfig ? WorldConfig.Instance.biomeConfigs[biomeId].GlobalCaveRarity : WorldConfig.Instance.GlobalCaveRarity;
            AreaCaveRarity = isUseBiomeConfig ? WorldConfig.Instance.biomeConfigs[biomeId].AreaCaveRarity : WorldConfig.Instance.AreaCaveRarity;
            CaveMaxAltitude = isUseBiomeConfig ? WorldConfig.Instance.biomeConfigs[biomeId].CaveMaxAltitude : WorldConfig.Instance.CaveMaxAltitude;
            CaveMinAltitude = isUseBiomeConfig ? WorldConfig.Instance.biomeConfigs[biomeId].CaveMinAltitude : WorldConfig.Instance.CaveMinAltitude;
            CaveHighCut = isUseBiomeConfig ? WorldConfig.Instance.biomeConfigs[biomeId].CaveHighCut : WorldConfig.Instance.CaveHighCut;
            CaveLowCut = isUseBiomeConfig ? WorldConfig.Instance.biomeConfigs[biomeId].CaveLowCut : WorldConfig.Instance.CaveLowCut;
            AreaCaveSum = isUseBiomeConfig ? WorldConfig.Instance.biomeConfigs[biomeId].AreaCaveSum : WorldConfig.Instance.AreaCaveSum;
            AreaRandomCaveRate = isUseBiomeConfig ? WorldConfig.Instance.biomeConfigs[biomeId].AreaRandomCaveRate : WorldConfig.Instance.AreaRandomCaveRate;
            AreaRandomCaveMaxSize = isUseBiomeConfig ? WorldConfig.Instance.biomeConfigs[biomeId].AreaRandomCaveMaxSize : WorldConfig.Instance.AreaRandomCaveMaxSize;
            AreaRandomCaveMinSize = isUseBiomeConfig ? WorldConfig.Instance.biomeConfigs[biomeId].AreaRandomCaveMinSize : WorldConfig.Instance.AreaRandomCaveMinSize;
            CaveEnable = isUseBiomeConfig ? WorldConfig.Instance.biomeConfigs[biomeId].CaveEnable : WorldConfig.Instance.CaveEnable;

            if (
                chunk.haveWater ||
                chunk.world.GetChunk(chunk.worldPos.x + Chunk.chunkWidth, chunk.worldPos.y, chunk.worldPos.z) != null && chunk.world.GetChunk(chunk.worldPos.x + Chunk.chunkWidth, chunk.worldPos.y, chunk.worldPos.z).haveWater ||
                chunk.world.GetChunk(chunk.worldPos.x - Chunk.chunkWidth, chunk.worldPos.y, chunk.worldPos.z) != null && chunk.world.GetChunk(chunk.worldPos.x - Chunk.chunkWidth, chunk.worldPos.y, chunk.worldPos.z).haveWater ||
                chunk.world.GetChunk(chunk.worldPos.x, chunk.worldPos.y, chunk.worldPos.z + Chunk.chunkDepth) != null && chunk.world.GetChunk(chunk.worldPos.x, chunk.worldPos.y, chunk.worldPos.z + Chunk.chunkDepth).haveWater ||
                chunk.world.GetChunk(chunk.worldPos.x, chunk.worldPos.y, chunk.worldPos.z - Chunk.chunkDepth) != null && chunk.world.GetChunk(chunk.worldPos.x, chunk.worldPos.y, chunk.worldPos.z - Chunk.chunkDepth).haveWater
                )
            {
                CaveHighCut = WorldConfig.Instance.biomeConfigs[biomeId].waterLevelMin - 2;
            }
        }

        /**
        * Generates the structure for the given chunk. The terrain generator
        * calls this method for all chunks not more than {@link #checkAreaSize}
        * chunks away on either axis from the generatingChunk.
        *
        * @param currentChunk          The chunk we're searching.
        * @param generatingChunkBuffer The chunk that is currently being
        *                              generated.
        */
        protected abstract void generateChunk(Vector3 chunkCoord, Chunk chunk);
    }
}
