using System;
namespace MTB
{
    public class CaveScaleBias : ICave
    {

        #region ICave implementation

        public CaveValue GetValue(float x, float y, float z, int height)
        {
            CaveValue result = _cave.GetValue(x, y, z, height);
            //result.highHeight = result.highHeight * _scale + _bias;
            //result.lowHeight = result.lowHeight * _scale + _bias;

            return result;
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
        private float _scale;
        private float _bias;

        public CaveScaleBias(ICave cave)
        {
            _cave = cave;
        }

        public CaveScaleBias(ICave cave, float scale, float bias)
            : this(cave)
        {
            this._scale = scale;
            this._bias = bias;
        }
    }
}
