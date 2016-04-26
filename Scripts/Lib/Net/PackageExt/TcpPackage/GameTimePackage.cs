using System;
namespace MTB
{
	public class GameTimePackage : TcpPackage
	{
		public float time{get;set;}
		public GameTimePackage (int id)
			:base(id)
		{
		}

		public override void WriteAllData ()
		{
			WriteFloat(time);
		}

		public override void ReadAllData ()
		{
			time = ReadFloat();
		}

		public override void ServerDo (ClientConnectionWorker connectionWork)
		{
			NetManager.Instance.server.gameTime = time;
			NetManager.Instance.server.BroadcastPackage(this,connectionWork.player,false);
		}

		public override void ClientDo (ServerConnectionWorker connectionWork)
		{
			DayNightTime.Instance.UpdateTime(time);
		}
	}
}

