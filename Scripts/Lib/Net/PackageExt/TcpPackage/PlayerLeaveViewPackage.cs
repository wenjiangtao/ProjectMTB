using System;
namespace MTB
{
	//玩家离开视野范围
	public class PlayerLeaveViewPackage : TcpPackage
	{
		public int aoId{get;set;}
		public PlayerLeaveViewPackage (int id)
			:base(id)
		{
		}

		public override void ClientDo (ServerConnectionWorker connectionWork)
		{
			UnityEngine.Debug.Log("玩家roleId:" + aoId+"离开视野!");
			HasActionObjectManager.Instance.playerManager.removeObj(aoId);
		}
		
		public override void WriteAllData ()
		{
			WriteInt(aoId);
		}
		
		public override void ReadAllData ()
		{
			aoId = ReadInt();
		}
	}
}

