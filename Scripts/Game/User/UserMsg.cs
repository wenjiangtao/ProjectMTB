using System;
using System.Collections.Generic;
using LitJson;
namespace MTB
{
	public class UserMsg
	{
		public static int SUCCESS = 1001;
		public static int FAIL = 1002;

		private static Dictionary<int,string> resultInfo = InitResultInfo();

		public int id{get;private set;}
		public string msg{get;private set;}
		public JsonData data{get;private set;}
		public UserMsg (int id)
		{
			this.id = id;
			if(resultInfo.ContainsKey(id))
			{
				this.msg = resultInfo[id];
			}
		}

		public UserMsg(int id,string msg)
			:this(id)
		{
			this.msg = msg;
		}

		public UserMsg(int id,string msg,JsonData data)
			:this(id,msg)
		{
			this.data = data;
		}

		public UserMsg(int id,JsonData data)
			:this(id)
		{
			this.data = data;
		}

		private static Dictionary<int,string> InitResultInfo()
		{
			Dictionary<int,string> info = new Dictionary<int, string>();
			info.Add(SUCCESS,"成功!");
			info.Add(FAIL,"失败!");
			return info;
		}
	}
}

