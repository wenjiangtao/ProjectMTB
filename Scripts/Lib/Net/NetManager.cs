using System;
using System.Collections;
using UnityEngine;
namespace MTB
{
	public class NetManager : Singleton<NetManager>
	{
		public Server server{get;private set;}
		public Client client{get;private set;}
		public BroadcastService broadcast{get;private set;}
		public bool isServer{get;private set;}

		void Awake()
		{
			Instance = this;
			server = gameObject.GetComponent<Server>();
			client = gameObject.GetComponent<Client>();
			broadcast = gameObject.GetComponent<BroadcastService>();
			isServer = false;
		}

		public void StartServer()
		{
			StartCoroutine(StartServerCoroutine());
		}

		private IEnumerator StartServerCoroutine()
		{
			MTBKeyboard.setEnable(false);
			MTBUserInput.Instance.SetJoyStickActive(false);
			//首先确保当前应该加载的所有区块数据都已经产生完（无论是从本地加载还是产生的）
			Debug.Log("等待数据产生中...");
			GUITextDebug.debug("等待数据产生中...");
			World.world.ChangeLoader(NetType.Local);
			while(!World.world.WorldGenerator.HasSingleDataGenerateFinish())
			{
				yield return new WaitForSeconds(0.5f);
			}
			Debug.Log("数据产生完毕,开始保存当前数据...");
			GUITextDebug.debug("数据产生完毕,开始保存当前数据...");
			foreach (var chunk in World.world.chunks.Values) {
				if(chunk.ResetEntity() > 0 || chunk.isUpdate)
				{
					WorldPersistanceManager.Instance.SaveChunk(chunk);
					yield return null;
				}
			}
			World.world.WorlderLoader.Start();
			Debug.Log("数据保存完毕!开始开启服务器...");
			GUITextDebug.debug("数据保存完毕!开始开启服务器...");
			isServer = true;
			try
			{
				server.StartAcceptClient();
				if(!broadcast.isBroadcast)
				{
					broadcast.StartBroadcast();
				}
			}catch(Exception e)
			{
				GUITextDebug.debug(e.Message + "\n" + e.StackTrace);
			}
			MTBKeyboard.setEnable(true);
			MTBUserInput.Instance.SetJoyStickActive(true);
		}

		public void StopServer()
		{
			isServer = false;
			server.StopServer();
			if(broadcast.isBroadcast)
			{
				broadcast.StopBroadcast();
			}
		}
	}
}

