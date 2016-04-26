using System;
using System.Collections.Generic;
namespace MTB
{
	public class DelayRefreshEntityPackage : TcpPackage
	{
		public ClientEntityInfo info{get;set;}
		public bool needRefresh{get;set;}
		public int aoId{get;set;}
		public DelayRefreshEntityPackage (int id)
			:base(id)
		{
		}

		public override void WriteAllData ()
		{
			WriteInt(aoId);
			WriteBool(needRefresh);
			if(needRefresh)
			{
				WriteClientEntityInfo(info);
			}
		}

		public override void ReadAllData ()
		{
			aoId = ReadInt();
			needRefresh = ReadBool();
			if(needRefresh)
			{
				info = ReadClientEntityInfo();
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
			if(needRefresh)
			{
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
			ClientEntity entity = NetManager.Instance.server.entityManager.GetEntity(aoId);
			if(entity != null && entity.viewPlayers.Contains(connectionWork.player.id))
			{
				needRefresh = true;
				info = entity.GetClientEntityInfo();
			}
			else
			{
				needRefresh = false;
			}
			connectionWork.SendPackage(this);
		}
	}
}

