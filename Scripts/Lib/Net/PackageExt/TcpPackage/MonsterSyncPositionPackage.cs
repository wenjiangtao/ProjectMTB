using System;
using UnityEngine;
namespace MTB
{
	public class MonsterSyncPositionPackage : TcpPackage
	{
		public int aoId{get;set;}
		public Vector3 position{get;set;}
		public MonsterSyncPositionPackage (int id)
			:base(id)
		{
		}
		
		public override void ReadAllData ()
		{
			position = ReadVector3();
			aoId = ReadInt();
		}
		
		public override void WriteAllData ()
		{
			WriteVector3(position);
			WriteInt(aoId);
		}
		
		public override void ClientDo (ServerConnectionWorker connectionWork)
		{
			MonsterManager manager = HasActionObjectManager.Instance.monsterManager;
			GameObject monster = manager.getObjById(aoId);
			if(monster == null)return;
			if(monster.GetComponent<GameObjectController>().baseAttribute.isNetObj)
			{
				if(Vector3.Distance(monster.transform.position,position) > 2)
				{
					monster.transform.position = position;
				}
			}
		}
		
		public override void ServerDo (ClientConnectionWorker connectionWork)
		{
			ClientEntity entity = NetManager.Instance.server.entityManager.GetEntity(aoId);
			if(entity != null && entity.hostPlayer != null && entity.hostPlayer.id == connectionWork.player.id)
			{
				entity.UpdatePosition(position);
				NetManager.Instance.server.entityManager.BroadcastPackageInEntityViewPlayers(this,entity,connectionWork.player.id);
			}
		}
	}
}

