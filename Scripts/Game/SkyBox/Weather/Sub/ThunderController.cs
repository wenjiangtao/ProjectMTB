using UnityEngine;
using System.Collections;
namespace MTB
{
    public class ThunderController : IWeatherController
    {
        private int HIDETIME = 200;
        private int SHOWTIME = 500;
        private int _pathLength = 5;
        private int _hidetime;
        private int _showTime;
        private int _randomx;
        private int _randomz;
        private int _startIndex;
        private int _state;
        private System.Random _random;
        private GameObject _thunder;
        private LineRenderer _lineRenderer;
        private Vector3[] _pathList = new Vector3[]{
         new Vector3(0.5f,2.5f,1.5f),new Vector3(0.2f,2f,1.5f),new Vector3(0,1.5f,1.8f),new Vector3(-0.5f,1f,1.8f),new Vector3(0,0.5f,1.5f),
         new Vector3(-0.5f,2.5f,-1.5f),new Vector3(0.4f,2f,-1.5f),new Vector3(-0.5f,1.5f,1.8f),new Vector3(0.5f,1f,1.8f),new Vector3(0,0,1.5f)
    
        };

        public ThunderController(GameObject thunder)
        {
            _random = new System.Random();
            _thunder = thunder;
            _lineRenderer = _thunder.GetComponent<LineRenderer>();
            _state = 1;
            _hidetime = HIDETIME;
            _showTime = SHOWTIME;
            setEnable(false);
        }

        public void reset()
        {
            _state = 1;
            _hidetime = HIDETIME;
            _showTime = SHOWTIME;
        }

        public void updateView()
        {
            if (_state == 1)
            {
                _hidetime--;
                if (_hidetime <= 0)
                {
                    _hidetime = HIDETIME;
                    _state = 2;
                    showLightning();
                }
            }
            if (_state == 2)
            {
                _showTime--;
                if (_showTime <= 0)
                {
                    _showTime = SHOWTIME;
                    _state = 1;
                    hideLightning();
                }
            }
        }

        private void showLightning()
        {
            _randomx = _random.Next(10) - 5;
            _randomz = _random.Next(10) - 5;
            _startIndex = _random.Next(100) > 50 ? 5 : 0;
            //_thunder.transform.position = HasActionObjectManager.Instance.playerManager.getMyPlayer().transform.position;
            for (int i = 0; i < 5; i++)
            {
                _lineRenderer.SetPosition(i, new Vector3(_pathList[_startIndex + i].x + _randomx, _pathList[_startIndex + i].y, _pathList[_startIndex + i].z + _randomz));
            }
        }

        private void hideLightning()
        {
            for (int i = 0; i < 5; i++)
            {
                _lineRenderer.SetPosition(i, new Vector3(0, 0, 0));
            }
        }


        public void setEnable(bool b)
        {
            _thunder.SetActive(b);
        }

        public void setParent(Transform p)
        {
            _thunder.transform.parent = p;
        }
        public void setPosition(Vector3 p)
        {
            _thunder.transform.position = p;
        }
        public void dispose() { }
    }
}
