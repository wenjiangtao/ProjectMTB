using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class PackageBuffer
{
	MemoryStream ms;
	BinaryReader br;
	int readPos = 0;
	int writePos = 0;
	
	public PackageBuffer()
	{
		ms = new MemoryStream ();
		br = new BinaryReader (ms);
	}
	
	public int GetDataLength()
	{
		return writePos - readPos;
	}
	
	public byte[] GetDataBuffer()
	{
		return ms.GetBuffer ();
	}
	
	public void SetReadPos(int pos, SeekOrigin seekOrigin = SeekOrigin.Begin)
	{
		if (seekOrigin == SeekOrigin.Current)
			pos = readPos + pos;
		readPos = pos;
	}
	
	public int GetReadPos()
	{
		return readPos;
	}
	
	public byte[] ReadBytes(int length)
	{
		BeforeRead ();
		int dataLength = this.GetDataLength ();
		if (length > dataLength)
			length = dataLength;
		
		byte[] res = br.ReadBytes(length);
		AfterRead ();
		return res;
	}
	
	public void Write(byte[] buffer, int offset, int length)
	{
		//BeforeWrite
		BeforeWrite ();
		ms.Write (buffer, offset, length);
		ms.Flush();
		AfterWrite ();
	}
	
	public void Write(byte[] buffer)
	{
		Write (buffer, 0, buffer.Length);
	}
	
	public int ReadInt()
	{
		BeforeRead ();
		int res = br.ReadInt32 ();
		AfterRead ();
		return res;
	}
	
	void BeforeWrite()
	{
		ms.Position = writePos;
	}
	
	void AfterWrite()
	{
		writePos = (int)ms.Position;
		if (ms.Length > this.GetDataLength () * 2) {
			byte[] buffer = ms.GetBuffer();
			int length = this.GetDataLength();
			System.Array.Copy(buffer, this.readPos, buffer, 0, length);
			this.readPos = 0;
			this.writePos = length;
			ms.Seek(0, SeekOrigin.Begin);
			ms.Position = writePos;
		}
	}
	
	void BeforeRead()
	{
		ms.Position = readPos;
	}
	
	void AfterRead()
	{
		readPos = (int)ms.Position;
		ms.Position = writePos;
	}
}