using System;
using LibNoise.Operator;
namespace MTB
{
	public class MTBScale : IMTBNoise
	{
		#region IMTBNoise implementation

		public float GetValue (float x, float y)
		{
			return GetValue(x,y,0);
		}

		public float GetValue (float x, float y, float z)
		{
			return (float)_scale.GetValue(x,y,z);
		}

		public LibNoise.ModuleBase ModuleBase {
			get {
				return _scale;
			}
		}

		#endregion

		private Scale _scale;
		public MTBScale (IMTBNoise input)
		{
			_scale = new Scale(input.ModuleBase);
		}

		public MTBScale(IMTBNoise input,float x,float y)
			:this(input)
		{
			this.X = x;
			this.Y = y;
			this.Z = 1;
		}

		public MTBScale(IMTBNoise input,float x,float y,float z)
			:this(input,x,y)
		{
			this.Z = z;
		}

		/// <summary>
		/// Gets or sets the scaling factor on the x-axis.
		/// </summary>
		public float X
		{
			get { return (float)_scale.X; }
			set { _scale.X = value; }
		}
		
		/// <summary>
		/// Gets or sets the scaling factor on the y-axis.
		/// </summary>
		public float Y
		{
			get { return (float)_scale.Y; }
			set { _scale.Y = value; }
		}
		
		/// <summary>
		/// Gets or sets the scaling factor on the z-axis.
		/// </summary>
		public float Z
		{
			get { return (float)_scale.Z; }
			set { _scale.Z = value; }
		}
	}
}

