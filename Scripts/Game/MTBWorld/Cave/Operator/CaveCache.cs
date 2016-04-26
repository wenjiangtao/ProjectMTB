using System;
namespace MTB
{
    public class CaveCache : ICave
    {
        #region ICave implementation

        public CaveValue GetValue(float x, float y, float z, int hight)
        {
            if (!(_cached && _x == x && _z == z))
            {
                _value = _cave.GetValue(x, y, z, hight);
                _x = x;
                _z = z;
            }
            _cached = true;
            return _value;
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
        private CaveValue _value;
        private bool _cached;
        private float _x;
        private float _z;


        public CaveCache(ICave cave)
        {
            _cave = cave;
            _cached = false;
        }
    }
}
