using System;
using LibNoise.Operator;
namespace MTB
{
	public class MTBScaleBias : IMTBNoise
	{
		#region IMTBNoise implementation

		public float GetValue (float x, float y)
		{
			return GetValue(x,y,0);
		}

		public float GetValue (float x, float y, float z)
		{
			return (float)_scaleBias.GetValue(x,y,z);
		}

		public LibNoise.ModuleBase ModuleBase {
			get {
				return _scaleBias;
			}
		}

		#endregion


		/// <summary>
		/// Gets or sets the bias to apply to the scaled output value from the source module.
		/// </summary>
		public float Bias { get{return (float)_scaleBias.Bias;} set{_scaleBias.Bias = value;} }
		
		/// <summary>
		/// Gets or sets the scaling factor to apply to the output value from the source module.
		/// </summary>
		public float Scale { get{return (float)_scaleBias.Scale;} set{_scaleBias.Scale = value;} }

		private ScaleBias _scaleBias;
		public MTBScaleBias (IMTBNoise input)
		{
			_scaleBias = new ScaleBias(input.ModuleBase);
		}

		public MTBScaleBias(IMTBNoise input,float bias,float scale)
			:this(input)
		{
			this.Bias = bias;
			this.Scale = scale;
		}
	}
}

