using System;
namespace MTB
{
	public interface IValueSerialization
	{
		byte[] GetBytes(object o);
		object GetValue(byte[] value);
		int Length{get;}
	}
}

