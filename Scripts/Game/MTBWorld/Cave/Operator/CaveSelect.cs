using System;
namespace MTB
{
    public class CaveSelect : ICave
    {
        #region ICave implementation

        public float RateInMap
        {
            get
            {
                return _totalRate;
            }
        }

        #endregion

        private ICave[] _caves;
        private ICaveController _controller;
        private float _totalRate;
        private float[] _fallOff;
        private CaveValue _caveValue = new CaveValue();

        public CaveSelect(ICave[] caves, float fallOff, ICaveController controller)
        {
            _caves = caves;
            _controller = controller;
            _totalRate = GetTotalRate();
            if (fallOff > 0.5f) fallOff = 0.5f;
            else if (fallOff < 0f) fallOff = 0;
            _fallOff = new float[caves.Length];
            for (int i = 0; i < _fallOff.Length; i++)
            {
                _fallOff[i] = fallOff;
            }
        }

        public CaveSelect(ICave[] caves, float[] fallOff, ICaveController controller)
        {
            _caves = caves;
            _controller = controller;
            _totalRate = GetTotalRate();
            _fallOff = new float[fallOff.Length];
            for (int i = 0; i < fallOff.Length; i++)
            {
                if (fallOff[i] > 0.5f) _fallOff[i] = 0.5f;
                else if (fallOff[i] < 0f) _fallOff[i] = 0f;
                else _fallOff[i] = fallOff[i];
            }
        }

        public CaveSelect(ICave[] caves, ICaveController controller)
            : this(caves, 0f, controller)
        {
        }

        #region ICaveGenerator implementation
        public CaveValue GetValue(float x, float y, float z, int height)
        {
            float cv = _controller.GetValue(x, z);
            if (cv > 1) cv = 1;
            if (cv < 0) cv = 0;
            float curTotalRate = 0;
            for (int i = 0; i < _caves.Length; i++)
            {
                float curTerrainRate = _caves[i].RateInMap / _totalRate;
                float preRate = curTotalRate;
                //解决精度问题
                if (_caves.Length - 1 == i) curTotalRate = 1f;
                else curTotalRate += curTerrainRate;
                if (cv <= curTotalRate)
                {
                    CaveValue curTerrainValue = _caves[i].GetValue(x, y, z, height);
                    if (_fallOff[i] > 0)
                    {
                        float a;
                        CaveValue tempA;
                        CaveValue tempB;
                        float curFallout = curTerrainRate * _fallOff[i];

                        if (cv < preRate + curFallout && i - 1 >= 0)
                        {
                            int j = i - 1;
                            tempA = curTerrainValue;
                            tempB = _caves[j].GetValue(x, y, z, height);
                            var lc = preRate - _caves[j].RateInMap / _totalRate * _fallOff[j];
                            var uc = preRate + curFallout;
                            a = MapCubicSCurve((cv - lc) / (uc - lc));
                            _caveValue.type = tempA.type;
                            _caveValue = InterpolateLinear(tempB, tempA, a);
                            return _caveValue;
                        }
                        if (cv < curTotalRate - curFallout)
                        {
                            return _caves[i].GetValue(x, y, z, height);
                        }
                        if (cv < curTotalRate && i < _caves.Length - 1)
                        {
                            int j = i + 1;
                            tempA = curTerrainValue;
                            tempB = _caves[j].GetValue(x, y, z, height);
                            var lc = curTotalRate - curFallout;
                            var uc = curTotalRate + _caves[j].RateInMap / _totalRate * _fallOff[j];
                            a = MapCubicSCurve((cv - lc) / (uc - lc));
                            _caveValue.type = tempA.type;
                            _caveValue = InterpolateLinear(tempA, tempB, a);
                            return _caveValue;
                        }
                        return _caves[i].GetValue(x, y, z, height);
                    }
                    else
                    {
                        return curTerrainValue;
                    }
                }
            }
            throw new Exception("传入的概率不正确!");
        }

        private float MapCubicSCurve(float value)
        {
            return (value * value * (3.0f - 2.0f * value));
        }

        private CaveValue InterpolateLinear(CaveValue a, CaveValue b, float position)
        {
            a.highHeight = Convert.ToInt32((1.0f - position) * a.highHeight + (position * b.highHeight));
            a.lowHeight = Convert.ToInt32((1.0f - position) * a.lowHeight + (position * b.lowHeight));
            return a;
        }

        public float GetRate()
        {
            return _totalRate;
        }

        private float GetTotalRate()
        {
            float totalRate = 0;
            for (int i = 0; i < _caves.Length; i++)
            {
                totalRate += _caves[i].RateInMap;
            }
            return totalRate;
        }

        #endregion

    }
}
