
using System;
using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;
namespace MTB
{
    public class CaveTurbulence : ICave
    {
        #region Constants

        private const float X0 = (12414.0f / 65536.0f);
        private const float Y0 = (21850.0f / 65536.0f);
        private const float Z0 = (31337.0f / 65536.0f);
        private const float X1 = (19464.0f / 65536.0f);
        private const float Y1 = (31650.0f / 65536.0f);
        private const float Z1 = (46837.0f / 65536.0f);
        private const float X2 = (26519.0f / 65536.0f);
        private const float Y2 = (43500.0f / 65536.0f);
        private const float Z2 = (60493.0f / 65536.0f);

        #endregion

        #region Fields

        private readonly Perlin _xDistort;
        private readonly Perlin _yDistort;
        private readonly Perlin _zDistort;

        #endregion

        #region ITerrain implementation

        public CaveValue GetValue(float x, float y, float z, int height)
        {
            float xd = x + ((float)_xDistort.GetValue(x + X0, y + Y0, z + Z0) * _power);
            float yd = y + ((float)_yDistort.GetValue(x + X1, y + Y1, z + Z1) * _power);
            float zd = z + ((float)_zDistort.GetValue(x + X2, y + Y2, z + Z2) * _power);
            return _cave.GetValue(xd, yd, zd, height);
        }

        public float RateInMap
        {
            get
            {
                return _cave.RateInMap;
            }
        }

        #endregion

        private ICave _cave;
        private float _power;
        public CaveTurbulence(ICave cave)
            : this(cave, 1.0f)
        {
        }

        public CaveTurbulence(ICave cave, float power)
        {
            _xDistort = new Perlin();
            _zDistort = new Perlin();
            _xDistort.Seed = 0;
            _zDistort.Seed = 1;
            _cave = cave;
            _power = power;
        }

        public CaveTurbulence(Perlin x, Perlin z, ICave cave, float power)
        {
            _power = power;
            _cave = cave;
            _xDistort = x;
            _zDistort = z;
        }

        public int Seed
        {
            get { return _xDistort.Seed; }
            set
            {
                _xDistort.Seed = value;
                _zDistort.Seed = value + 1;
            }
        }

        public float Power
        {
            get { return _power; }
            set { _power = value; }
        }

        public double Frequency
        {
            get { return _xDistort.Frequency; }
            set
            {
                _xDistort.Frequency = value;
                _zDistort.Frequency = value;
            }
        }
    }
}
