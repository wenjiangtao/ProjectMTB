using System;
using System.IO;
using zlib;
namespace MTB
{
	public class ZLibMTBCompress : IMTBCompress
	{
		private MemoryStream outputStream = new MemoryStream();
		#region IMTBCompress implementation

		public Stream Encompress (Stream sourceStream)
		{
			outputStream.SetLength(0);
			//压缩
			ZOutputStream zStream = new ZOutputStream(outputStream,zlib.zlibConst.Z_DEFAULT_COMPRESSION);
			CopyStream(sourceStream,zStream);
			zStream.finish();
			sourceStream.SetLength(0);
			outputStream.Position = 0;
			CopyStream(outputStream,sourceStream);
			return sourceStream;
		}

		public Stream Decompress (Stream sourceStream)
		{
			outputStream.SetLength(0);
			//解压
			ZOutputStream zStream = new ZOutputStream(outputStream);
			CopyStream(sourceStream,zStream);
			sourceStream.SetLength(0);
			outputStream.Position = 0;
			CopyStream(outputStream,sourceStream);
			return sourceStream;
		}

		#endregion

		private void CopyStream(Stream input, Stream output)
		{
			byte[] buffer = new byte[4096];
			int len;
			while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
			{
				output.Write(buffer, 0, len);
			}
			output.Flush();
		}

		#region IMTBCompress implementation

		public byte[] Encompress (byte[] data)
		{
			MemoryStream outputStream = new MemoryStream();
			//压缩
			ZOutputStream zStream = new ZOutputStream(outputStream,zlib.zlibConst.Z_DEFAULT_COMPRESSION);
			zStream.Write(data,0,data.Length);
			zStream.finish();
			byte[] result = outputStream.ToArray();
			zStream.Close();
			outputStream.Close();
			outputStream.Dispose();
			return result;
		}

		public byte[] Decompress (byte[] data)
		{
			MemoryStream outStream = new MemoryStream();
			//解压
			ZOutputStream zStream = new ZOutputStream(outStream);
			zStream.Write(data,0,data.Length);
			outStream.Flush();
			byte[] result = outStream.ToArray();
			zStream.Close();
			outStream.Close();
			outStream.Dispose();
			return result;
		}

		public MTBCompressType CompressType {
			get {
				return MTBCompressType.ZLib;
			}
		}

		#endregion

		public ZLibMTBCompress ()
		{
		}
	}
}

