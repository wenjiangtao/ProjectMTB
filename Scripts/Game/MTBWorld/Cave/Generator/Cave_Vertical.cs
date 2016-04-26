using System;
using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;
namespace MTB
{
    //纵向山洞
    public class Cave_Vertical : AbstractCave
    {
        #region ICave implementation

        protected override CaveValue checkValue(CaveValue value, int height)
        {
            value.highHeight = (value.highHeight - value.lowHeight > 10 && value.highHeight - value.lowHeight < 15) ? value.lowHeight + 5 : value.highHeight;
            value.highHeight = (value.highHeight - value.lowHeight > 14) ? value.lowHeight : value.highHeight;
            if (value.highHeight < height && value.highHeight >= value.lowHeight && value.highHeight - value.lowHeight <= 6 && value.highHeight - value.lowHeight >= 3)
            {
                value.highHeight = value.lowHeight + 8;
            }
            if (value.highHeight - value.lowHeight > 7)
            {
                value.highHeight = value.lowHeight + 7 - (value.highHeight - value.lowHeight - 7);
            }
            if (value.highHeight < height - 30)
            {
                value.lowHeight += 20;
                value.highHeight += 20;
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
            perlin.OctaveCount = 40;
            perlin.Frequency = 20;
            perlin.Persistence = 0.1;
            perlin.Lacunarity = 4.5;
            perlin.Seed = _seed + 1;
            var lowwall = new ScaleBias(perlin);
            lowwall.Bias = BlockHeightToFloat(1);
            _lowWallGenerator = lowwall;
            var perlin2 = new Perlin();
            perlin2.OctaveCount = 80;
            perlin2.Frequency = 40;
            perlin2.Persistence = 0.1;
            perlin2.Lacunarity = 1.5;
            perlin2.Seed = _seed + 2;
            var highwall = new ScaleBias(perlin2);
            highwall.Bias = BlockHeightToFloat(1);
            _highWallGenerator = highwall;
        }

        public override CaveType CaveType
        {
            get
            {
                return CaveType.Vertical;
            }
        }
        #endregion

        public Cave_Vertical(int seed, float rate) : base(seed, rate) { }
    }
}
