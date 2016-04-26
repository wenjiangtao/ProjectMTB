using System;
namespace MTB
{
	public interface IByteSerialization
	{
		byte[] Serialize();
		void Deserialize(byte[] data);
	}
}

