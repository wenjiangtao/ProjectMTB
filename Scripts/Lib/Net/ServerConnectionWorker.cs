using System;
namespace MTB
{
	//如服务器连接的worker
	public class ServerConnectionWorker : ConnectionWorker
	{
		public ServerConnectionWorker (Connection conn)
			:base(conn)
		{
		}
	}
}

