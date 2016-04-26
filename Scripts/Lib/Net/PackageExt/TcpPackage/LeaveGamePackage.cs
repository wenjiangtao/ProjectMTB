using System;
namespace MTB
{
	//客户端收到其他玩家离开游戏的消息
	public class LeaveGamePackage : TcpPackage
	{
		public int aoId{get;set;}
		public LeaveGamePackage (int id)
			:base(id)
		{
		}

		public override void ClientDo (ServerConnectionWorker connectionWork)
		{
			UnityEngine.Debug.Log("玩家roleId:" + aoId+"离开游戏!");
			//如果在我的视野，就删除，如果不在，就不管
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

