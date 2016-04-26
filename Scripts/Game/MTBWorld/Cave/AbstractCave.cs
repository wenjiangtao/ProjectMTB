using System;
using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;
using UnityEngine;
namespace MTB
{
    public abstract class AbstractCave : ICave
    {
        #region ICave implementation
        public float RateInMap
        {
            get
            {
                return _rateInMap;
            }
        }

        public virtual CaveValue GetValue(float x, float y, float z, int heightLineHeight)
        {
            float lowWallHeightOffset = (float)_lowWallGenerator.GetValue(x, 0, z);
			int lowWallOffset = Mathf.RoundToInt(Chunk.chunkHeight * lowWallHeightOffset / 2);
            float highwallHeightOffset = (float)_highWallGenerator.GetValue(x, 0, z);
			int highwallOffset = Mathf.RoundToInt(Chunk.chunkHeight * highwallHeightOffset / 2);
            _caveValue.highHeight = Mathf.Clamp(highwallOffset, World.MinHeight, World.MaxHeight) / 2;
            _caveValue.lowHeight = Mathf.Clamp(lowWallOffset, World.MinHeight, World.MaxHeight) / 2;
            _caveValue = checkValue(_caveValue, heightLineHeight);
            return _caveValue;
        }

        protected virtual CaveValue checkValue(CaveValue value, int height)
        {
            return value;
        }
        #endregion

        protected ModuleBase _lowWallGenerator;
        protected ModuleBase _highWallGenerator;
        protected int _seed;
        private float _rateInMap;
        private float _scale;
        protected CaveValue _caveValue;

        public AbstractCave(int seed, float rateInMap)
        {
            _seed = seed;
            _rateInMap = rateInMap;
            InitCaveGenerator();
            _scale = 1.0f;
            _caveValue = new CaveValue();
            _caveValue.type = CaveType;
        }

        public static float BlockHeightToFloat(int height)
        {
            float result = 2.0f * height / Chunk.chunkHeight;
            return result;
        }

        public static int FloatToBlockHeight(float height)
        {
            return Mathf.RoundToInt(Chunk.chunkHeight * height / 2);
        }

        protected abstract void InitCaveGenerator();

        public abstract CaveType CaveType { get; }

    }
}
