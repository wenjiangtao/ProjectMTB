using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MTB
{
	public class ClientPlayerManager
	{
		private Dictionary<int,ClientPlayer> _playerMap;
		private Dictionary<WorldPos,List<ClientPlayer>> _playerChunkMap;
		public ClientPlayer mainPlayer{get;private set;}
		public ClientPlayerManager ()
		{
			_playerMap = new Dictionary<int, ClientPlayer>();
			_playerChunkMap = new Dictionary<WorldPos, List<ClientPlayer>>(new WorldPosComparer());
		}

		public void AddPlayer(ClientPlayer player,bool isMainPlayer = false)
		{
			if(isMainPlayer)mainPlayer = player;
			lock(_playerMap)
			{
				_playerMap.Add(player.id,player);
			}
			lock(_playerChunkMap)
			{
				WorldPos chunkPos = player.inChunkPos;
				List<ClientPlayer> listPlayer;
				_playerChunkMap.TryGetValue(chunkPos,out listPlayer);
				if(listPlayer == null)
				{
					listPlayer = new List<ClientPlayer>();
					_playerChunkMap.Add(chunkPos,listPlayer);
				}
				listPlayer.Add(player);
			}
		}
		
		public void RemovePlayer(ClientPlayer player)
		{
			lock(_playerMap)
			{
				_playerMap.Remove(player.id);
			}
			lock(_playerChunkMap)
			{
				WorldPos chunkPos = player.inChunkPos;
				List<ClientPlayer> listPlayer;
				_playerChunkMap.TryGetValue(chunkPos,out listPlayer);
				if(listPlayer != null)
				{
					listPlayer.Remove(player);
					if(listPlayer.Count <= 0)_playerChunkMap.Remove(chunkPos);
				}
			}
		}
		
		public void RemovePlayerById(int id)
		{
			ClientPlayer player;
			lock(_playerMap)
			{
				_playerMap.TryGetValue(id,out player);
				if(player == null)return;
			}
			RemovePlayer(player);
		}

		public void UpdatePlayerChunkPos(WorldPos oldPos,WorldPos newPos,ClientPlayer player)
		{
			lock(_playerChunkMap)
			{
				List<ClientPlayer> listPlayer;
				_playerChunkMap.TryGetValue(oldPos,out listPlayer);
				if(listPlayer != null)
				{
					listPlayer.Remove(player);
					if(listPlayer.Count <= 0)_playerChunkMap.Remove(oldPos);
				}
				List<ClientPlayer> newPosListPlayer;
				_playerChunkMap.TryGetValue(newPos,out newPosListPlayer);
				if(newPosListPlayer == null)
				{
					newPosListPlayer = new List<ClientPlayer>();
					_playerChunkMap.Add(newPos,newPosListPlayer);
				}
				newPosListPlayer.Add(player);
//				Debug.Log("玩家id:"+player.id + "更新区块位置--oldPos:" + oldPos.ToString() + " newPos:"+newPos.ToString());
			}
		}
		
		public ClientPlayer GetPlayer(int id)
		{
			lock(_playerMap)
			{
				ClientPlayer player;
				_playerMap.TryGetValue(id,out player);
				return player;
			}
		}

		public List<ClientPlayer> GetPlayersInChunk(WorldPos chunkPos)
		{
			lock(_playerChunkMap)
			{
				List<ClientPlayer> listPlayer;
				_playerChunkMap.TryGetValue(chunkPos,out listPlayer);
				return listPlayer;
			}
		}

		public List<int> GetAroundPlayer(ClientPlayer curPlayer,int width,bool includeSelf = true)
		{
			List<int> list = new List<int>();
			WorldPos pos = curPlayer.inChunkPos;
			for (int x = -width; x <= width; x++) {
				for (int z = -width; z <= width; z++) {
					WorldPos chunkPos = new WorldPos(pos.x + x * Chunk.chunkWidth,pos.y,pos.z + z * Chunk.chunkDepth);
					List<ClientPlayer> playerList = GetPlayersInChunk(chunkPos);
					if(playerList != null)
					{
						for (int i = 0; i < playerList.Count; i++) {
							if(!includeSelf && curPlayer.id == playerList[i].id)continue;
							list.Add(playerList[i].id);
						}
					}
				}
			}
			return list;
		}

		public List<int> GetAroundPlayer(WorldPos curChunkPos,int width)
		{
			List<int> list = new List<int>();
			WorldPos pos = curChunkPos;
			for (int x = -width; x <= width; x++) {
				for (int z = -width; z <= width; z++) {
					WorldPos chunkPos = new WorldPos(pos.x + x * Chunk.chunkWidth,pos.y,pos.z + z * Chunk.chunkDepth);
					List<ClientPlayer> playerList = GetPlayersInChunk(chunkPos);
					if(playerList != null)
					{
						for (int i = 0; i < playerList.Count; i++) {
							list.Add(playerList[i].id);
						}
					}
				}
			}
			return list;
		}

		public List<ClientPlayer> GetAllPlayers()
		{
			List<ClientPlayer> list = new List<ClientPlayer>();
			foreach (var item in _playerMap) {
				list.Add(item.Value);
			}
			return list;
		}

		//广播给所有人
		public void BroadcastPackage(NetPackage pacakge)
		{
			foreach (var item in _playerMap) {
				item.Value.worker.SendPackage(pacakge);
			}
		}

		//广播给所有人
		public void BroadcastPackage(NetPackage pacakge,ClientPlayer broadcastSource,bool includeSelf = true)
		{
			foreach (var item in _playerMap) {
				if(!includeSelf && broadcastSource.id == item.Value.id)continue;
				item.Value.worker.SendPackage(pacakge);
			}
		}


		public void BroadcastChunkPackage(NetPackage package,ClientPlayer broadcastSource,List<WorldPos> chunkPosList,bool includeSelf)
		{
			for (int i = 0; i < chunkPosList.Count; i++) {
				List<ClientPlayer> listPlayer = GetPlayersInChunk(chunkPosList[i]);
				if(listPlayer != null)
				{
					for (int j = 0; j < listPlayer.Count; j++) {
						if(!includeSelf && listPlayer[j].id == broadcastSource.id)continue;
						listPlayer[j].worker.SendPackage(package);
					}
				}
			}
		}

		public void Dispose()
		{
			_playerMap.Clear();
			_playerChunkMap.Clear();
			mainPlayer = null;
		}
	}
}

