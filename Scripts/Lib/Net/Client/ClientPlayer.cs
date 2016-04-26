using System;
using System.Collections.Generic;
using UnityEngine;
namespace MTB
{
	public class ClientPlayer
	{
		public static int VIEW_WIDTH = 5;
		public ConnectionWorker worker{get;private set;}
		public int id{get;private set;}
		public int aoId{get;set;}
		public int playerId{get;set;}
		public WorldPos inChunkPos{get;private set;}
		private Vector3 _position;
		public Vector3 position{get{
				return _position;
			}}

		public string configName{get;set;}
		public int seed{get;set;}
		public List<WorldPos> chunkPosList{get;private set;}
//		//维护当前玩家chunk是否渲染
//		public List<WorldPos> renderChunkPosList{get;private set;}
		//玩家视野范围
		public int viewWidth{get;private set;}
		public List<int> viewPlayers{get;private set;}
		public List<int> viewEntities{get;private set;}
		public ClientPlayer (int id,ConnectionWorker worker)
		{
			this.id = id;
			this.worker = worker;
			chunkPosList = new List<WorldPos>();
//			renderChunkPosList = new List<WorldPos>();
			viewPlayers = new List<int>();
			viewWidth = VIEW_WIDTH;
		}

		public void InitPosition(Vector3 position)
		{
			_position = position;
			inChunkPos = Terrain.GetChunkPos(position);
			//初始化人物视野
			viewPlayers = NetManager.Instance.server.playerManager.GetAroundPlayer(this,viewWidth,false);
			for (int i = 0; i < viewPlayers.Count; i++) {
				ClientPlayer clientPlayer = NetManager.Instance.server.playerManager.GetPlayer(viewPlayers[i]);
				clientPlayer.AddViewPlayer(id);
			}
			viewEntities = NetManager.Instance.server.entityManager.GetAroundEntity(inChunkPos,viewWidth);
			for (int i = 0; i < viewEntities.Count; i++) {
				ClientEntity clientMonster = NetManager.Instance.server.entityManager.GetEntity(viewEntities[i]);
				clientMonster.AddViewPlayer(id);
			}
		}

		public void AddViewPlayer(int id)
		{
			lock(viewPlayers)
			{
				if(!viewPlayers.Contains(id))
				{
					viewPlayers.Add(id);
				}
			}
		}

		public void RemoveViewPlayer(int id)
		{
			lock(viewPlayers)
			{
				viewPlayers.Remove(id);
			}
		}

		public void AddViewEntity(int aoId)
		{
			lock(viewEntities)
			{
				if(!viewEntities.Contains(aoId))
				{
					viewEntities.Add(aoId);
				}
			}
		}

		public void RemoveViewEntity(int aoId)
		{
			lock(viewEntities)
			{
				viewEntities.Remove(aoId);
			}
		}

		public void UpdatePosition(Vector3 position)
		{
			_position = position;
			WorldPos newPos = Terrain.GetChunkPos(position);
			if(!inChunkPos.EqualOther(newPos))
			{
				WorldPos oldPos = inChunkPos;
				inChunkPos = newPos;
				NetManager.Instance.server.playerManager.UpdatePlayerChunkPos(oldPos,inChunkPos,this);
				//人物移动时更新人物与怪物的视野范围
				UpdateViewPlayer();
				UpdateViewEntity();
			}
		}

		private void UpdateViewPlayer()
		{
			//更新当前人物的视野与其他人物的视野
			List<int> curViewPlayers = NetManager.Instance.server.playerManager.GetAroundPlayer(this,viewWidth,false);
			for (int i = 0; i < curViewPlayers.Count; i++) {
				//如果原先已经包含了该玩家，那么跳过
				if(viewPlayers.Contains(curViewPlayers[i]))
				{
					RemoveViewPlayer(curViewPlayers[i]);
					continue;
				}
				else
				{
					//如果看见其他人了，其他人也能看到自己
					ClientPlayer otherPlayer = NetManager.Instance.server.playerManager.GetPlayer(curViewPlayers[i]);
					otherPlayer.AddViewPlayer(id);
					//通知其他人我进入他的视野
					PlayerJoinViewPackage joinOtherPlayerViewPackage = PackageFactory.GetPackage(PackageType.PlayerJoinView) as PlayerJoinViewPackage;
					joinOtherPlayerViewPackage.playerId = playerId;
					joinOtherPlayerViewPackage.aoId = aoId;
					joinOtherPlayerViewPackage.birthPlace = position;
					otherPlayer.worker.SendPackage(joinOtherPlayerViewPackage);
					//通知我其他人进入我的视野
					PlayerJoinViewPackage otherPlayerJoinViewPackage = PackageFactory.GetPackage(PackageType.PlayerJoinView) as PlayerJoinViewPackage;
					otherPlayerJoinViewPackage.playerId = otherPlayer.playerId;
					otherPlayerJoinViewPackage.aoId = otherPlayer.aoId;
					otherPlayerJoinViewPackage.birthPlace = otherPlayer.position;
					this.worker.SendPackage(otherPlayerJoinViewPackage);
				}
			}
			
			//剩下的人都是看不见的了
			for (int i = 0; i < viewPlayers.Count; i++) {
				ClientPlayer otherPlayer = NetManager.Instance.server.playerManager.GetPlayer(viewPlayers[i]);
				otherPlayer.RemoveViewPlayer(id);
				//通知其他人我离开了他的视野
				PlayerLeaveViewPackage leaveOtherPlayerPackage = PackageFactory.GetPackage(PackageType.PlayerLeaveView) as PlayerLeaveViewPackage;
				leaveOtherPlayerPackage.aoId = aoId;
				otherPlayer.worker.SendPackage(leaveOtherPlayerPackage);
				//通知我其他人离开了我的视野
				PlayerLeaveViewPackage otherPlayerLeavePackage = PackageFactory.GetPackage(PackageType.PlayerLeaveView) as PlayerLeaveViewPackage;
				otherPlayerLeavePackage.aoId = otherPlayer.aoId;
				this.worker.SendPackage(otherPlayerLeavePackage);
			}
			//最后更新自己能看见的人
			viewPlayers = curViewPlayers;
		}

