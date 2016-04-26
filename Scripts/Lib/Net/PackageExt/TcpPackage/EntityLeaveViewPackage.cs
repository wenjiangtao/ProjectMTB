using System;
namespace MTB
{
	public class EntityLeaveViewPackage : TcpPackage
	{
		public int aoId{get;set;}
		public int type{get;set;}
		public EntityLeaveViewPackage (int id)
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
				HasActionObjectManager.Instance.monsterManager.removeObj(aoId);
			}
			else if(type == EntityType.NPC)
			{
				HasActionObjectManager.Instance.npcManager.removeObj(aoId);
			}
			else if(type == EntityType.PLANT)
			{
				HasActionObjectManager.Instance.plantManager.removePlant(aoId);
			}
			UnityEngine.Debug.Log("实体aoId:" + aoId + "离开视野!");
		}
	}
}

