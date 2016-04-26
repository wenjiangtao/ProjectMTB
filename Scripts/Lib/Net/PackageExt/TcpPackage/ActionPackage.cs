using System;
using System.Collections.Generic;
using UnityEngine;
namespace MTB
{
	//动作同步
	public class ActionPackage : TcpPackage
	{
		public int aoId{get;set;}
		public Vector3 direction{get;set;}
		public bool isJump{get;set;}
		public int actionId{get;set;}
		public float yRotate{get;set;}
		public ActionPackage (int id)
			:base(id)
		{
		}

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
			GameObject player = HasActionObjectManager.Instance.playerManager.getObjById(aoId);
			//做动作的时候，可能还没有真正实例化出来
			if(player == null)return;

			GameObjectController controller = player.GetComponent<GameObjectController>();
//			if(!controller.IsRead)return;
			player.transform.localRotation = Quaternion.AngleAxis(yRotate, Vector3.up);
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

		public override void ServerDo (ClientConnectionWorker connectionWork)
		{
			NetManager.Instance.server.BroadcastPackage(this,connectionWork.player,connectionWork.player.viewWidth,false);
		}
	}
}

