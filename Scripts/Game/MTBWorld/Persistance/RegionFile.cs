using System;
using System.IO;
using System.Linq;
using System.Collections;
using UnityEngine;
namespace MTB
{
	public class RegionFile
	{
		public const int SECTOR_LENGTH = 4096;
		public const int REGION_WIDTH = 32;
		public const int REGION_DEPTH = 32;
		private MTBCompressType _compressType;
		private string _filename;
		private FileStream _fs;
		private ChunkStream _cs;
		private ChunkHeadData[,] _headData;
		private ArrayList _useSector;
		public RegionFile (string filename)
			:this(filename,MTBCompressType.None)
		{
		}

		public RegionFile (string filename,MTBCompressType type)
		{
			_filename = filename;
			_compressType = type;
		}

		public void Init()
		{
			FileInfo fi = new FileInfo(_filename);
			if(!fi.Directory.Exists)
			{
				fi.Directory.Create();
			}
			_fs = fi.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite);
			if(_fs.Length < SectorToByte(2))
			{
				byte[] headByte = Enumerable.Repeat((byte)0x00,(int)SectorToByte(2)).ToArray();
				_fs.Position = 0;
				_fs.Write(headByte,0,headByte.Length);
			}
			_headData = new ChunkHeadData[REGION_WIDTH,REGION_DEPTH];
			_useSector = new ArrayList(new bool[ByteToSector(_fs.Length)]);
			_useSector[0] = true;
			_useSector[1] = true;
			for (int x = 0; x < REGION_WIDTH; x++) {
				for (int z = 0; z < REGION_DEPTH; z++) {
					_headData[x,z] = new ChunkHeadData(x,z);
					ReadHeadData(x,z);
				}
			}
			_cs = new ChunkStream();
		}

		private void ReadHeadData(int x,int z)
		{
			int startPosition = (x * REGION_DEPTH + z) * 4;
			_fs.Position = startPosition;
			byte[] blockOffsetByte = new byte[4];
			_fs.Read(blockOffsetByte,0,blockOffsetByte.Length - 1);
			int blockOffset = Serialization.ReadIntFromByteArr(blockOffsetByte);
			//如果块的偏移量小于两个扇区大小，那么当前块没有创建
			if(blockOffset < 2)return;
			byte blockLength = (byte)_fs.ReadByte();
			if(blockLength == 0)
			{
				_headData[x,z].status = ChunkDataStatus.Length_Error;
				return;
			}

//			//读入时间戳
//			_fs.Position = startPosition + SectorToByte(2);
//			byte[] timestampByte = new byte[4];
//			_fs.Read(timestampByte,0,timestampByte.Length);

			_fs.Position = SectorToByte(blockOffset);
			byte[] chunkSizeByte = new byte[4];
			_fs.Read(chunkSizeByte,0,chunkSizeByte.Length);
			int chunkSize = Serialization.ReadIntFromByteArr(chunkSizeByte);
			MTBCompressType compressFormat = (MTBCompressType)_fs.ReadByte();
			_headData[x,z].BlockOffset = blockOffset;
			_headData[x,z].BlockLength = blockLength;
			_headData[x,z].Length = chunkSize;
			_headData[x,z].CompressionType = compressFormat;
			_headData[x,z].status = ChunkDataStatus.OK;
			for (int i = blockOffset; i < blockOffset + blockLength; i++) {
				_useSector[i] = true;
			}
		}

		private void ClearChunkData(int x,int z)
		{
			//如果当前块已创建，默认设置其为可存储扇区
			if(_headData[x,z].status == ChunkDataStatus.OK)
			{
				int startIndex = _headData[x,z].BlockOffset;
				int length = _headData[x,z].BlockLength;
				for (int i = startIndex; i < startIndex + length; i++) {
					_useSector[i] = false;
				}
				byte[] chunkByte = Enumerable.Repeat((byte)0x00,(int)SectorToByte(length)).ToArray();
				_fs.Position = SectorToByte(startIndex);
				_fs.Write(chunkByte,0,chunkByte.Length);
			}
		}

		private int GetChunkCanSavedSector(int sectorLength)
		{
			int startIndex = _useSector.Count;
			for (int i = 2; i < _useSector.Count; i++) {
				if(AllCanUseSector(i,sectorLength))
				{
					startIndex = i;
					break;
				}
			}
			int addSectorLength = startIndex + sectorLength - _useSector.Count;
			if(addSectorLength > 0)
			{
				_useSector.AddRange(new bool[addSectorLength]);
			}
			return startIndex;
		}

