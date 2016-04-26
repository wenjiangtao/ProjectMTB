using System;
using System.Collections.Generic;
using UnityEngine;
namespace MTB
{
	public class RequestRefreshChunkEntitiesPackage : TcpPackage
	{
		public WorldPos pos{get;set;}
		public List<EntityData> entities{get;set;}
		public RequestRefreshChunkEntitiesPackage (int id)
			:base(id)
		{
		}

		public override void WriteAllData ()
		{
			WriteInt(pos.x);
			WriteInt(pos.y);
			WriteInt(pos.z);
			WriteInt(entities.Count);
			for (int i = 0; i < entities.Count; i++) {
				WriteEntityData(entities[i]);
			}
		}

		public override void ReadAllData ()
		{
			int x = ReadInt();
			int y = ReadInt();
			int z = ReadInt();
			pos = new WorldPos(x,y,z);
			int count = ReadInt();
			entities = new List<EntityData>();
			for (int i = 0; i < count; i++) {
				entities.Add(ReadEntityData());
			}
		}

		private void WriteEntityData(EntityData entityData)
		{
			WriteInt(entityData.id);
			WriteInt(entityData.type);
			WriteListInt(entityData.exData);
			WriteVector3(entityData.pos);
		}

		private EntityData ReadEntityData()
		{
			EntityData entityData = new EntityData();
			entityData.id = ReadInt();
			entityData.type = ReadInt();
			entityData.exData = ReadListInt();
			entityData.pos = ReadVector3();
			return entityData;
		}

		private void WriteListInt(List<int> list)
		{
			WriteInt(list.Count);
			for (int i = 0; i < list.Count; i++) {
				WriteInt(list[i]);
			}
		}
		
		private List<int> ReadListInt()
		{
			List<int> list = new List<int>();
			int count = ReadInt();
			for (int i = 0; i < count; i++) {
				list.Add(ReadInt());
			}
			return list;
		}

		public override void ServerDo (ClientConnectionWorker connectionWork)
		{
			if(NetManager.Instance.server.sceneManager.RefreshEntity(pos))
			{
				List<int> listRefershEntity = new List<int>();
				for (int i = 0; i < entities.Count; i++) {
					int aoId = AoIdManager.instance.getAoId();
					ClientEntityInfo info = new ClientEntityInfo();
					info.aoId = aoId;
					info.entityId = entities[i].id;
					info.type = entities[i].type;
					info.extData = entities[i].exData;
					info.position = entities[i].pos;
					info.roleId = connectionWork.player.id;
					NetManager.Instance.server.entityManager.InitEntity(info);
					listRefershEntity.Add(aoId);

				}
				//全部刷新完成后，通知持有该块的玩家更改刷新标志，并通知其刷新怪物
				ChunkSignChangedPackage signChangedPackage = PackageFactory.GetPackage(PackageType.ChunkSignChanged) as ChunkSignChangedPackage;
				signChangedPackage.pos = pos;
				signChangedPackage.sign = NetManager.Instance.server.sceneManager.GetSign(pos);
				NetManager.Instance.server.sceneManager.BroadcastPlayerHasChunkPackage(connectionWork.player,pos,signChangedPackage,true);
				for (int i = 0; i < listRefershEntity.Count; i++) {
					ClientEntity clientEntity = NetManager.Instance.server.entityManager.GetEntity(listRefershEntity[i]);
					ClientEntityInfo clientEntityInfo = clientEntity.GetClientEntityInfo();
					EntityJoinViewPackage entityJoinViewPackage = PackageFactory.GetPackage(PackageType.EntityJoinView)
						as EntityJoinViewPackage;
					entityJoinViewPackage.info = clientEntityInfo;
					for (int j = 0; j < clientEntity.viewPlayers.Count; j++) {
						ClientPlayer clientPlayer = NetManager.Instance.server.playerManager.GetPlayer(clientEntity.viewPlayers[j]);
						clientPlayer.worker.SendPackage(entityJoinViewPackage);
					}
				}
//				Debug.Log("请求刷新区块pos:" + pos.ToString() + "的entity成功,刷新数量:" + listRefershEntity.Count + " changedSign:" +signChangedPackage.sign);
			}
		}
	}
}

