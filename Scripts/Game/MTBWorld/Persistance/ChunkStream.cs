using System;
using System.IO;
namespace MTB
{
	public class ChunkStream : Stream
	{
		#region implemented abstract members of Stream

		public override void Flush ()
		{
			ms.Flush();
		}

		public override long Seek (long offset, SeekOrigin origin)
		{
			return ms.Seek(offset,origin);
		}

		public override void SetLength (long value)
		{
			ms.SetLength(value);
		}

		public override int Read (byte[] buffer, int offset, int count)
		{
			return ms.Read(buffer,offset,count);
		}

		public override void Write (byte[] buffer, int offset, int count)
		{
			ms.Write(buffer,offset,count);
		}

		public override bool CanRead {
			get {
				return ms.CanRead;
			}
		}

		public override bool CanSeek {
			get {
				return ms.CanSeek;
			}
		}

		public override bool CanWrite {
			get {
				return ms.CanWrite;
			}
		}

		public override long Length {
			get {
				return ms.Length;
			}
		}

		public override long Position {
			get {
				return ms.Position;
			}
			set {
				ms.Position = value;
			}
		}

		public override void Close ()
		{
			ms.Close();
		}

		#endregion

		private MemoryStream ms;
		byte[] buffer = new byte[1024];
		public ChunkStream ()
		{
			ms = new MemoryStream();
		}

		public byte[] ToArray()
		{
			return ms.ToArray();
		}

		public void Write(Stream input,int length)
		{
			int totalLen = buffer.Length;
			int len;
			while(totalLen < length)
			{
				len = input.Read(buffer,0,buffer.Length);
				ms.Write(buffer,0,len);
				totalLen += len;
			}
			int nextLength = length + buffer.Length - totalLen;
			input.Read(buffer,0,nextLength);
			ms.Write(buffer,0,nextLength);
			ms.Flush();
		}

		public void Read(Stream output,int length)
		{
			int totalLen = buffer.Length;
			int len;
			while(totalLen < length)
			{
				len = ms.Read(buffer,0,buffer.Length);
				output.Write(buffer,0,len);
				totalLen += len;
			}
			int nextLength = length + buffer.Length - totalLen;
			ms.Read(buffer,0,nextLength);
			output.Write(buffer,0,nextLength);
			ms.Flush();
		}
	}
}

