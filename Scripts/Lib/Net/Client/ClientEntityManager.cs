using System;
using System.Collections.Generic;
namespace MTB
{
	public class ClientEntityManager
	{
		private Dictionary<int,ClientEntity> _entityMap;
		private Dictionary<WorldPos,List<ClientEntity>> _entityChunkMap;
		public ClientEntityManager ()
		{
			_entityMap = new Dictionary<int, ClientEntity>();
			_entityChunkMap = new Dictionary<WorldPos, List<ClientEntity>>(new WorldPosComparer());
		}

		public void InitEntity(ClientEntityInfo info)
		{
			ClientEntity entity = new ClientEntity(info);
			AddEntity(entity);
		}

		private void AddEntity(ClientEntity entity)
		{
			lock(_entityMap)
			{
				_entityMap.Add(entity.aoId,entity);
			}
			lock(_entityChunkMap)
			{
				WorldPos chunkPos = entity.inChunkPos;
				List<ClientEntity> listMonster;
				_entityChunkMap.TryGetValue(chunkPos,out listMonster);
				if(listMonster == null)
				{
					listMonster = new List<ClientEntity>();
					_entityChunkMap.Add(chunkPos,listMonster);
				}
				listMonster.Add(entity);
			}
		}
		
		public void RemoveEntity(ClientEntity entity)
		{
			lock(_entityMap)
			{
				_entityMap.Remove(entity.aoId);
			}
			lock(_entityChunkMap)
			{
				WorldPos chunkPos = entity.inChunkPos;
				List<ClientEntity> listEntity;
				_entityChunkMap.TryGetValue(chunkPos,out listEntity);
				if(listEntity != null)
				{
					listEntity.Remove(entity);
					if(listEntity.Count <= 0)_entityChunkMap.Remove(chunkPos);
				}
			}
		}
		public void RemoveEntitiesInChunk(WorldPos chunkPos)
		{
			lock(_entityChunkMap)
			{
				_entityChunkMap.Remove(chunkPos);
			}
		}
		
		public void RemoveEntityById(int aoId)
		{
			ClientEntity entity;
			lock(_entityMap)
			{
				_entityMap.TryGetValue(aoId,out entity);
				if(entity == null)return;
			}
			RemoveEntity(entity);
		}
		
		public void UpdateEntityChunkPos(WorldPos oldPos,WorldPos newPos,ClientEntity entity)
		{
			lock(_entityChunkMap)
			{
				List<ClientEntity> listEntity;
				_entityChunkMap.TryGetValue(oldPos,out listEntity);
				if(listEntity != null)
				{
					listEntity.Remove(entity);
					if(listEntity.Count <= 0)_entityChunkMap.Remove(oldPos);
				}
				List<ClientEntity> newPosListEntity;
				_entityChunkMap.TryGetValue(newPos,out newPosListEntity);
				if(newPosListEntity == null)
				{
					newPosListEntity = new List<ClientEntity>();
					_entityChunkMap.Add(newPos,newPosListEntity);
				}
				newPosListEntity.Add(entity);
			}
		}
		
		public ClientEntity GetEntity(int aoId)
		{
			lock(_entityMap)
			{
				ClientEntity entity;
				_entityMap.TryGetValue(aoId,out entity);
				return entity;
			}
		}

		public List<ClientEntity> GetEntitiesInChunk(WorldPos chunkPos)
		{
			lock(_entityChunkMap)
			{
				List<ClientEntity> listEntities;
				_entityChunkMap.TryGetValue(chunkPos,out listEntities);
				return listEntities;
			}
		}

		public List<int> GetAroundEntity(WorldPos curChunkPos,int width)
		{
			List<int> list = new List<int>();
			WorldPos pos = curChunkPos;
			for (int x = -width; x <= width; x++) {
				for (int z = -width; z <= width; z++) {
					WorldPos chunkPos = new WorldPos(pos.x + x * Chunk.chunkWidth,pos.y,pos.z + z * Chunk.chunkDepth);
					List<ClientEntity> monsterList = GetEntitiesInChunk(chunkPos);
					if(monsterList != null)
					{
						for (int i = 0; i < monsterList.Count; i++) {
							list.Add(monsterList[i].aoId);
						}
					}
				}
			}
			return list;
		}

		public void BroadcastPackageInEntityViewPlayers(NetPackage package,ClientEntity clientEntity,int besidePlayerId)
		{
			for (int i = 0; i < clientEntity.viewPlayers.Count; i++) {
				int id = clientEntity.viewPlayers[i];
				if(id != besidePlayerId)
				{
					ClientPlayer player = NetManager.Instance.server.playerManager.GetPlayer(id);
					player.worker.SendPackage (package);
				}
			}
		}

		public void Dispose()
		{
			_entityMap.Clear();
			_entityChunkMap.Clear();
		}
	}
}

