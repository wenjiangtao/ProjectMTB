using System;
using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;
using UnityEngine;
namespace MTB
{
    public class PerlinCaveController : ICaveController
    {
        #region ICaveController implementation
        public float GetValue(float x, float z)
        {
            float result = ((float)_controller.GetValue(x, 0, z) + 1) / 2;
            return result;
        }

        public float Frequency
        {
            get
            {
                return _frequency;
            }
            set
            {
                _frequency = value;
            }
        }

        #endregion

        private ModuleBase _controller;
        private int _seed;
        private float _frequency;

        public PerlinCaveController() : this(0, 1.0f) { }

        public PerlinCaveController(int seed, float frequency) {
            _seed = seed;
            _frequency = frequency;
            var perlin = new Perlin();
            perlin.OctaveCount = 1;
            perlin.Quality = QualityMode.Low;
            perlin.Seed = seed;
            perlin.Frequency = frequency;

            var result = new Turbulence(perlin);
            result.Seed = seed;
            result.Power = 1f;
            _controller = result;	
        }
    }
}
