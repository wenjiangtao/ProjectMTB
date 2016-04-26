using System;
using System.IO;
namespace MTB
{
	public class NibbleArray
	{
		private byte[] a;
		private int b;
		private int c;
		
		public NibbleArray(int i, int j) {
			this.a = new byte[i >> 1];
			this.b = j;
			this.c = j + 4;
		}
		
		public NibbleArray(byte[] abyte, int i) {
			this.a = abyte;
			this.b = i;
			this.c = i + 4;
		}
		
		public int GetData(int i, int j, int k) {
			int l = j << this.c | k << this.b | i;
			int i1 = l >> 1;
			int j1 = l & 1;
			
			return j1 == 0 ? this.a[i1] & 15 : this.a[i1] >> 4 & 15;
		}
		
		public void SetData(int i, int j, int k, int value) {
			int i1 = j << this.c | k << this.b | i;
			int j1 = i1 >> 1;
			int k1 = i1 & 1;
			
			if (k1 == 0) {
				this.a[j1] = (byte) (this.a[j1] & 240 | value & 15);
			} else {
				this.a[j1] = (byte) (this.a[j1] & 15 | (value & 15) << 4);
			}
		}

		public void WriteBytes(Stream stream)
		{
			stream.Write(a,0,a.Length);
		}

		public void ReadBytes(Stream stream)
		{
			stream.Read(a,0,a.Length);
		}

		public void Clear()
		{
			Array.Clear(a,0,a.Length);
		}
	}
}

