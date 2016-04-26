/***
 * 编辑器模式使用的worldLoader
 * **/
using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
namespace MTB
{
    public class EditorWorldLoader : IWorldLoader
    {
        #region IWorldLoader implementation

        public void Stop()
        {
        }

        public void Start()
        {
        }

        #endregion

        private GameObject player;

        private World world;

        public WorldPos _curChunkPos;

        private Queue<WorldPos> loadQueue;

        public EditorWorldLoader(World world)
        {
            this.world = world;
            loadQueue = new Queue<WorldPos>(500);
            _curChunkPos = new WorldPos(int.MaxValue, 0, 0);
            EventManager.RegisterEvent(EventMacro.CHUNK_GENERATE_FINISH, OnChunkGenerateFinish);
        }

        public void Dispose()
        {
            EventManager.UnRegisterEvent(EventMacro.CHUNK_GENERATE_FINISH, OnChunkGenerateFinish);
            if (WorldConfig.Instance.saveBlock)
            {
                SaveAll();
            }
        }

        public void SaveAll()
        {
            foreach (var chunk in world.chunks.Values)
            {
                if (chunk.isUpdate)
                {
                    WorldPersistanceManager.Instance.SaveChunk(chunk);
                }
            }
        }

        List<WorldPos> firstChunks = new List<WorldPos>();
        public void LoadFirst(Vector3 pos, int size)
        {
            size = 4;
            EventManager.SendEvent(EventMacro.START_GENERATE_FIRST_WORLD, null);
            EventManager.RegisterEvent(EventMacro.CHUNK_GENERATE_FINISH, OnFirstWorldChunkGenerate);
            WorldPos worldPos = Terrain.GetWorldPos(pos);
            WorldPos curChunkPos = Terrain.GetChunkPos(worldPos.x, worldPos.y, worldPos.z);
            for (int x = -size; x <= size; x++)
            {
                for (int z = -size; z <= size; z++)
                {
                    WorldPos chunkPos = new WorldPos(curChunkPos.x + x * Chunk.chunkWidth, curChunkPos.y, curChunkPos.z + z * Chunk.chunkDepth);
                    if ((x > -size + 1 && x < size - 1) && (z > -size + 1 && z < size - 1))
                    {
                        firstChunks.Add(chunkPos);
                    }
                    world.WorldGenerator.GenerateChunk(chunkPos.x, chunkPos.y, chunkPos.z, curChunkPos);
                }
            }
        }

        private void OnFirstWorldChunkGenerate(object[] param)
        {
            Chunk chunk = param[0] as Chunk;
            int index = firstChunks.IndexOf(chunk.worldPos);
            if (index != -1)
            {
                firstChunks.RemoveAt(index);
                if (firstChunks.Count == 0)
                {
                    EventManager.UnRegisterEvent(EventMacro.CHUNK_GENERATE_FINISH, OnFirstWorldChunkGenerate);
                    EventManager.SendEvent(EventMacro.GENERATE_FIRST_WORLD_FINISH, null);
                }
            }
        }

        private void OnChunkGenerateFinish(object[] param)
        {
            Chunk chunk = param[0] as Chunk;

            chunk.RefreshEntity();
        }

        private float entityPowWidth = WorldConfig.Instance.playerViewWidth * WorldConfig.Instance.playerViewWidth;
        public void CheckEntityVisible(Chunk chunk)
        {
            WorldPos pos = chunk.worldPos;
            float xDis = (_curChunkPos.x - pos.x) / Chunk.chunkWidth;
            float yDis = (_curChunkPos.z - pos.z) / Chunk.chunkDepth;
            float dis = xDis * xDis + yDis * yDis;
        }

        public void ChangeAllEntityVisible()
        {
            var enumerator = world.chunks.GetEnumerator();
            while (enumerator.MoveNext())
            {
                CheckEntityVisible(enumerator.Current.Value);
            }
        }

        public void Update()
        {
            if (player == null)
            {
                player = HasActionObjectManager.Instance.playerManager.getMyPlayer();
            }
            else
            {
                WorldPos playerPos = Terrain.GetWorldPos(player.transform.position);
                WorldPos curChunkPos = Terrain.GetChunkPos(playerPos.x, 0, playerPos.z);
                if (!curChunkPos.EqualOther(_curChunkPos))
                {
                    _curChunkPos = curChunkPos;
                    world.WorldGenerator.DataProcessorManager.UpdateChunkPriority(_curChunkPos);
                    ChangeAllEntityVisible();
                    LoadChunks();
                    RemoveChunks();
                    UpdatePersistanceOperate();
                }
            }
        }

        public void UpdatePersistanceOperate()
        {
            WorldPos playerPos = Terrain.GetWorldPos(player.transform.position);
            WorldPersistanceManager.Instance.UpdateRegionFileLinkByChunkPos(_curChunkPos);
        }

        private float loadPowWidth = WorldConfig.Instance.extendChunkWidth * Chunk.chunkWidth * WorldConfig.Instance.extendChunkWidth * Chunk.chunkWidth;
        public void LoadChunks()
        {
            int xWidth = WorldConfig.Instance.extendChunkWidth * Chunk.chunkWidth;
            int zWidth = WorldConfig.Instance.extendChunkWidth * Chunk.chunkDepth;
            for (int x = -xWidth; x <= xWidth; x += Chunk.chunkWidth)
            {
                for (int z = -zWidth; z <= zWidth; z += Chunk.chunkDepth)
                {
                    WorldPos chunkPos = new WorldPos(_curChunkPos.x + x, _curChunkPos.y, _curChunkPos.z + z);
                    if (!world.chunks.ContainsKey(chunkPos))
                    {
                        double dis = x * x + z * z;
                        if (dis <= loadPowWidth)
                            loadQueue.Enqueue(chunkPos);
                    }
                }
            }

            while (loadQueue.Count > 0)
            {
                WorldPos pos = loadQueue.Dequeue();
                if (!WorldConfig.Instance.hasBorder || WorldConfig.Instance.hasBorder && pos.x > WorldConfig.Instance.borderLeft &&
                   pos.x < WorldConfig.Instance.borderRight && pos.z > WorldConfig.Instance.borderBack &&
                   pos.z < WorldConfig.Instance.borderFront)
                {
                    world.WorldGenerator.GenerateChunk(pos.x, pos.y, pos.z, _curChunkPos);
                }
            }
        }

        private List<Chunk> deleteChunkList = new List<Chunk>(500);
        private float deletePowWidth = (WorldConfig.Instance.extendChunkWidth + 0.5f) * (WorldConfig.Instance.extendChunkWidth + 0.5f);
        public void RemoveChunks()
        {
            deleteChunkList.Clear();

            var enumerator = world.chunks.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var key = enumerator.Current;
                var item = key.Key;
                float xDis = (item.x - _curChunkPos.x) / Chunk.chunkWidth;
                float zDis = (item.z - _curChunkPos.z) / Chunk.chunkDepth;
                float dis = xDis * xDis + zDis * zDis;
                if (dis > deletePowWidth)
                {
                    deleteChunkList.Add(key.Value);
                }
            }

            for (int i = 0; i < deleteChunkList.Count; i++)
            {
                if (deleteChunkList[i].isTerrainDataPrepared)
                    world.WorldGenerator.RemoveChunk(deleteChunkList[i]);
            }
        }

        public void updateArea(RefreshChunkArea area)
        {
            World.world.WorldGenerator.RefreshChunk(area);
        }

    }
}
