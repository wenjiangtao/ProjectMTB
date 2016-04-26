using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
namespace MTB
{
    public class WorldPersistanceManager
    {
        private Dictionary<WorldPos, RegionFile> _map;
        private Dictionary<WorldPos, RegionFile> _netMap;
        private static WorldPersistanceManager _instance;
        public WorldPersistanceManager()
        {
            _map = new Dictionary<WorldPos, RegionFile>();
            _netMap = new Dictionary<WorldPos, RegionFile>();
        }

        public static WorldPersistanceManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new WorldPersistanceManager();
                }
                return _instance;
            }
        }

        public void Init()
        {
        }

        public List<WorldFileInfo> GetAllWorldInfo()
        {
            List<WorldFileInfo> infos = new List<WorldFileInfo>();
            DirectoryInfo di = new DirectoryInfo(GameConfig.Instance.WorldSavedPath);
            string[] split = { "_", "_" };
            if (di.Exists)
            {
                DirectoryInfo[] fis = di.GetDirectories();
                for (int i = 0; i < fis.Length; i++)
                {

                    string name = fis[i].Name;
                    string[] result = name.Split(split, StringSplitOptions.RemoveEmptyEntries);
                    if (result.Length == 3)
                    {
                        string lastSaveTime = fis[i].LastWriteTime.Year + "-" + fis[i].LastWriteTime.Month + "-" + fis[i].LastWriteTime.Day + "-" + fis[i].LastWriteTime.Hour + "-" + fis[i].LastWriteTime.Minute + "-" + fis[i].LastWriteTime.Second;
                        WorldFileInfo worldFileInfo = new WorldFileInfo(result[0], result[1], lastSaveTime, Convert.ToInt32(result[2]));
                        infos.Add(worldFileInfo);
                    }
                }
            }
            return infos;
        }

        private MemoryStream _ms = new MemoryStream();
        public Chunk GetChunk(NetChunk netChunk)
        {
            Chunk chunk = netChunk.chunk;
            _ms.SetLength(0);
            _ms.Write(netChunk.chunkData.data.data, 0, netChunk.chunkData.data.data.Length);
            _ms.Position = 0;
            IMTBCompress compress = MTBCompressFactory.GetCompress(netChunk.chunkData.data.compressType);
            _ms = (MemoryStream)compress.Decompress(_ms);
            _ms.Position = 0;
            chunk.Deserialize(_ms);
            return chunk;
        }

        public NetChunkData GetNetChunkData(NetChunk netChunk)
        {
            Chunk chunk = netChunk.chunk;
            _ms.SetLength(0);
            chunk.Serialize(_ms);
            _ms.Position = 0;
            IMTBCompress compress = MTBCompressFactory.GetCompress(MTBCompressType.ZLib);
            _ms = (MemoryStream)compress.Encompress(_ms);
            _ms.Position = 0;
            netChunk.chunkData.data.data = _ms.ToArray();
            netChunk.chunkData.data.compressType = MTBCompressType.ZLib;
            return netChunk.chunkData;
        }

        public NetChunkData GetNetChunkData(Chunk chunk, int roleId)
        {
            _ms.SetLength(0);
            chunk.Serialize(_ms);
            _ms.Position = 0;
            IMTBCompress compress = MTBCompressFactory.GetCompress(MTBCompressType.ZLib);
            _ms = (MemoryStream)compress.Encompress(_ms);
            _ms.Position = 0;
            NetChunkData netChunkData = new NetChunkData(roleId, chunk.worldPos);
            netChunkData.data.data = _ms.ToArray();
            netChunkData.data.compressType = MTBCompressType.ZLib;
            return netChunkData;
        }

        public void SaveNetChunk(NetChunkData netChunkData)
        {
            WorldPos regionPos = GetRegionPos(netChunkData.worldPos);
            RegionFile regionFile = GetNetRegionFile(regionPos);
            WorldPos regionFileChunkPos = GetRegionFileChunkPos(netChunkData.worldPos, regionPos);
            regionFile.SaveChunkData(regionFileChunkPos.x, regionFileChunkPos.z, netChunkData.data);
        }

        public bool LoadNetChunk(NetChunkData netChunkData)
        {
            WorldPos regionPos = GetRegionPos(netChunkData.worldPos);
            RegionFile regionFile = GetNetRegionFile(regionPos);
            WorldPos regionFileChunkPos = GetRegionFileChunkPos(netChunkData.worldPos, regionPos);
            bool succ = regionFile.GetChunkData(regionFileChunkPos.x, regionFileChunkPos.z, netChunkData.data);
            return succ;
        }

        public void SaveChunk(Chunk chunk)
        {
            WorldPos regionPos = GetRegionPos(chunk.worldPos);
            RegionFile regionFile = GetRegionFile(regionPos);
            WorldPos regionFileChunkPos = GetRegionFileChunkPos(chunk.worldPos, regionPos);
            regionFile.SaveChunkData(regionFileChunkPos.x, regionFileChunkPos.z, chunk);
        }

        public bool LoadChunk(Chunk chunk)
        {
            WorldPos regionPos = GetRegionPos(chunk.worldPos);
            RegionFile regionFile = GetRegionFile(regionPos);
            WorldPos regionFileChunkPos = GetRegionFileChunkPos(chunk.worldPos, regionPos);
            bool succ = regionFile.GetChunkData(regionFileChunkPos.x, regionFileChunkPos.z, chunk);
            return succ;
        }

        public void UpdateRegionFileLinkByChunkPos(WorldPos chunkPos)
        {
            WorldPos curRegionPos = GetRegionPos(chunkPos);
            List<WorldPos> removeList = new List<WorldPos>();
            foreach (var regionPos in _map.Keys)
            {
                if (Math.Abs(regionPos.x - curRegionPos.x) > 1 || Math.Abs(regionPos.z - curRegionPos.z) > 1)
                {
                    removeList.Add(regionPos);
                }
            }
            for (int i = 0; i < removeList.Count; i++)
            {
                _map[removeList[i]].Close();
                _map.Remove(removeList[i]);
            }
        }

        private WorldPos GetRegionPos(WorldPos worldPos)
        {
            int x = Mathf.FloorToInt((float)worldPos.x / (Chunk.chunkWidth * RegionFile.REGION_WIDTH));
            int z = Mathf.FloorToInt((float)worldPos.z / (Chunk.chunkDepth * RegionFile.REGION_DEPTH));
            return new WorldPos(x, 0, z);
        }


        private WorldPos GetRegionFileChunkPos(WorldPos worldPos, WorldPos regionPos)
        {
            int x = Mathf.FloorToInt((float)worldPos.x / Chunk.chunkWidth) - regionPos.x * RegionFile.REGION_WIDTH;
            int z = Mathf.FloorToInt((float)worldPos.z / Chunk.chunkDepth) - regionPos.z * RegionFile.REGION_DEPTH;
            return new WorldPos(x, 0, z);
        }

        private RegionFile GetRegionFile(WorldPos worldPos)
        {
            RegionFile regionFile = null;
            _map.TryGetValue(worldPos, out regionFile);
            if (regionFile != null) return regionFile;
            string regionFileName = GetRegionFileName(worldPos);
            //			Debug.Log("FullName:" + regionFileName + " worldPos:x=" + worldPos.x + ",z=" + worldPos.z);
            RegionFile file = new RegionFile(regionFileName, MTBCompressType.ZLib);
            file.Init();
            _map.Add(worldPos, file);
            return file;
        }

        private RegionFile GetNetRegionFile(WorldPos worldPos)
        {
            RegionFile regionFile = null;
            _map.TryGetValue(worldPos, out regionFile);
            if (regionFile != null) return regionFile;
            else _netMap.TryGetValue(worldPos, out regionFile);
            if (regionFile != null) return regionFile;
            string regionFileName = GetRegionFileName(worldPos);
            RegionFile file = new RegionFile(regionFileName, MTBCompressType.ZLib);
            file.Init();
            _netMap.Add(worldPos, file);
            return file;
        }

        public void Dispose()
        {
            foreach (var item in _map)
            {
                item.Value.Close();
            }
        }

        public string GetRegionFileName(WorldPos worldPos)
        {
            string fileName = "r_" + worldPos.x + "_" + worldPos.z + ".mtb";
            return WorldConfig.Instance.savedPath + "/" + fileName;
        }
    }

    public class WorldFileInfo
    {
        public string worldConfigStr { get; private set; }
        public string worldName { get; private set; }
        public string lastSaveTimeStr { get; private set; }
        public int seed { get; private set; }
        public WorldFileInfo(string worldConfigStr, string worldName, string lastSaveTimeStr, int seed)
        {
            this.worldConfigStr = worldConfigStr;
            this.worldName = worldName;
            this.lastSaveTimeStr = lastSaveTimeStr;
            this.seed = seed;
        }
    }
}

