using System;
using LibNoise.Operator;
namespace MTB
{
	public class MTBBlend : IMTBNoise
	{
		#region IMTBNoise implementation

		public float GetValue (float x, float y)
		{
			return GetValue(x,y,0);
		}

		public float GetValue (float x, float y, float z)
		{
			return (float)_blend.GetValue(x,y,z);
		}

		public LibNoise.ModuleBase ModuleBase {
			get {
				return _blend;
			}
		}

		#endregion

		private Blend _blend;
		public MTBBlend (IMTBNoise l,IMTBNoise r,IMTBNoise controller)
		{
			_blend = new Blend(l.ModuleBase,r.ModuleBase,controller.ModuleBase);
		}
	}
}

