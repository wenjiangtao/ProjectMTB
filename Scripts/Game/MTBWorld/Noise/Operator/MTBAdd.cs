using System;
using LibNoise.Operator;
namespace MTB
{
	public class MTBAdd : IMTBNoise
	{
		#region IMTBNoise implementation

		public float GetValue (float x, float y)
		{
			return GetValue(x,y,0);
		}

		public float GetValue (float x, float y, float z)
		{
			return (float)_add.GetValue(x,y,z);
		}

		public LibNoise.ModuleBase ModuleBase {
			get {
				return _add;
			}
		}

		#endregion

		private Add _add;
		public MTBAdd (IMTBNoise l,IMTBNoise r)
		{
			_add = new Add(l.ModuleBase,r.ModuleBase);
		}
	}
}

