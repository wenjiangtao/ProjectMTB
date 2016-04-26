using System;
using System.IO;
namespace MTB
{
	public interface IMTBCompress
	{
		byte[] Encompress(byte[] data);
		byte[] Decompress(byte[] data);
		Stream Encompress(Stream sourceStream);
		Stream Decompress(Stream sourceStream);

		MTBCompressType CompressType{get;}
	}
}

