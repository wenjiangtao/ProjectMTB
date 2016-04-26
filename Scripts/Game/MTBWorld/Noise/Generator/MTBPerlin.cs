using System;
using LibNoise;
using LibNoise.Generator;
using UnityEngine;
namespace MTB
{
	public class MTBPerlin : IMTBNoise
	{
		#region INoise implementation

		public float GetValue (float x, float y)
		{
			return GetValue(x,y,0);
		}

		public float GetValue (float x, float y, float z)
		{
			return (float)_perlin.GetValue(x,y,z);
		}

		public ModuleBase ModuleBase {
			get {
				return _perlin;
			}
		}

		#endregion

		private Perlin _perlin;

		public MTBPerlin (int seed)
		{
			_perlin = new Perlin();
			_perlin.Seed = seed;
		}

		/// <summary>
		/// Gets or sets the frequency of the first octave.
		/// </summary>
		public double Frequency
		{
			get { return _perlin.Frequency; }
			set { _perlin.Frequency = value; }
		}
		
		/// <summary>
		/// Gets or sets the lacunarity of the perlin noise.
		/// </summary>
		public double Lacunarity
		{
			get { return _perlin.Lacunarity; }
			set { _perlin.Lacunarity = value; }
		}
		
		/// <summary>
		/// Gets or sets the number of octaves of the perlin noise.
		/// </summary>
		public int OctaveCount
		{
			get { return _perlin.OctaveCount; }
			set { _perlin.OctaveCount = Mathf.Clamp(value, 1, Utils.OctavesMaximum); }
		}
		
		/// <summary>
		/// Gets or sets the persistence of the perlin noise.
		/// </summary>
		public double Persistence
		{
			get { return _perlin.Persistence; }
			set { _perlin.Persistence = value; }
		}
		
	}
}

