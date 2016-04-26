using System;
namespace MTB
{
	public class Int32ValueSerialization : IValueSerialization
	{
		#region IValueSerialization implementation

		public byte[] GetBytes (object o)
		{
			Int32 value = (Int32)o;
			return BitConverter.GetBytes(value);
		}

		public object GetValue (byte[] value)
		{
			return BitConverter.ToInt32(value,0);
		}

		public int Length {
			get {
				return 4;
			}
		}

		#endregion

		public Int32ValueSerialization ()
		{
		}
	}
}

