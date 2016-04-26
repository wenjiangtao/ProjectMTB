using System;
using System.IO;
namespace MTB
{
	public interface IDataSerialize
	{
		void Serialize(Stream stream);
		void Deserialize(Stream stream);
	}
}