		private bool AllCanUseSector(int startIndex,int length)
		{
			for (int i = startIndex; i < startIndex + length; i++) {
				if(i < _useSector.Count && (bool)_useSector[i])return false;
			}
			return true;
		}

		private int GetLastUseSectorIndex()
		{
			for (int i = _useSector.Count - 1; i >= 0; i--) {
				if((bool)_useSector[i])return i;
			}
			return -1;
		}

		public int ByteToSector(long size,int sectorLength = SECTOR_LENGTH)
		{
			return (int)Math.Ceiling((double)size / sectorLength);
		}

		public long SectorToByte(int size,int sectorLength = SECTOR_LENGTH)
		{
			return size * sectorLength;
		}

		public bool GetChunkData(int x,int z,ChunkByteData chunkByteData)
		{
			if(_headData[x,z].status == ChunkDataStatus.Not_Create)return false;
			_fs.Position = SectorToByte(_headData[x,z].BlockOffset) + 5;

			byte[] data = new byte[_headData[x,z].Length];
			_fs.Read(data,0,data.Length);
			chunkByteData.data = data;
			chunkByteData.compressType = _headData[x,z].CompressionType;
			return true;
		}

		public void SaveChunkData(int x,int z,ChunkByteData chunkByteData)
		{
			byte[] chunkSize = new byte[4];
			Serialization.WriteIntToByteArr(chunkSize,(int)chunkByteData.data.Length);
			int singleChunkLength = (int)chunkByteData.data.Length + 5;
			
			int blockLength = ByteToSector(singleChunkLength);
			
			ClearChunkData(x,z);

			int blockOffset = GetChunkCanSavedSector(blockLength);
			long blockOffsetStartByteIndex = SectorToByte(blockOffset);
			_fs.Position = blockOffsetStartByteIndex;
			//写入区块长度
			_fs.Write(chunkSize,0,chunkSize.Length);
			//写入压缩格式
			_fs.WriteByte((byte)chunkByteData.compressType);
			//写入区块压缩数据
			_fs.Write(chunkByteData.data,0,(int)chunkByteData.data.Length);

			int startPosition = (x * REGION_DEPTH + z) * 4;
			_fs.Position = startPosition;
			byte[] blockOffsetByte = new byte[4];
			Serialization.WriteIntToByteArr(blockOffsetByte,blockOffset);
			_fs.Write(blockOffsetByte,0,3);
			_fs.WriteByte((byte)blockLength);
			
			//更新可存储标志，并将块后面不足一个扇区的部分写0x00
			for (int i = blockOffset; i < blockOffset + blockLength; i++) {
				_useSector[i] = true;
			}
			long chunkEnd = blockOffsetStartByteIndex + singleChunkLength;
			long sectorEnd = blockOffsetStartByteIndex + SectorToByte(blockLength);
			if(chunkEnd < sectorEnd)
			{
				_fs.Position = chunkEnd;
				byte[] extendByte = Enumerable.Repeat((byte)0x00,(int)(sectorEnd - chunkEnd)).ToArray();
				_fs.Write(extendByte,0,extendByte.Length);
			}
			//截取文件最后没有使用的扇区
			int lastUseSectorIndex = GetLastUseSectorIndex();
			if(lastUseSectorIndex != -1 && lastUseSectorIndex != _useSector.Count - 1)
			{
				int truncatedIndex = lastUseSectorIndex + 1;
				_useSector.RemoveRange(truncatedIndex,_useSector.Count - truncatedIndex);
				_fs.SetLength(SectorToByte(truncatedIndex));
			}
			//更新headData数据
			_headData[x,z].BlockOffset = blockOffset;
			_headData[x,z].BlockLength = (byte)blockLength;
			_headData[x,z].Length = chunkByteData.data.Length;
			_headData[x,z].status = ChunkDataStatus.OK;
			_headData[x,z].CompressionType = chunkByteData.compressType;

		}

