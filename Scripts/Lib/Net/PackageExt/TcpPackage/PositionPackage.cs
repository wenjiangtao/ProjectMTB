using System;
using UnityEngine;
using System.Collections.Generic;
namespace MTB
{
	//玩家位置同步消息
	public class PositionPackage : TcpPackage
	{
		public int aoId{get;set;}
		public Vector3 position{get;set;}
		public PositionPackage (int id)
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
			PlayerManager manager = HasActionObjectManager.Instance.playerManager;
			GameObject player = manager.getObjById(aoId);
			if(player == null)return;
			if(Vector3.Distance(player.transform.position,position) > 2)
			{
//				Debug.Log("位置出现误差!修正中..." + player.transform.position.ToString() + "->" + position.ToString());
				player.transform.position = position;
			}
		}

		public override void ServerDo (ClientConnectionWorker connectionWork)
		{
			connectionWork.player.UpdatePosition(position);
			NetManager.Instance.server.BroadcastPackage(this,connectionWork.player,connectionWork.player.viewWidth,false);
		}
	}
}

