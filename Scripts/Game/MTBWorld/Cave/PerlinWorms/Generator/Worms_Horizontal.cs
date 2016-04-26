using System;
using System.Collections.Generic;
using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;
using UnityEngine;
namespace MTB
{
    public class Worms_Horizontal : AbstractWorms
    {
        #region IWorms implementation

        protected override int getHeightValue(float x, float z)
        {
            float heightOffset = (float)_heightGenerator.GetValue(x, 0, z);
			int heightOff = Mathf.RoundToInt(Chunk.chunkHeight * heightOffset / 2) + 50;
            return heightOff;
        }

        #endregion

        #region implemented abstract members of AbstractWorms

        /***
         *   OctaveCount   八度数
         *   Frequency     频率
         *   Persistence   连续性
         *   Lacunarity    空隙度
         * **/
        protected override void InitWormsGenerator()
        {
            var perlin = new MTBPerlin(_seed);
            perlin.OctaveCount = 40;
            perlin.Frequency = 10;
            perlin.Persistence = 0.2;
            perlin.Lacunarity = 0.4;
            var hightline = new ScaleBias(perlin.ModuleBase);
            hightline.Bias = BlockHeightToFloat(1);
            _heightGenerator = hightline;

            var perlin2 = new MTBPerlin(_seed+1);
            perlin2.OctaveCount = 80;
            perlin2.Frequency = 50;
            perlin2.Persistence = 0.1;
            perlin2.Lacunarity = 0.1;
            var scetion = new ScaleBias(perlin2.ModuleBase);
            scetion.Bias = BlockHeightToFloat(1);
            _sectionGenerator = scetion;

            _lenght = 80;
            _thresholdOffsetFront = 1.0f;
            _limitWidth = 1;
            _limitHeight = 0;
            _radiusHeight = 1;
            _radiusWidth = 2;
            _upMixValue = 1;
            _downMixValue = 2;
            _emptyRateOffset = 0.01f;
        }

        public override CaveType CaveType
        {
            get
            {
                return CaveType.Horizontal;
            }
        }
        #endregion
        public Worms_Horizontal(int seed) : base(seed) { }

    }
}