		//获取到的块数据
		public bool GetChunkData(int x,int z,Chunk chunk)
		{
			if(_headData[x,z].status == ChunkDataStatus.Not_Create)return false;
			_fs.Position = SectorToByte(_headData[x,z].BlockOffset) + 5;
			_cs.SetLength(0);
			_cs.Write(_fs,_headData[x,z].Length);
			_cs.Position = 0;
			IMTBCompress compress = MTBCompressFactory.GetCompress(_headData[x,z].CompressionType);
			_cs = (ChunkStream)compress.Decompress(_cs);
			_cs.Position = 0;
			chunk.Deserialize(_cs);
			return true;
		}

		//保存块的字节数据
		public void SaveChunkData(int x,int z,Chunk chunk)
		{
			_cs.SetLength(0);
			chunk.Serialize(_cs);
			_cs.Position = 0;
			IMTBCompress compress = MTBCompressFactory.GetCompress(_compressType);
			_cs = (ChunkStream)compress.Encompress(_cs);
			byte[] chunkSize = new byte[4];

			Serialization.WriteIntToByteArr(chunkSize,(int)_cs.Length);
			int singleChunkLength = (int)_cs.Length + 5;

			int blockLength = ByteToSector(singleChunkLength);

			ClearChunkData(x,z);
			int blockOffset = GetChunkCanSavedSector(blockLength);
			long blockOffsetStartByteIndex = SectorToByte(blockOffset);
			_fs.Position = blockOffsetStartByteIndex;
			//写入区块长度
			_fs.Write(chunkSize,0,chunkSize.Length);
			//写入压缩格式
			_fs.WriteByte((byte)compress.CompressType);
			//写入区块压缩数据
			_cs.Position = 0;
			_cs.Read(_fs,(int)_cs.Length);
			int startPosition = (x * REGION_DEPTH + z) * 4;
			_fs.Position = startPosition;
			byte[] blockOffsetByte = new byte[4];
			Serialization.WriteIntToByteArr(blockOffsetByte,blockOffset);
			_fs.Write(blockOffsetByte,0,3);
			_fs.WriteByte((byte)blockLength);

			//更新可存储标志，并将块后面不足一个扇区的部分写0x00
			for (int i = blockOffset; i < blockOffset + blockLength; i++) {
				_useSector[i] = true;
			}
			long chunkEnd = blockOffsetStartByteIndex + singleChunkLength;
			long sectorEnd = blockOffsetStartByteIndex + SectorToByte(blockLength);
			if(chunkEnd < sectorEnd)
			{
				_fs.Position = chunkEnd;
				byte[] extendByte = Enumerable.Repeat((byte)0x00,(int)(sectorEnd - chunkEnd)).ToArray();
				_fs.Write(extendByte,0,extendByte.Length);
			}
			//截取文件最后没有使用的扇区
			int lastUseSectorIndex = GetLastUseSectorIndex();
			if(lastUseSectorIndex != -1 && lastUseSectorIndex != _useSector.Count - 1)
			{
				int truncatedIndex = lastUseSectorIndex + 1;
				_useSector.RemoveRange(truncatedIndex,_useSector.Count - truncatedIndex);
				_fs.SetLength(SectorToByte(truncatedIndex));
			}
			//更新headData数据
			_headData[x,z].BlockOffset = blockOffset;
			_headData[x,z].BlockLength = (byte)blockLength;
			_headData[x,z].Length = (int)_cs.Length;
			_headData[x,z].status = ChunkDataStatus.OK;
			_headData[x,z].CompressionType = compress.CompressType;
		}

		public void Close()
		{
			if(_fs != null)
			{
				_fs.Close();
				_fs.Dispose();
				_fs = null;
			}
		}
	}

	public class ChunkByteData
	{
		public byte[] data{get;set;}
		public MTBCompressType compressType{get;set;}

		public ChunkByteData()
		{
		}
	}

	class ChunkHeadData
	{
		public int X{get;set;}
		public int Z{get;set;}
		public int BlockOffset{get;set;}
		public byte BlockLength{get;set;}
		public long Timestamp{get;set;}
		public int Length{get;set;}
		public MTBCompressType CompressionType{get;set;}
		public ChunkDataStatus status{get;set;}
		public ChunkHeadData(int x,int z)
		{
			X = x;
			Z = z;
			BlockOffset = 0;
			BlockLength = 0;
			Timestamp = 0;
			Length = 0;
			CompressionType = MTBCompressType.None;
			status = ChunkDataStatus.Not_Create;
		}
	}

	enum ChunkDataStatus
	{
		OK = 0,
		Not_Create=1,
		Length_Error = 2
	}
}

