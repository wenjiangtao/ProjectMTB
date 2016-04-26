using System;
using UnityEngine;
namespace MTB
{
	public class MonsterSyncActionPakage : TcpPackage
	{
		public MonsterSyncActionPakage (int id)
			:base(id)
		{
		}

		public int aoId{get;set;}
		public Vector3 direction{get;set;}
		public bool isJump{get;set;}
		public int actionId{get;set;}
		public float yRotate{get;set;}
		
		public override void WriteAllData ()
		{
			WriteInt(aoId);
			WriteVector3(direction);
			WriteBool(isJump);
			WriteInt(actionId);
			WriteFloat(yRotate);
		}
		
		public override void ReadAllData ()
		{
			aoId = ReadInt();
			direction = ReadVector3();
			isJump = ReadBool();
			actionId = ReadInt();
			yRotate = ReadFloat();
		}
		
		public override void ClientDo (ServerConnectionWorker connectionWork)
		{
			GameObject monster = HasActionObjectManager.Instance.monsterManager.getObjById(aoId);
			//做动作的时候，可能还没有真正实例化出来
			if(monster == null)return;
			monster.transform.localRotation = Quaternion.AngleAxis(yRotate, Vector3.up);
			GameObjectController controller = monster.GetComponent<GameObjectController>();
			if(controller.baseAttribute.isNetObj)
			{
				controller.Move(direction);
				if(isJump)
				{
					controller.Jump();
				}
				else
				{
					controller.StopJump();
				}
				controller.DoAction(actionId);
			}
		}
		
		public override void ServerDo (ClientConnectionWorker connectionWork)
		{
			ClientEntity clientEntity = NetManager.Instance.server.entityManager.GetEntity(aoId);
			NetManager.Instance.server.entityManager.BroadcastPackageInEntityViewPlayers(this,clientEntity,connectionWork.player.id);
		}
	}
}

