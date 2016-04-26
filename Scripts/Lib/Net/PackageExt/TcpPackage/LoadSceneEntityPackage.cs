using System;
using System.Collections.Generic;
using UnityEngine;
namespace MTB
{
	//玩家进入游戏后请求附近entity的消息
	public class LoadSceneEntityPackage : TcpPackage
	{
		public List<ClientEntityInfo> list;
		public LoadSceneEntityPackage (int id)
			:base(id)
		{
		}

		public override void WriteAllData ()
		{
			WriteInt(list.Count);
			for (int i = 0; i < list.Count; i++) {
				WriteClientEntityInfo(list[i]);
			}
		}

		public override void ReadAllData ()
		{
			list = new List<ClientEntityInfo>();
			int count = ReadInt();
			for (int i = 0; i < count; i++) {
				list.Add(ReadClientEntityInfo());
			}
		}

		private void WriteClientEntityInfo(ClientEntityInfo info)
		{
			WriteInt(info.aoId);
			WriteInt(info.entityId);
			WriteInt(info.type);
			WriteInt(info.roleId);
			WriteListInt(info.extData);
			WriteVector3(info.position);
		}
		
		private ClientEntityInfo ReadClientEntityInfo()
		{
			ClientEntityInfo info = new ClientEntityInfo();
			info.aoId = ReadInt();
			info.entityId = ReadInt();
			info.type = ReadInt();
			info.roleId = ReadInt();
			info.extData = ReadListInt();
			info.position = ReadVector3();
			return info;
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

		public override void ClientDo (ServerConnectionWorker connectionWork)
		{
			for (int i = 0; i < list.Count; i++) {
				ClientEntityInfo info = list[i];
				if(info.type == EntityType.MONSTER)
				{
					MonsterInfo monsterInfo = new MonsterInfo();
					monsterInfo.aoId = info.aoId;
					monsterInfo.monsterId = info.entityId;
					monsterInfo.position = info.position;
					monsterInfo.isNetObj = true;
					HasActionObjectManager.Instance.monsterManager.InitMonster(monsterInfo);
				}
				else if(info.type == EntityType.NPC)
				{
					NPCInfo npcInfo = new NPCInfo();
					npcInfo.aoId = info.aoId;
					npcInfo.NPCId = info.entityId;
					npcInfo.position = info.position;
					npcInfo.isNetObj = true;
					HasActionObjectManager.Instance.npcManager.InitNPC(npcInfo);
				}
				else if(info.type == EntityType.PLANT)
				{
					HasActionObjectManager.Instance.plantManager.buildPlant(info.position,(DecorationType)info.entityId,info.aoId);
				}
			}
		}

		public override void ServerDo (ClientConnectionWorker connectionWork)
		{
			List<int> viewEntities = connectionWork.player.viewEntities;
			list = new List<ClientEntityInfo>(viewEntities.Count);
			for (int i = 0; i < viewEntities.Count; i++) {
				ClientEntity entity = NetManager.Instance.server.entityManager.GetEntity(viewEntities[i]);
				list.Add(entity.GetClientEntityInfo());
			}
			connectionWork.SendPackage(this);
		}
	}
}

