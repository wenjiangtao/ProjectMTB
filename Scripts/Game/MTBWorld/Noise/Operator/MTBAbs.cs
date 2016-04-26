using System;
using LibNoise;
using LibNoise.Operator;
namespace MTB
{
	public class MTBAbs : IMTBNoise
	{

		#region INoise implementation

		public float GetValue (float x, float y)
		{
			return GetValue(x,y,0);
		}

		public float GetValue (float x, float y, float z)
		{
			return (float)_abs.GetValue(x,y,z);
		}

		public ModuleBase ModuleBase {
			get {
				return _abs;
			}
		}

		#endregion

		private Abs _abs;
		public MTBAbs (IMTBNoise input)
		{
			_abs = new Abs(input.ModuleBase);
		}
	}
}

