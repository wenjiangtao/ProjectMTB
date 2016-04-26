using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
namespace MTB
{
    public class NetWorldLoader : IWorldLoader
    {

        private World world;
        private GameObject player;
        public WorldPos _curChunkPos;
        private HeapPriorityQueue<PriorityWorldPos> loadQueue;
		private Queue<WorldPos> entityRefreshQueue;
		private Dictionary<WorldPos,Chunk> _removedChunks;
		private Dictionary<WorldPos,Chunk> _savedChunks;
        private bool _stop;
        public NetWorldLoader(World world)
        {
            this.world = world;
            _stop = true;
            loadQueue = new HeapPriorityQueue<PriorityWorldPos>(1000);
			entityRefreshQueue = new Queue<WorldPos>(200);
			_removedChunks = new Dictionary<WorldPos, Chunk>(new WorldPosComparer());
			_savedChunks = new Dictionary<WorldPos, Chunk>(new WorldPosComparer());
            _curChunkPos = new WorldPos(int.MaxValue, 0, 0);
            EventManager.RegisterEvent(NetEventMacro.ON_NET_CHUNK_GENERATOR_RETURN, OnNetChunkGeneratorReturn);
            EventManager.RegisterEvent(NetEventMacro.ON_NET_CHUNK_GENERATOR, OnNetChunkGenerator);
            EventManager.RegisterEvent(NetEventMacro.ON_NET_SAVE_CHUNK, OnNetSaveChunk);
            EventManager.RegisterEvent(NetEventMacro.ON_NET_POPULATION_GENERATE, OnNetPopulationGenerate);
            EventManager.RegisterEvent(NetEventMacro.ON_NET_AREA_UPDATE, OnNetAreaUpdate);
			EventManager.RegisterEvent(NetEventMacro.ON_RESPONSE_REMOVE_CHUNK_EXT_DATA,OnResponseRemoveChunkExtData);

			EventManager.RegisterEvent(EventMacro.CHUNK_GENERATE_FINISH, OnChunkGenerateFinish);
            world.StartCoroutine(ResponseNetChunk());
			world.StartCoroutine(RefreshEntity());
        }

        public void Stop()
        {
            _stop = true;
        }

        public void Start()
        {
            _stop = false;
        }

        private void OnNetPopulationGenerate(object[] param)
        {
            NetChunkData data = (NetChunkData)param[0];
            world.WorldGenerator.UpdatePopulationFromServer(data, _curChunkPos);
        }

        private void OnNetSaveChunk(object[] param)
        {
            NetChunkData data = (NetChunkData)param[0];
            world.WorldGenerator.DataProcessorManager.EnqueueSaveNetChunkData(data);
        }

        private void OnNetChunkGenerator(object[] param)
        {
            NetChunkData data = (NetChunkData)param[0];
            world.WorldGenerator.LoadNetChunkData(data);
        }

        private void OnNetAreaUpdate(object[] param)
        {
            RefreshChunkArea area = (RefreshChunkArea)param[0];
            world.WorldGenerator.UpdateAreaFromServer(area);
        }

        private IEnumerator ResponseNetChunk()
        {
            while (true)
            {
                NetChunkData netChunkData = world.WorldGenerator.DataProcessorManager.DeResponseNetChunkData();
                if (netChunkData != null)
                {
                    ResponseChunkPackage package = PackageFactory.GetPackage(PackageType.ResponseChunk) as ResponseChunkPackage;
                    package.roleId = netChunkData.roleId;
                    package.pos = netChunkData.worldPos;
                    package.isExit = netChunkData.isExist;
                    package.compressType = (byte)netChunkData.data.compressType;
                    package.chunkByteData = netChunkData.data.data;
                    NetManager.Instance.client.SendPackage(package);
                }
                yield return null;
            }
        }

        private void OnNetChunkGeneratorReturn(object[] param)
        {
            NetChunkData netChunkData = (NetChunkData)param[0];
            world.WorldGenerator.GenerateNetChunk(netChunkData, _curChunkPos);
        }

