using System;
using LibNoise.Operator;
namespace MTB
{
	public class MTBSelect : IMTBNoise
	{
		#region IMTBNoise implementation

		public float GetValue (float x, float y)
		{
			return GetValue(x,y,0);
		}

		public float GetValue (float x, float y, float z)
		{
			return (float)_select.GetValue(x,y,z);
		}

		public LibNoise.ModuleBase ModuleBase {
			get {
				return _select;
			}
		}

		#endregion

		private Select _select;
		public MTBSelect (IMTBNoise l,IMTBNoise r,IMTBNoise controller)
		{
			_select = new Select(l.ModuleBase,r.ModuleBase,controller.ModuleBase);
		}

		/// <summary>
		/// Gets or sets the falloff value at the edge transition.
		/// </summary>
		/// <remarks>
		/// Called SetEdgeFallOff() on the original LibNoise.
		/// </remarks>
		public double FallOff
		{
			get { return _select.FallOff; }
			set
			{
				_select.FallOff = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the maximum, and re-calculated the fall-off accordingly.
		/// </summary>
		public double Maximum
		{
			get { return _select.Maximum; }
			set
			{
				_select.Maximum = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the minimum, and re-calculated the fall-off accordingly.
		/// </summary>
		public double Minimum
		{
			get { return _select.Minimum; }
			set
			{
				_select.Minimum = value;
			}
		}
	}
}

