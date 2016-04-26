using System;
namespace MTB
{
	public class NoneMTBCompress : IMTBCompress
	{
		#region IMTBCompress implementation

		public System.IO.Stream Encompress (System.IO.Stream sourceStream)
		{
			return sourceStream;
		}

		public System.IO.Stream Decompress (System.IO.Stream sourceStream)
		{
			return sourceStream;
		}

		#endregion

		#region IMTBCompress implementation

		public byte[] Encompress (byte[] data)
		{
			return data;
		}

		public byte[] Decompress (byte[] data)
		{
			return data;
		}

		public MTBCompressType CompressType {
			get {
				return MTBCompressType.None;
			}
		}

		#endregion

		public NoneMTBCompress ()
		{
		}
	}
}

