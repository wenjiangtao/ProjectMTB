using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MTB
{
    public class WeatherOperation
    {
        private int _curWeather;

        private int _oldBiomeGroupId = -1;

        private int _curBiomeGroupId;

        private int _curBiomeId;

        private Transform _player;

        private System.Random _random = new System.Random();

        private WeatherSetting[] _weatherSettings;

        private WeatherSetting _defaultWeatherSetting;

        public WeatherOperation(Transform player, WeatherSetting defalutSetting)
        {
            _defaultWeatherSetting = defalutSetting;
            _player = player;
            _weatherSettings = new WeatherSetting[WorldConfig.Instance.biomeGroupConfigs.ToArray().Length + 1];
            _curBiomeId = World.world.GetBiomeId((int)_player.position.x, (int)_player.position.z);
            _curBiomeGroupId = WorldConfig.Instance.GetBiomeConfigOrNullById(_curBiomeId).groupId;
        }

        public void setBiome()
        {
            _curBiomeId = World.world.GetBiomeId((int)_player.position.x, (int)_player.position.z);
            _curBiomeGroupId = WorldConfig.Instance.GetBiomeConfigOrNullById(_curBiomeId).groupId;

            if (_curBiomeGroupId != _oldBiomeGroupId)
            {
                _oldBiomeGroupId = _curBiomeGroupId;
                if (_curBiomeGroupId == 0)
                {
                    _weatherSettings[_curBiomeGroupId] = _defaultWeatherSetting;
                }
                else
                {
                    if (_weatherSettings[_curBiomeGroupId] == null)
                        _weatherSettings[_curBiomeGroupId] = WorldConfig.Instance.GetBiomeGroupConfigById(_curBiomeGroupId).WeatherSettingParam;
                    if (_weatherSettings[_curBiomeGroupId].weatherParams.Length == 0)
                        _weatherSettings[_curBiomeGroupId] = _defaultWeatherSetting;
                }
                _curWeather = _weatherSettings[_curBiomeGroupId].curWeather;
            }
        }

        public void updateEntropy()
        {
            setBiome();
            _weatherSettings[_curBiomeGroupId].weatherParams[_curWeather].entropy += (Time.deltaTime * _weatherSettings[_curBiomeGroupId].weatherParams[_curWeather].decreaseEntropy);
            _weatherSettings[_curBiomeGroupId].weatherParams[_curWeather].entropy = _weatherSettings[_curBiomeGroupId].weatherParams[_curWeather].entropy < 0 ? 0 : _weatherSettings[_curBiomeGroupId].weatherParams[_curWeather].entropy;
            for (int i = 0; i < _weatherSettings[_curBiomeGroupId].weatherParams.Length; i++)
            {
                if (i != _curWeather)
                    _weatherSettings[_curBiomeGroupId].weatherParams[i].entropy += (Time.deltaTime * _weatherSettings[_curBiomeGroupId].weatherParams[i].increaseEntropy);
                _weatherSettings[_curBiomeGroupId].weatherParams[i].entropy = _weatherSettings[_curBiomeGroupId].weatherParams[i].entropy > 10000 ? 10000 : _weatherSettings[_curBiomeGroupId].weatherParams[i].entropy;
            }
            selectCurWeather();
        }

        public int getCurWeather()
        {
            return _curWeather;
        }

        private void selectCurWeather()
        {
            if (_weatherSettings[_curBiomeGroupId].weatherParams[_curWeather].entropy <= 0)
            {
                int nextWeatherIndex = _random.Next(599) / 200;
                for (int i = 0; i < _weatherSettings[_curBiomeGroupId].weatherParams.Length; i++)
                {
                    if (_weatherSettings[_curBiomeGroupId].weatherParams[nextWeatherIndex].entropy >= 10000)
                    {
                        _curWeather = nextWeatherIndex;
                        _weatherSettings[_curBiomeGroupId].curWeather = _curWeather;
                        break;
                    }
                    nextWeatherIndex++;
                    nextWeatherIndex = nextWeatherIndex >= _weatherSettings[_curBiomeGroupId].weatherParams.Length ? 0 : nextWeatherIndex;
                }
            }
        }
    }
}
