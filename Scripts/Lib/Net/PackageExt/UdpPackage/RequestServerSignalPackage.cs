using System;
namespace MTB
{
	public class RequestServerSignalPackage : UdpPackage
	{
		public RequestServerSignalPackage (int id)
			:base(id)
		{
		}

		public override void Do ()
		{
			//如果当前服务器是开启的
			if(NetManager.Instance.isServer)
			{
				BroadcastServerSignalPackage package = PackageFactory.GetPackage(PackageType.BroadcastServerSignal) as BroadcastServerSignalPackage;
				package.serverName = "server";
				NetManager.Instance.broadcast.SendPackage(package);
				UnityEngine.Debug.Log("RequestServerSignalPackage");
			}
		}
	}
}

