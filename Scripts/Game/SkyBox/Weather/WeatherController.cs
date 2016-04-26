/***
 * 当前只做一套天气熵值变换
 * **/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace MTB
{
    public class WeatherController : Singleton<WeatherController>
    {
        public WeatherSetting _weatherSetting;
        private WeatherOperation _weatherOperation;
        private ThunderController _tunderController;
        private IWeatherController[] _weatherPool;
        private Transform _player;
        private int _curWeather;
        private int _translateFrame;
        private bool _translateState;
        private bool _onWork = false;

        void Awake()
        {
            WeatherController.Instance = this;
        }

        void Update()
        {
            if (!_onWork)
                return;
            if (_translateState)
            {
                translateSky();
                return;
            }
            _weatherOperation.updateEntropy();
            if (_curWeather != _weatherOperation.getCurWeather() && !_translateState)
                changeWeather();
            if (_curWeather == WeatherType.Rain || _curWeather == WeatherType.Snow)
                _weatherPool[_curWeather].updateView();
            if (CameraManager.Init)
            {
                gameObject.transform.position = new Vector3(CameraManager.Instance.CurCamera.FollowCamera.transform.position.x, CameraManager.Instance.CurCamera.FollowCamera.transform.position.y + 10, CameraManager.Instance.CurCamera.FollowCamera.transform.position.z);
            }
        }

        public void Init()
        {
            _onWork = true;
            _player = HasActionObjectManager.Instance.playerManager.getMyPlayer().transform;
            _weatherOperation = new WeatherOperation(_player, _weatherSetting);
            loadWeatherResources();
            changeWeather();

            _translateFrame = 0;
            _translateState = false;
        }

        private void loadWeatherResources()
        {
            _weatherPool = new IWeatherController[3];
            _weatherPool[WeatherType.Rain] = new RainController(GameObject.Instantiate(Resources.Load("Prefabs/Weather/Rain_Prefab") as GameObject));
            _weatherPool[WeatherType.Snow] = new SnowController(GameObject.Instantiate(Resources.Load("Prefabs/Weather/Snow_Prefab") as GameObject));
            _weatherPool[WeatherType.Rain].setParent(gameObject.transform);
            _weatherPool[WeatherType.Snow].setParent(gameObject.transform);
            _weatherPool[WeatherType.Rain].setPosition(gameObject.transform.position);
            _weatherPool[WeatherType.Snow].setPosition(gameObject.transform.position);
        }

        private void changeWeather()
        {
            _curWeather = _weatherOperation.getCurWeather();
            if (_curWeather != WeatherType.Sunny)
                _weatherPool[_curWeather].setEnable(true);
            if (_curWeather == WeatherType.Thunder)
                _tunderController.reset();
            for (int i = 0; i < _weatherPool.Length; i++)
            {
                if (_weatherPool[i] != null && i != _curWeather)
                    _weatherPool[i].setEnable(false);
            }
            _translateState = true;
        }

        private void translateSky()
        {
            if (_curWeather != WeatherType.Sunny && MTBSkyBox.Instance.skyBoxParams.cloudParam.cloudSharpness > 0)
            {
                MTBSkyBox.Instance.skyBoxParams.cloudParam.cloudSharpness -= 0.004f;
            }
            else if (MTBSkyBox.Instance.skyBoxParams.cloudParam.cloudSharpness < 0.8f)
            {
                MTBSkyBox.Instance.skyBoxParams.cloudParam.cloudSharpness += 0.004f;
            }

            if (_translateFrame > 250)
            {
                _translateFrame = 0;
                _translateState = false;
            }
            _translateFrame++;
        }
    }
}
