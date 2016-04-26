using System;
namespace MTB
{
	public class NetEventMacro
	{
		//搜索周围信号
		public const string ON_SEARCH_SERVER_SIGNAL = "On_Search_Server_Signal";
		//连接服务器
		public const string ON_LINK_SERVER = "On_Link_Server";
		//进入游戏
		public const string ON_JION_GAME = "On_Jion_Game";
		//服务器开启
		public const string ON_SERVER_START = "On_Server_Start";
		//服务器停止
		public const string ON_SERVER_STOP = "On_Server_Stop";
		//主机收到产生网络chunk
		public const string ON_NET_CHUNK_GENERATOR = "On_Net_Chunk_Generator";
		//其他客户端收到网络产生的chunk
		public const string ON_NET_CHUNK_GENERATOR_RETURN = "On_Net_Chunk_Generator_Return";
		//主机收到保存区块
		public const string ON_NET_SAVE_CHUNK = "On_Net_Save_Chunk";
		//生物群落产生
		public const string ON_NET_POPULATION_GENERATE = "On_Net_Population_Generate";

        public const string ON_NET_AREA_UPDATE = "On_Net_Area_Update";

		//接受到服务器返回的将要移除区块的额外信息
		public const string ON_RESPONSE_REMOVE_CHUNK_EXT_DATA = "On_Response_Remove_Chunk_Ext_Data";
	}
}

