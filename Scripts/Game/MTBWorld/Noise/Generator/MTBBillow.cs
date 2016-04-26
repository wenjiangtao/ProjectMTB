using System;
using LibNoise;
using LibNoise.Generator;
using UnityEngine;
namespace MTB
{
	public class MTBBillow : IMTBNoise
	{
		#region INoise implementation

		public float GetValue (float x, float y)
		{
			return GetValue(x,y,0);
		}

		public float GetValue (float x, float y, float z)
		{
			return (float)_billow.GetValue(x,y,z);
		}

		public ModuleBase ModuleBase {
			get {
				return _billow;
			}
		}

		#endregion

		private Billow _billow;

		public MTBBillow (int seed)
		{
			_billow = new Billow();
			_billow.Seed = seed;
		}

		/// <summary>
		/// Gets or sets the frequency of the first octave.
		/// </summary>
		public double Frequency
		{
			get { return _billow.Frequency; }
			set { _billow.Frequency = value; }
		}
		
		/// <summary>
		/// Gets or sets the quality of the perlin noise.
		/// </summary>
		public QualityMode Quality
		{
			get { return _billow.Quality; }
			set { _billow.Quality = value; }
		}
		
		/// <summary>
		/// Gets or sets the number of octaves of the perlin noise.
		/// </summary>
		public int OctaveCount
		{
			get { return _billow.OctaveCount; }
			set { _billow.OctaveCount = Mathf.Clamp(value, 1, Utils.OctavesMaximum); }
		}
		
		/// <summary>
		/// Gets or sets the persistence of the perlin noise.
		/// </summary>
		public double Persistence
		{
			get { return _billow.Persistence; }
			set { _billow.Persistence = value; }
		}
	}
}