		public void UpdateViewEntity()
		{
			List<int> curViewEntities = NetManager.Instance.server.entityManager.GetAroundEntity(inChunkPos,viewWidth);

			for (int i = 0; i < curViewEntities.Count; i++) {
				//如果原先已经包含了该玩家，那么跳过
				if(viewEntities.Contains(curViewEntities[i]))
				{
					RemoveViewEntity(curViewEntities[i]);
					continue;
				}
				else
				{
					//如果看见其他怪物了，其他怪物的应用需要更新
					ClientEntity otherEntity = NetManager.Instance.server.entityManager.GetEntity(curViewEntities[i]);
					otherEntity.AddViewPlayer(id);

					//通知当前玩家，有entity进入视野
					EntityJoinViewPackage entityJoinViewPackage = PackageFactory.GetPackage(PackageType.EntityJoinView)
						as EntityJoinViewPackage;
					ClientEntityInfo info = new ClientEntityInfo();
					info.aoId = otherEntity.aoId;
					info.type = otherEntity.type;
					info.entityId = otherEntity.entityId;
					info.position = otherEntity.position;
					info.extData = otherEntity.extData;
					info.roleId = otherEntity.hostPlayer == null ? -1 : otherEntity.hostPlayer.id;
					entityJoinViewPackage.info = info;
					this.worker.SendPackage(entityJoinViewPackage);

					//更新当前entity所属
					otherEntity.CheckViewHold();
				}
			}
			
			//剩下的怪物都是看不见的了
			for (int i = 0; i < viewEntities.Count; i++) {
				ClientEntity otherEntity = NetManager.Instance.server.entityManager.GetEntity(viewEntities[i]);
				otherEntity.RemoveViewPlayer(id);

				//通知玩家，有entity退出视野
				EntityLeaveViewPackage entityLeaveViewPackage = PackageFactory.GetPackage(PackageType.EntityLeaveView)
					as EntityLeaveViewPackage;
				entityLeaveViewPackage.aoId = otherEntity.aoId;
				entityLeaveViewPackage.type = otherEntity.type;
				this.worker.SendPackage(entityLeaveViewPackage);

				//更新当前entity所属
				otherEntity.CheckViewHold();
			}
			//最后更新自己能看见的人
			viewEntities = curViewEntities;
		}

		public void AddChunkPos(WorldPos pos)
		{
			lock(chunkPosList)
			{
				if(!chunkPosList.Contains(pos))
				{
					chunkPosList.Add(pos);
				}
			}
		}

		public bool RemoveChunkPos(WorldPos pos)
		{
			lock(chunkPosList)
			{
				return chunkPosList.Remove(pos);
			}
		}

//		public bool AddRenderChunkPos(WorldPos pos)
//		{
//			lock(renderChunkPosList)
//			{
//				if(!renderChunkPosList.Contains(pos))
//				{
//					renderChunkPosList.Add(pos);
//					return true;
//				}
//				return false;
//			}
//		}
//
//		public bool RemoveRenderChunkPos(WorldPos pos)
//		{
//			lock(renderChunkPosList)
//			{
//				return renderChunkPosList.Remove(pos);
//			}
//		}

		public void Dispose()
		{
			lock(chunkPosList)
			{
				for (int i = chunkPosList.Count - 1; i >= 0; i--) {
					NetManager.Instance.server.sceneManager.RemovePlayerChunks(this,chunkPosList[i]);
				}
			}

			for (int i = 0; i < viewPlayers.Count; i++) {
				ClientPlayer otherPlayer = NetManager.Instance.server.playerManager.GetPlayer(viewPlayers[i]);
				otherPlayer.RemoveViewPlayer(id);
			}

			for (int i = 0; i < viewEntities.Count; i++) {
				ClientEntity otherEntity = NetManager.Instance.server.entityManager.GetEntity(viewEntities[i]);
				otherEntity.RemoveViewPlayer(id);
				otherEntity.CheckViewHold();
			}
//			renderChunkPosList.Clear();
			viewPlayers.Clear();
			viewEntities.Clear();
			worker = null;
		}
	}
}

