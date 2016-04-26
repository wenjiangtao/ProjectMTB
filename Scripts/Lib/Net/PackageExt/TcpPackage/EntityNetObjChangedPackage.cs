using System;
using UnityEngine;
namespace MTB
{
	//将网络对象变为本地对象
	public class EntityNetObjChangedPackage :TcpPackage
	{
		public int aoId{get;set;}
		public int type{get;set;}
		public EntityNetObjChangedPackage (int id)
			:base(id)
		{
		}

		public override void WriteAllData ()
		{
			WriteInt(aoId);
			WriteInt(type);
		}

		public override void ReadAllData ()
		{
			aoId = ReadInt();
			type = ReadInt();
		}

		public override void ClientDo (ServerConnectionWorker connectionWork)
		{
			if(type == EntityType.MONSTER)
			{
				HasActionObjectManager.Instance.monsterManager.ChangeObjNet(aoId,false);
//				GameObject obj = HasActionObjectManager.Instance.monsterManager.getObjById(aoId);
//				GOMonsterController controller = obj.GetComponent<GOMonsterController>();
//				controller.ChangeNetObj(false);
			}
			else if(type == EntityType.NPC)
			{
//				HasActionObjectManager.Instance.npcManager.getObjById(aoId);
			}
			else if(type == EntityType.PLANT)
			{
//				HasActionObjectManager.Instance.plantManager.removePlant(aoId);
			}
			UnityEngine.Debug.Log("实体aoId:" + aoId+"的所属改变!");
		}
	}
}

