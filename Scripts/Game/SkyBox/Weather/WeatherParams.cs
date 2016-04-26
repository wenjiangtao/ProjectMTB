using UnityEngine;
namespace MTB
{
    [System.Serializable]
    public class WeatherSetting
    {
        public int startWeather;
        public int curWeather;
        public WeatherParams[] weatherParams;
    }
    [System.Serializable]
    public class WeatherParams
    {
        public string name;
        public float entropy;
        public int increaseEntropy;
        public int decreaseEntropy;
    }

}
