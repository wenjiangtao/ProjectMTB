using System;
namespace MTB
{
	public class BlockValueSerialization : IValueSerialization
	{
		#region IValueSerialization implementation

		public byte[] GetBytes (object o)
		{
			Block block = (Block)o;
			byte[] result = new byte[]{(byte)block.BlockType};
			return result;
		}

		public object GetValue (byte[] value)
		{
			Block block = new Block((BlockType)value[0]);
			return block;
		}

		public int Length {
			get {
				return 1;
			}
		}

		#endregion

		public BlockValueSerialization ()
		{
		}
	}
}

