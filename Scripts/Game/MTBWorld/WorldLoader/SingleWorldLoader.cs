using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
namespace MTB
{
	public class SingleWorldLoader : IWorldLoader
	{
		private GameObject player;
		
		private World world;
		
		public WorldPos _curChunkPos;
		
		private Queue<WorldPos> loadQueue;
		private Queue<WorldPos> entityRefreshQueue;
		private Queue<WorldPos> entityRemoveQueue;
		private bool _stop;
		
		public SingleWorldLoader (World world)
		{
			this.world = world;
			_stop = true;
			loadQueue = new Queue<WorldPos>(200);
			entityRefreshQueue = new Queue<WorldPos>(200);
			entityRemoveQueue = new Queue<WorldPos>(200);
			//使初始位置与出生位置不一样，第一次加载地图
			_curChunkPos = new WorldPos(int.MaxValue,0,0);
			EventManager.RegisterEvent(EventMacro.CHUNK_GENERATE_FINISH,OnChunkGenerateFinish);
			world.StartCoroutine(RefreshEntity());
		}

		public void Dispose ()
		{
			world.StopCoroutine(RefreshEntity());
			EventManager.UnRegisterEvent(EventMacro.CHUNK_GENERATE_FINISH,OnChunkGenerateFinish);
			EventManager.UnRegisterEvent(EventMacro.CHUNK_GENERATE_FINISH,OnFirstWorldChunkGenerate);
			if(WorldConfig.Instance.saveBlock)
			{
				SaveAll();
			}
		}

		public void Stop()
		{
			_stop = true;
		}

		public void Start()
		{
			_stop = false;
		}

		public void SaveAll()
		{
			foreach (var chunk in world.chunks.Values) {
				if(chunk.ResetEntity() > 0 || chunk.isUpdate)
				{
					WorldPersistanceManager.Instance.SaveChunk(chunk);
				}
			}
		}

		List<WorldPos> firstChunks = new List<WorldPos>();
		public void LoadFirst (Vector3 pos, int size)
		{
			EventManager.SendEvent(EventMacro.START_GENERATE_FIRST_WORLD,null);
			EventManager.RegisterEvent(EventMacro.CHUNK_GENERATE_FINISH,OnFirstWorldChunkGenerate);
			WorldPos worldPos = Terrain.GetWorldPos(pos);
			WorldPos curChunkPos = Terrain.GetChunkPos(worldPos.x,0,worldPos.z);
			for (int x = -size; x <= size; x++) {
				for (int z = -size; z <= size; z++) {
					WorldPos chunkPos = new WorldPos(curChunkPos.x + x * Chunk.chunkWidth,curChunkPos.y,curChunkPos.z + z * Chunk.chunkDepth);
					if((x >= -size + 2 && x <= size - 2) && (z >= -size + 2 && z <= size - 2)){
						firstChunks.Add(chunkPos);
					}
					world.WorldGenerator.GenerateChunk(chunkPos.x,chunkPos.y,chunkPos.z,curChunkPos);
				}
			}
		}

		private void OnFirstWorldChunkGenerate(object[] param)
		{
			Chunk chunk = param[0] as Chunk;
			int index = firstChunks.IndexOf(chunk.worldPos);
			if(index != -1)
			{
				firstChunks.RemoveAt(index);
				if(firstChunks.Count == 0)
				{
					EventManager.UnRegisterEvent(EventMacro.CHUNK_GENERATE_FINISH,OnFirstWorldChunkGenerate);
					EventManager.SendEvent(EventMacro.GENERATE_FIRST_WORLD_FINISH,null);
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
							if(chunk != null && chunk.isGenerated)
							{
								if(chunk.RefreshEntity() > 0)
									yield return new WaitForSeconds(0.1f);
							}
						}
					}
					if(entityRemoveQueue.Count > 0)
					{
						WorldPos removePos = entityRemoveQueue.Dequeue();
						if(!IsInPlayerView(removePos))
						{
							Chunk chunk = world.GetChunk(removePos.x,removePos.y,removePos.z);
							if(chunk != null)
							{
								if(chunk.RemoveEntity() > 0)
									yield return new WaitForSeconds(0.1f);
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
			else
			{
				if(chunk.hasRefreshEntities && !entityRemoveQueue.Contains(pos))
				{
					entityRemoveQueue.Enqueue(pos);
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
		
		
		public void Update()
		{
			if(!_stop)
			{
				if(player == null)
				{
					player = HasActionObjectManager.Instance.playerManager.getMyPlayer();
				}
				else
				{
					WorldPos playerPos = Terrain.GetWorldPos(player.transform.position);
					WorldPos curChunkPos = Terrain.GetChunkPos(playerPos.x,0,playerPos.z);
					if(!curChunkPos.EqualOther(_curChunkPos))
					{
						_curChunkPos = curChunkPos;
						world.WorldGenerator.DataProcessorManager.UpdateChunkPriority(_curChunkPos);
						CheckAllChunkRefresh();
						LoadChunks();
						RemoveChunks();
						UpdatePersistanceOperate();
					}
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
			for (int x = -xWidth; x <= xWidth; x+=Chunk.chunkWidth) {
				for (int z = -zWidth; z <= zWidth; z+=Chunk.chunkDepth) {
					WorldPos chunkPos = new WorldPos(_curChunkPos.x + x,_curChunkPos.y,_curChunkPos.z + z);
					if(!world.chunks.ContainsKey(chunkPos))
					{
						double dis = x * x + z * z;
						if(dis <= loadPowWidth)
							loadQueue.Enqueue(chunkPos);
					}
				}
			}
			
			while(loadQueue.Count > 0)
			{
				WorldPos pos = loadQueue.Dequeue();
				world.WorldGenerator.GenerateChunk(pos.x,pos.y,pos.z,_curChunkPos);
			}
		}
		
		private List<Chunk> deleteChunkList = new List<Chunk>(500);
		private float deletePowWidth = (WorldConfig.Instance.extendChunkWidth + 0.5f) * (WorldConfig.Instance.extendChunkWidth + 0.5f);
		public void RemoveChunks()
		{
			deleteChunkList.Clear();
			
			var enumerator = world.chunks.GetEnumerator();
			while(enumerator.MoveNext())
			{
				var key = enumerator.Current;
				var item = key.Key;
				float xDis = (item.x - _curChunkPos.x) / Chunk.chunkWidth;
				float zDis = (item.z - _curChunkPos.z) / Chunk.chunkDepth;
				float dis = xDis * xDis + zDis * zDis;
				if(dis > deletePowWidth)
				{
					deleteChunkList.Add(key.Value);
				}
			}
			
			for (int i = 0; i < deleteChunkList.Count; i++) {
				if(deleteChunkList[i].isTerrainDataPrepared)
					world.WorldGenerator.RemoveChunk(deleteChunkList[i]);
			}
		}


        public void updateArea(RefreshChunkArea area)
        {
            World.world.WorldGenerator.RefreshChunk(area);
        }
	}
}

