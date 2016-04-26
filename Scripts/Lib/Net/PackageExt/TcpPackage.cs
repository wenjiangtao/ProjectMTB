using System;
namespace MTB
{
	public abstract class TcpPackage : NetPackage
	{
		public TcpPackage (int id)
			:base(id)
		{
		}

		public virtual void ClientDo(ServerConnectionWorker connectionWork)
		{
		}
		
		public virtual void ServerDo(ClientConnectionWorker connectionWork)
		{
		}
	}
}

