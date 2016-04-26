using System;
using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;
namespace MTB
{
    //空间山洞
    public class Cave_Space : AbstractCave
    {

        #region ICave implementation
        protected override CaveValue checkValue(CaveValue value, int height)
        {
            value.highHeight = (value.highHeight - value.lowHeight > 12 && value.highHeight - value.lowHeight < 15) ? (value.lowHeight + 8) : value.highHeight;
            value.highHeight = (value.highHeight - value.lowHeight > 14) ? (value.lowHeight) : value.highHeight;
            if (value.highHeight - value.lowHeight < 2)
            {
                value.highHeight = value.lowHeight;
            }
            if (value.highHeight - value.lowHeight > 9)
            {
                value.highHeight = value.lowHeight + 9 - (value.highHeight - value.lowHeight - 9);
            }
            if (value.highHeight > height + 1)
            {
                value.lowHeight -= 20;
                value.highHeight -= 20;
                return checkValue(value, height);
            }
            return value;
        }
        #endregion

        #region implemented abstract members of AbstractBiome

        /***
         *   OctaveCount   八度数
         *   Frequency     频率
         *   Persistence   连续性
         *   Lacunarity    空隙度
         * **/
        protected override void InitCaveGenerator()
        {
            var perlin = new Perlin();
            perlin.OctaveCount = 10;
            perlin.Frequency = 5;
            perlin.Persistence = 0.1;
            perlin.Lacunarity = 1.5;
            perlin.Seed = _seed;

            var lowwall = new ScaleBias(perlin);
            lowwall.Bias = BlockHeightToFloat(1);
            _lowWallGenerator = lowwall;
            var perlin2 = new Perlin();
            perlin2.OctaveCount = 20;
            perlin2.Frequency = 10;
            perlin2.Persistence = 0.1;
            perlin2.Lacunarity = 1.0;
            perlin2.Seed = _seed + 1;

            var highwall = new ScaleBias(perlin2);
            highwall.Bias = BlockHeightToFloat(1);
            _highWallGenerator = highwall;
        }

        public override CaveType CaveType
        {
            get
            {
                return CaveType.Space;
            }
        }
        #endregion
        public Cave_Space(int seed, float rate) : base(seed, rate) { }

    }
}
