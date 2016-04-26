using System;
using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;
namespace MTB
{
    //横向山洞
    public class Cave_Horizontal : AbstractCave
    {
        #region ICave implementation

        protected override CaveValue checkValue(CaveValue value, int height)
        {
            value.highHeight = (value.highHeight - value.lowHeight > 7 && value.highHeight - value.lowHeight < 10) ? value.lowHeight + 5 : value.highHeight;
            value.highHeight = (value.highHeight - value.lowHeight > 9) ? value.lowHeight : value.highHeight;
            if (value.highHeight < height && value.highHeight >= value.lowHeight && value.highHeight - value.lowHeight >= 3)
            {
                value.highHeight = value.lowHeight + 5;
            }
            if (value.highHeight > height + 6)
            {
                value.lowHeight -= 30;
                value.highHeight -= 30;
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
            perlin.Frequency = 4;
            perlin.Persistence = 0.5;
            perlin.Lacunarity = 1.0;
            perlin.Seed = _seed + 1;
            var lowwall = new ScaleBias(perlin);
            lowwall.Bias = BlockHeightToFloat(1);
            _lowWallGenerator = lowwall;
            var perlin2 = new Perlin();
            perlin2.OctaveCount = 40;
            perlin2.Frequency = 10;
            perlin2.Persistence = 0.1;
            perlin2.Lacunarity = 2.5;
            perlin2.Seed = _seed + 2;
            var highwall = new ScaleBias(perlin2);
            highwall.Bias = BlockHeightToFloat(1);
            _highWallGenerator = highwall;
        }

        public override CaveType CaveType
        {
            get
            {
                return CaveType.Horizontal;
            }
        }
        #endregion
        public Cave_Horizontal(int seed, float rate) : base(seed, rate) { }

    }
}
