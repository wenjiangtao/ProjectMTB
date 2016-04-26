using System;
using System.IO;
using ICSharpCode.SharpZipLib.GZip;
namespace MTB
{
	public class GZipMTBCompress : IMTBCompress
	{
		#region IMTBCompress implementation

		public Stream Encompress (Stream sourceStream)
		{
			return sourceStream;
		}

		public Stream Decompress (Stream sourceStream)
		{
			return sourceStream;
		}

		#endregion

		#region IMTBCompress implementation

		public byte[] Encompress (byte[] data)
		{
			byte[] result;
			MemoryStream ms = new MemoryStream();
			GZipOutputStream stream = new GZipOutputStream(ms);
			stream.Write(data,0,data.Length);
			stream.Close();
			stream.Dispose();
			result = ms.ToArray();
			ms.Close();
			ms.Dispose();
			return result;
		}

		public byte[] Decompress (byte[] data)
		{
			int bufferSize = 4096;
			byte[] buffer = new byte[bufferSize];
			MemoryStream ms = new MemoryStream(data);
			MemoryStream resultMs = new MemoryStream();
			GZipInputStream stream = new GZipInputStream(ms);
			while(true)
			{
				int size = stream.Read(buffer,0,bufferSize);
				if(size == 0)break;
				resultMs.Write(buffer,0,size);
			}
			stream.Close();
			stream.Dispose();
			ms.Close();
			ms.Dispose();
			byte[] result = resultMs.ToArray();
			resultMs.Close();
			resultMs.Dispose();
			return result;
		}

		public MTBCompressType CompressType {
			get {
				return MTBCompressType.GZip;
			}
		}

		#endregion

		public GZipMTBCompress ()
		{
		}
	}
}

