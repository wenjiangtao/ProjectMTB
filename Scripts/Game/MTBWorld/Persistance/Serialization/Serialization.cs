using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
namespace MTB
{
	public class Serialization
	{
		public Serialization ()
		{
		}
		//对象序列化（会增加很大的存储）
		public static byte[] Serialize<T>(T t)
		{
			MemoryStream mStream = new MemoryStream();
			BinaryFormatter bFormatter = new BinaryFormatter();
			bFormatter.Serialize(mStream, t);
			return mStream.GetBuffer();
		}
		//反序列化
		public static T Deserialize<T>(byte[] b)
		{
			BinaryFormatter bFormatter = new BinaryFormatter();
			return (T)bFormatter.Deserialize(new MemoryStream(b));
		}

		public static byte[] SerializeArrayTwo<T>(T[,] map,ValueSerializationType type)
		{
			IValueSerialization serialization = ValueSerializationFactory.GetSerialization(type);
			MemoryStream mStream = new MemoryStream();
			int width = map.GetLength(0);
			int height = map.GetLength(1);
			byte[] widthBytes = BitConverter.GetBytes(width);
			byte[] heightBytes = BitConverter.GetBytes(height);
			mStream.Write(widthBytes,0,widthBytes.Length);
			mStream.Write(heightBytes,0,heightBytes.Length);
			for (int x = 0; x < width; x++) {
				for (int z = 0; z < height; z++) {
					byte[] buffer = serialization.GetBytes(map[x,z]);
					mStream.Write(buffer,0,buffer.Length);
				}
			}
			byte[] result = mStream.ToArray();
			mStream.Close();
			mStream.Dispose();
			return result;
		}

		public static T[,] DeserializeArrayTwo<T>(byte[] data,ValueSerializationType type)
		{
			IValueSerialization serialization = ValueSerializationFactory.GetSerialization(type);
			MemoryStream mStream = new MemoryStream(data);
			byte[] widthBytes = new byte[4];
			byte[] heightBytes = new byte[4];
			mStream.Read(widthBytes,0,widthBytes.Length);
			mStream.Read(heightBytes,0,heightBytes.Length);
			int width = BitConverter.ToInt32(widthBytes,0);
			int height = BitConverter.ToInt32(heightBytes,0);
			T[,] result = new T[width,height];
			for (int x = 0; x < width; x++) {
				for (int z = 0; z < height; z++) {
					byte[] buffer = new byte[serialization.Length];
					mStream.Read(buffer,0,buffer.Length);
					T value = (T)serialization.GetValue(buffer);
					result[x,z] = value;
				}
			}
			return result;
		}

		public static byte[] SerializeArrayThree<T>(T[,,] map,ValueSerializationType type)
		{
			IValueSerialization serialization = ValueSerializationFactory.GetSerialization(type);
			MemoryStream mStream = new MemoryStream();
			int width = map.GetLength(0);
			int height = map.GetLength(1);
			int depth = map.GetLength(2);
			byte[] widthBytes = BitConverter.GetBytes(width);
			byte[] heightBytes = BitConverter.GetBytes(height);
			byte[] depthBytes = BitConverter.GetBytes(depth);
			mStream.Write(widthBytes,0,widthBytes.Length);
			mStream.Write(heightBytes,0,heightBytes.Length);
			mStream.Write(depthBytes,0,depthBytes.Length);
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					for (int z = 0; z < depth; z++) {
						byte[] buffer = serialization.GetBytes(map[x,y,z]);
						mStream.Write(buffer,0,buffer.Length);
					}
				}
			}
			byte[] result = mStream.ToArray();
			mStream.Close();
			mStream.Dispose();
			return result;
		}
		
		public static T[,,] DeserializeArrayThree<T>(byte[] data,ValueSerializationType type)
		{
			IValueSerialization serialization = ValueSerializationFactory.GetSerialization(type);
			MemoryStream mStream = new MemoryStream(data);
			byte[] widthBytes = new byte[4];
			byte[] heightBytes = new byte[4];
			byte[] depthBytes = new byte[4];
			mStream.Read(widthBytes,0,widthBytes.Length);
			mStream.Read(heightBytes,0,heightBytes.Length);
			mStream.Read(depthBytes,0,depthBytes.Length);
			int width = BitConverter.ToInt32(widthBytes,0);
			int height = BitConverter.ToInt32(heightBytes,0);
			int depth = BitConverter.ToInt32(depthBytes,0);
			T[,,] result = new T[width,height,depth];
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					for (int z = 0; z < depth; z++) {
						byte[] buffer = new byte[serialization.Length];
						mStream.Read(buffer,0,buffer.Length);
						T value = (T)serialization.GetValue(buffer);
						result[x,y,z] = value;
					}
				}
			}
			return result;
		}

		public static byte[] SerializeList(List<byte[]> list)
		{
			MemoryStream mStream = new MemoryStream();
			byte[] listLengthBytes = BitConverter.GetBytes(list.Count);
			mStream.Write(listLengthBytes,0,listLengthBytes.Length);
			for (int i = 0; i < list.Count; i++) {
				byte[] data = list[i];
				byte[] lengthBytes = BitConverter.GetBytes(data.Length);
				mStream.Write(lengthBytes,0,lengthBytes.Length);
				mStream.Write(data,0,data.Length);
			}
			byte[] result = mStream.ToArray();
			mStream.Close();
			mStream.Dispose();
			return result;
		}

		public static List<byte[]> DeserializeList(byte[] data)
		{
			MemoryStream mStream = new MemoryStream(data);
			byte[] listLengthBytes = new byte[4];
			mStream.Read(listLengthBytes,0,listLengthBytes.Length);
			int listLength = BitConverter.ToInt32(listLengthBytes,0);
			List<byte[]> list = new List<byte[]>();
			for (int i = 0; i < listLength; i++) {
				byte[] lengthByte = new byte[4];
				mStream.Read(lengthByte,0,lengthByte.Length);
				int length = BitConverter.ToInt32(lengthByte,0);
				byte[] singleData = new byte[length];
				mStream.Read(singleData,0,singleData.Length);
				list.Add(singleData);
			}
			return list;
		}

		public static void WriteIntToStream(Stream stream,int value)
		{
			stream.WriteByte((byte)value);
			stream.WriteByte((byte)(value >> 8));
			stream.WriteByte((byte)(value >> 16));
			stream.WriteByte((byte)(value >> 24));
		}

		public static int ReadIntFromStream(Stream stream)
		{
			return (int)((uint)(stream.ReadByte() | (stream.ReadByte() << 8) | (stream.ReadByte() << 16) | (stream.ReadByte() << 24)));
		}

		public static void WriteIntToByteArr(byte[] data,int value)
		{
			data[0] = (byte)value;
			data[1] = (byte)(value >> 8);
			data[2] = (byte)(value >> 16);
			data[3] = (byte)(value >> 24);
		}

		public static int ReadIntFromByteArr(byte[] data)
		{
			return (int)(data[0] | data[1] << 8 | data[2] << 16 | data[3] << 24);
		}

		public static void WriteBoolToStream(Stream stream,bool value)
		{
			byte result = 0x00;
			if(value)result = 0x01;
			stream.WriteByte(result);
		}

		public static bool ReadBoolFromStream(Stream stream)
		{
			if(stream.ReadByte() == 0)return false;
			return true;
		}

	}
}

