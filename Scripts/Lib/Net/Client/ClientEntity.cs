using System;
using System.Collections.Generic;
using UnityEngine;
namespace MTB
{
	public class ClientEntity
	{
		public int aoId{get;private set;}
		public int type{get;private set;}
		public int entityId{get;private set;}
		public List<int> extData{get;private set;}
		public WorldPos inChunkPos{get;private set;}
		private Vector3 _position;
		public Vector3 position{get{
				return _position;
			}}
		//是由哪个玩家产生的
		public ClientPlayer hostPlayer{get;private set;}

		//被哪些玩家看到
		public List<int> viewPlayers{get;private set;}
		public ClientEntity (ClientEntityInfo info)
		{
			this.aoId = info.aoId;
			this.type = info.type;
			this.entityId = info.entityId;
			this.extData = info.extData;
			this.hostPlayer = NetManager.Instance.server.playerManager.GetPlayer(info.roleId);
			InitPosition(info.position);
		}

		public ClientEntityInfo GetClientEntityInfo()
		{
			ClientEntityInfo info = new ClientEntityInfo();
			info.aoId = aoId;
			info.type = type;
			info.entityId = entityId;
			info.extData = extData;
			info.position = position;
			info.roleId = hostPlayer == null ? 0 : hostPlayer.id;
			return info;
		}

		public void InitPosition(Vector3 position)
		{
			_position = position;
			inChunkPos = Terrain.GetChunkPos(position);
			NetManager.Instance.server.sceneManager.ChangeChunkNeedSaved(inChunkPos,true);
			viewPlayers = NetManager.Instance.server.playerManager.GetAroundPlayer(inChunkPos,ClientPlayer.VIEW_WIDTH);
			for (int i = 0; i < viewPlayers.Count; i++) {
				ClientPlayer clientPlayer = NetManager.Instance.server.playerManager.GetPlayer(viewPlayers[i]);
				clientPlayer.AddViewEntity(aoId);
			}
		}

		//检测当前entity的持有者是否正确，如不正确，会变更
		public void CheckViewHold()
		{
			if(hostPlayer != null)
			{
				if(!viewPlayers.Contains(hostPlayer.id))
				{
					this.hostPlayer = null;
				}
			}
			if(hostPlayer == null)
			{
				ClientPlayer player = null;
				int minDis = int.MaxValue;
				//找到距离当前怪物最近的玩家，将怪物的主动权交个他
				for (int i = 0; i < viewPlayers.Count; i++) {
					ClientPlayer tempPlayer = NetManager.Instance.server.playerManager.GetPlayer(viewPlayers[i]);
					int dis = Mathf.Max(Mathf.Abs(tempPlayer.inChunkPos.x - inChunkPos.x),Mathf.Abs(tempPlayer.inChunkPos.z - inChunkPos.z));
					if(dis < minDis)
					{
						player = tempPlayer;
						minDis = dis;
					}
				}
				this.hostPlayer = player;
				if(this.hostPlayer != null)
				{
					//通知当前玩家的entity变为本地entity，同步的时候就一当前玩家的entity为主
					EntityNetObjChangedPackage netObjChangedPackage = PackageFactory.GetPackage(PackageType.EntityNetObjChanged)
						as EntityNetObjChangedPackage;
					netObjChangedPackage.aoId = aoId;
					netObjChangedPackage.type = type;
					this.hostPlayer.worker.SendPackage(netObjChangedPackage);
				}
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
				NetManager.Instance.server.sceneManager.ChangeChunkNeedSaved(inChunkPos,true);
				NetManager.Instance.server.entityManager.UpdateEntityChunkPos(oldPos,inChunkPos,this);
				UpdateViewPlayer();
			}
		}

		private void UpdateViewPlayer()
		{
			List<int> curViewPlayers = NetManager.Instance.server.playerManager.GetAroundPlayer(inChunkPos,ClientPlayer.VIEW_WIDTH);
			for (int i = 0; i < curViewPlayers.Count; i++) {
				if(viewPlayers.Contains(curViewPlayers[i]))
				{
					RemoveViewPlayer(curViewPlayers[i]);
				}
				else
				{
					ClientPlayer otherPlayer = NetManager.Instance.server.playerManager.GetPlayer(curViewPlayers[i]);
					otherPlayer.AddViewEntity(aoId);
					//通知玩家，有entity进入视野
					EntityJoinViewPackage entityJoinViewPackage = PackageFactory.GetPackage(PackageType.EntityJoinView)
						as EntityJoinViewPackage;
					ClientEntityInfo info = new ClientEntityInfo();
					info.aoId = aoId;
					info.type = type;
					info.entityId = entityId;
					info.position = position;
					info.extData = extData;
					info.roleId = hostPlayer == null ? -1 : hostPlayer.id;
					entityJoinViewPackage.info = info;
					otherPlayer.worker.SendPackage(entityJoinViewPackage);
				}
			}
			for (int i = 0; i < viewPlayers.Count; i++) {
				ClientPlayer otherPlayer = NetManager.Instance.server.playerManager.GetPlayer(viewPlayers[i]);
				otherPlayer.RemoveViewEntity(aoId);
				//通知玩家，有entity退出视野
				EntityLeaveViewPackage entityLeaveViewPackage = PackageFactory.GetPackage(PackageType.EntityLeaveView)
					as EntityLeaveViewPackage;
				entityLeaveViewPackage.aoId = aoId;
				entityLeaveViewPackage.type = type;
				otherPlayer.worker.SendPackage(entityLeaveViewPackage);
			}
			viewPlayers = curViewPlayers;
			//检测当前entity是否退出玩家视野，如果是的话，更改entity所属
			CheckViewHold();
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

		public void Dispose()
		{
			hostPlayer = null;
		}
	}
}

