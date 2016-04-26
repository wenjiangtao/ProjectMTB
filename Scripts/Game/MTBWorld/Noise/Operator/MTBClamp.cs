using System;
using LibNoise.Operator;
namespace MTB
{
	public class MTBClamp : IMTBNoise
	{
		#region IMTBNoise implementation

		public float GetValue (float x, float y)
		{
			return GetValue(x,y,0);
		}

		public float GetValue (float x, float y, float z)
		{
			return (float)_clamp.GetValue(x,y,z);
		}

		public LibNoise.ModuleBase ModuleBase {
			get {
				return _clamp;
			}
		}

		#endregion

		private Clamp _clamp;
		public MTBClamp (IMTBNoise input)
		{
			_clamp = new Clamp(input.ModuleBase);
		}

		/// <summary>
		/// Gets or sets the maximum to clamp to.
		/// </summary>
		public float Maximum
		{
			get { return (float)_clamp.Maximum; }
			set { _clamp.Maximum = value; }
		}
		
		/// <summary>
		/// Gets or sets the minimum to clamp to.
		/// </summary>
		public float Minimum
		{
			get { return (float)_clamp.Minimum; }
			set { _clamp.Minimum = value; }
		}
	}
}

