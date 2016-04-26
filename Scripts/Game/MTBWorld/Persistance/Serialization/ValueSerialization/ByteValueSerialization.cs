using System;
namespace MTB
{
	public class ByteValueSerialization : IValueSerialization
	{

		#region IValueSerialization implementation

		public byte[] GetBytes (object o)
		{
			byte[] result = new byte[]{(byte)o};
			return result;
		}

		public object GetValue (byte[] value)
		{
			return value[0];
		}

		public int Length {
			get {
				return 1;
			}
		}

		#endregion

		public ByteValueSerialization ()
		{
		}
	}
}

