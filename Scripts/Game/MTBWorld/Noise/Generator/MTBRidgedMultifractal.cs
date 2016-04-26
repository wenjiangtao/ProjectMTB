using System;
using LibNoise;
using LibNoise.Generator;
using UnityEngine;
namespace MTB
{
	public class MTBRidgedMultifractal : IMTBNoise
	{
		#region INoise implementation

		public float GetValue (float x, float y)
		{
			return GetValue(x,y,0);
		}

		public float GetValue (float x, float y, float z)
		{
			return (float)_ridged.GetValue(x,y,z);
		}

		public ModuleBase ModuleBase {
			get {
				return _ridged;
			}
		}

		#endregion

		private RidgedMultifractal _ridged;

		public MTBRidgedMultifractal (int seed)
		{
			_ridged = new RidgedMultifractal();
			_ridged.Seed = seed;
		}

		/// <summary>
		/// Gets or sets the frequency of the first octave.
		/// </summary>
		public double Frequency
		{
			get { return _ridged.Frequency; }
			set { _ridged.Frequency = value; }
		}
		
		/// <summary>
		/// Gets or sets the lacunarity of the perlin noise.
		/// </summary>
		public double Lacunarity
		{
			get { return _ridged.Lacunarity; }
			set { _ridged.Lacunarity = value; }
		}
		
		/// <summary>
		/// Gets or sets the number of octaves of the perlin noise.
		/// </summary>
		public int OctaveCount
		{
			get { return _ridged.OctaveCount; }
			set { _ridged.OctaveCount = Mathf.Clamp(value, 1, Utils.OctavesMaximum); }
		}
	}
}