        List<WorldPos> firstChunks = new List<WorldPos>();
        public void LoadFirst(Vector3 pos, int size)
        {
            EventManager.SendEvent(EventMacro.START_GENERATE_FIRST_WORLD, null);
            EventManager.RegisterEvent(EventMacro.CHUNK_GENERATE_FINISH, OnFirstWorldChunkGenerate);
            WorldPos worldPos = Terrain.GetWorldPos(pos);
            WorldPos curChunkPos = Terrain.GetChunkPos(worldPos.x, 0, worldPos.z);
            for (int x = -size; x <= size; x++)
            {
                for (int z = -size; z <= size; z++)
                {
                    WorldPos chunkPos = new WorldPos(curChunkPos.x + x * Chunk.chunkWidth, curChunkPos.y, curChunkPos.z + z * Chunk.chunkDepth);
                    if ((x > -size + 1 && x < size - 1) && (z > -size + 1 && z < size - 1))
                    {
                        firstChunks.Add(chunkPos);
                    }
                    world.WorldGenerator.GenerateFromServer(chunkPos.x, chunkPos.y, chunkPos.z);
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

		private IEnumerator RefreshEntity()
		{
			while(true)
			{
				if(world.FirstGeneted)
				{
					if(entityRefreshQueue.Count > 0)
					{
						WorldPos refreshPos = entityRefreshQueue.Dequeue();
						//真正要刷的时候再次判断是否在玩家视野
						if(IsInPlayerView(refreshPos))
						{
							Chunk chunk = world.GetChunk(refreshPos.x,refreshPos.y,refreshPos.z);
							if(chunk != null && chunk.isGenerated && !chunk.hasRefreshEntities)
							{
								//向服务器发送刷新怪物请求
								List<EntityData> list = chunk.ClearEntityAndSetRefresh();
								RequestRefreshChunkEntitiesPackage package = PackageFactory.GetPackage(PackageType.RequestRefreshChunkEntities)
									as RequestRefreshChunkEntitiesPackage;
								package.entities = list;
								package.pos = chunk.worldPos;
								NetManager.Instance.client.SendPackage(package);
							}
						}
					}
				}
				yield return null;
			}
		}
		
		private void OnChunkGenerateFinish(object[] param)
		{
			Chunk chunk = param[0] as Chunk;
			CheckEntityRefresh(chunk);
		}

		private void OnResponseRemoveChunkExtData(object[] param)
		{
			WorldPos pos = (WorldPos)param[0];
			bool needSave = (bool)param[1];
			List<ClientEntityInfo> listEntityInfo = (List<ClientEntityInfo>)param[2];
			Chunk chunk;
			_removedChunks.TryGetValue(pos,out chunk);
			if(chunk != null)
			{
				_removedChunks.Remove(pos);
				NetRemovedChunk removedChunk = new NetRemovedChunk(chunk);
				removedChunk.needSave = needSave;
				removedChunk.changedEntityInfos = listEntityInfo;
				world.WorldGenerator.RemoveChunkFromClient(removedChunk);
				Debug.Log("保存位置为:" + pos + "的数据!");
			}else
			{
				chunk = null;
				_savedChunks.TryGetValue(pos,out chunk);
				if(chunk != null)
				{
					_savedChunks.Remove(pos);
					NetRemovedChunk savedChunk = new NetRemovedChunk(chunk);
					savedChunk.needSave = needSave;
					savedChunk.changedEntityInfos = listEntityInfo;
					EventManager.SendEvent(EventMacro.CHUNK_DATA_SAVE, savedChunk);
					if(_savedChunks.Count <= 0)
					{
						Debug.Log("保存数据完毕!");
					}
				}
			}
		}
		
		private bool IsInPlayerView(WorldPos pos)
		{
			int width = Mathf.Max(Mathf.Abs(_curChunkPos.x - pos.x) / Chunk.chunkWidth,Mathf.Abs(_curChunkPos.z - pos.z) / Chunk.chunkDepth);
			return width <= WorldConfig.Instance.playerViewWidth;
		}
		
		public void CheckEntityRefresh(Chunk chunk)
		{
			WorldPos pos = chunk.worldPos;
			if(IsInPlayerView(pos))
			{
				if(!chunk.hasRefreshEntities && !entityRefreshQueue.Contains(pos))
				{
					entityRefreshQueue.Enqueue(pos);
				}
			}
		}
		
		public void CheckAllChunkRefresh()
		{
			foreach (var item in world.chunks) {
				if(item.Value.isGenerated)
				{
					CheckEntityRefresh(item.Value);
				}
			}
		}

        public void Dispose()
        {
//			foreach (var chunk in world.chunks.Values)
//            {
//				NetRemovedChunk removedChunk = new NetRemovedChunk(chunk);
//				removedChunk.needSave = true;
//				EventManager.SendEvent(EventMacro.CHUNK_DATA_REMOVE_FINISH, removedChunk);
//            }

//			Debug.Log("保存所有数据中...！");
//			Queue<Chunk> removedChunkQueue = new Queue<Chunk>(world.chunks.Values);
//			while(removedChunkQueue.Count > 0)
//			{
//				RemoveChunkInLocalAndRequestExtData(removedChunkQueue.Dequeue());
//			}
			world.StopCoroutine(ResponseNetChunk());
			world.StopCoroutine(RefreshEntity());
			EventManager.UnRegisterEvent(NetEventMacro.ON_NET_CHUNK_GENERATOR_RETURN, OnNetChunkGeneratorReturn);
			EventManager.UnRegisterEvent(NetEventMacro.ON_NET_CHUNK_GENERATOR, OnNetChunkGenerator);
			EventManager.UnRegisterEvent(EventMacro.CHUNK_GENERATE_FINISH, OnFirstWorldChunkGenerate);
			EventManager.UnRegisterEvent(NetEventMacro.ON_NET_SAVE_CHUNK, OnNetSaveChunk);
			EventManager.UnRegisterEvent(NetEventMacro.ON_NET_POPULATION_GENERATE, OnNetPopulationGenerate);
			EventManager.UnRegisterEvent(NetEventMacro.ON_RESPONSE_REMOVE_CHUNK_EXT_DATA,OnResponseRemoveChunkExtData);
			
			EventManager.UnRegisterEvent(EventMacro.CHUNK_GENERATE_FINISH, OnChunkGenerateFinish);
			NetManager.Instance.client.Dispose();
			NetManager.Instance.server.Dispose();

        }

        public void SaveAll()
        {
			Debug.Log("保存所有数据中...！");
            foreach (var chunk in world.chunks.Values)
            {
				RemoveChunkInLocalAndRequestExtData(chunk,true);
//                EventManager.SendEvent(EventMacro.CHUNK_DATA_SAVE, chunk);
            }
//			Debug.Log("保存所有数据中...！");
//			Queue<Chunk> removedChunkQueue = new Queue<Chunk>(world.chunks.Values);
//			while(removedChunkQueue.Count > 0)
//			{
//				RemoveChunkInLocalAndRequestExtData(removedChunkQueue.Dequeue(),true);
//			}
        }

        public void Update()
        {
            if (!_stop)
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
                        world.WorldGenerator.DataProcessorManager.UpdateNetChunkPriority(_curChunkPos);
						CheckAllChunkRefresh();
						LoadChunks();
                        RemoveChunks();
                    }
                }
            }
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
                        {
                            PriorityWorldPos priorityWorldPos = GetPriorityPos(chunkPos);
                            loadQueue.Enqueue(priorityWorldPos, priorityWorldPos.Priority);
                        }
                    }
                }
            }

            while (loadQueue.Count > 0)
            {
                PriorityWorldPos priorityPos = loadQueue.Dequeue();
                WorldPos pos = priorityPos.pos;
                world.WorldGenerator.GenerateFromServer(pos.x, pos.y, pos.z);
            }
        }

        private PriorityWorldPos GetPriorityPos(WorldPos pos)
        {
            PriorityWorldPos priorityPos = new PriorityWorldPos(pos, GetPriority(pos, _curChunkPos));
            return priorityPos;
        }

        public double GetPriority(WorldPos chunkPos, WorldPos curInChunkPos)
        {
            double priority = Math.Sqrt(Math.Pow((chunkPos.x - curInChunkPos.x) / Chunk.chunkWidth, 2) +
                                        Math.Pow((chunkPos.z - curInChunkPos.z) / Chunk.chunkDepth, 2));
            return priority;
        }

        private List<Chunk> deleteChunkList = new List<Chunk>(1000);
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
				{
					RemoveChunkInLocalAndRequestExtData(deleteChunkList[i]);
				}
            }
        }

		public void RemoveChunkInLocalAndRequestExtData(Chunk chunk,bool isSave = false)
		{
			if(isSave)
			{
				if(!_savedChunks.ContainsKey(chunk.worldPos))
				{
					_savedChunks.Add(chunk.worldPos,chunk);
				}
			}
			else
			{
				world.chunks.Remove(chunk.worldPos);
				if(!_removedChunks.ContainsKey(chunk.worldPos))
				{
					_removedChunks.Add(chunk.worldPos,chunk);
				}
			}
			RequestRemoveChunkExtDataPackage package = PackageFactory.GetPackage(PackageType.RequestRemoveChunkExtData)
				as RequestRemoveChunkExtDataPackage;
			package.pos = chunk.worldPos;
			NetManager.Instance.client.SendPackage(package);
		}


        public void updateArea(RefreshChunkArea area)
        {
            //同步大块更改数据到服务器
            ClientAreaCollection.Instance.BeginCollection();
            ClientAreaCollection.Instance.Collection(area);
            ClientAreaCollection.Instance.SendPackage();
            ClientAreaCollection.Instance.EndCollection();
        }
    }

    public class PriorityWorldPos : PriorityQueueNode
    {
        public WorldPos pos;

        public PriorityWorldPos(WorldPos pos, double priority)
        {
            this.pos = pos;
            Priority = priority;
        }

        public PriorityWorldPos(WorldPos pos)
        {
            this.pos = pos;
        }

        public override bool Equals(object obj)
        {
            if (obj is WorldPos)
            {
                return pos.EqualOther((WorldPos)obj);
            }
            return false;
        }
    }
}

