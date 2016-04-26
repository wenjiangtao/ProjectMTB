using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Net;
using System;
using MTB;

public class Client : MonoBehaviour {

	public string ip{get;private set;}
	public int port{get;private set;}
	public int roleId{get;private set;}
	ServerConnectionWorker serverConnectionWorker;
	public bool isConnection{get;private set;}
	public bool isMainClient{get;private set;}

	void Awake()
	{
		isConnection = false;
		isMainClient = false;
		EventManager.RegisterEvent (NetEventMacro.ON_SERVER_START, HandleRequestConnect); 
	}

	public void Init()
	{
	}

	public int GetRoleId(string ip)
	{
		IPAddress address = IPAddress.Parse(ip);
		return GetRoleId(address);
	}

	public int GetRoleId(IPAddress address)
	{
		byte[] dataBytes = address.GetAddressBytes();
		int roleId = BitConverter.ToInt32(dataBytes,0);
		//因为要在本机上测试，所以加上当前进程id作为唯一表示符
		int id = System.Diagnostics.Process.GetCurrentProcess().Id;
		roleId += id;
		Debug.Log("创建roleId:" + roleId);
		return roleId;
	}

	public int GetRoleId()
	{
		IPEndPoint ipPoint = (IPEndPoint)serverConnectionWorker.conn.LocalEndPoint;
		return GetRoleId(ipPoint.Address);
	}

	private void HandleRequestConnect(params object [] paras){
		string ip = System.Convert.ToString (paras [0]);
		int port = System.Convert.ToInt32 (paras [1]);
		UnityEngine.Debug.Log("本机连接服务器开始!");
		this.Connect (ip, port);
		isMainClient = true;
		InitServerMainPlayerInfo();
		AddMainClientEvents();
		//主机同步开始
		StartSync();
	}

	private void AddMainClientEvents()
	{
		EventManager.RegisterEvent(NetEventMacro.ON_SERVER_STOP,OnServerStop);
	}

	private void RemoveMainClientEvents()
	{
		EventManager.UnRegisterEvent(NetEventMacro.ON_SERVER_STOP,OnServerStop);
	}

	private void AddClientEvents()
	{
		EventManager.RegisterEvent(EventMacro.CHUNK_DATA_GENERATE_FINISH,OnChunkDataGenerateFinish);
		EventManager.RegisterEvent(EventMacro.CHUNK_DATA_REMOVE_FINISH,OnChunkDataRemoveFinish);
		EventManager.RegisterEvent(EventMacro.CHUNK_DATA_SAVE,OnChunkDataSave);
		EventManager.RegisterEvent(EventMacro.BLOCK_DATA_UPDATE_AFTER_POPULATION,OnBlockUpdateAfterPopulation);
	}

	private void RemoveClientEvents()
	{
		EventManager.UnRegisterEvent(EventMacro.CHUNK_DATA_GENERATE_FINISH,OnChunkDataGenerateFinish);
		EventManager.UnRegisterEvent(EventMacro.CHUNK_DATA_REMOVE_FINISH,OnChunkDataRemoveFinish);
		EventManager.UnRegisterEvent(EventMacro.CHUNK_DATA_SAVE,OnChunkDataSave);
		EventManager.UnRegisterEvent(EventMacro.BLOCK_DATA_UPDATE_AFTER_POPULATION,OnBlockUpdateAfterPopulation);
	}

	private void OnBlockUpdateAfterPopulation(object[] param)
	{
		ChunkBlockChangedPackage package = PackageFactory.GetPackage(PackageType.ChunkBlockChanged) as ChunkBlockChangedPackage;
		WorldPos pos = (WorldPos)param[0];
		int x = (int)param[1];
		int y = (int)param[2];
		int z = (int)param[3];
		Block b = (Block)param[4];
		Int16 index = ClientChangedChunk.GetChunkIndex(x,y,z);
		byte blockType = (byte)b.BlockType;
		byte extendId = b.ExtendId;
		package.pos = pos;
		package.changedBlock = new ClientChangedBlock(index,blockType,extendId);
		SendPackage(package);
	}

	//addChunk到服务器上必须比UpdateAfterPopulation早，因此，只要数据产生，无论是读取网络数据、本地数据、产生数据都需要发送
	private void OnChunkDataGenerateFinish(object[] param)
	{
		Chunk chunk = param[0] as Chunk;
		PlayerAddChunkPackage package = PackageFactory.GetPackage(PackageType.PlayerAddChunk) as PlayerAddChunkPackage;
		package.pos = chunk.worldPos;
		package.sign = chunk.GetSign();
		SendPackage(package);
	}

	private void OnChunkDataRemoveFinish(object[] param)
	{
		NetRemovedChunk netRemovedChunk = param[0] as NetRemovedChunk;
		Chunk chunk = netRemovedChunk.chunk;
		//当chunk为移除时，向服务器发送chunk移除消息
		PlayerRemoveChunkPackage removePackage = PackageFactory.GetPackage(PackageType.PlayerRemoveChunk) as PlayerRemoveChunkPackage;
		removePackage.pos = chunk.worldPos;
		SendPackage(removePackage);

		if(netRemovedChunk.needSave)
		{
			for (int i = 0; i < netRemovedChunk.changedEntityInfos.Count; i++) {
				EntityData entityData = new EntityData();
				ClientEntityInfo info = netRemovedChunk.changedEntityInfos[i];
				entityData.id = info.entityId;
				entityData.type = info.type;
				entityData.pos = info.position;
				entityData.exData = info.extData;
				chunk.AddEntityData(entityData);
			}
			NetChunkData netChunkData = WorldPersistanceManager.Instance.GetNetChunkData(chunk,roleId);
			//如果当前客户端是主机客户端，直接保存文件到当前本地，否则，将数据发送给服务器，保存在主机
			if(isMainClient)
			{
				World.world.WorldGenerator.DataProcessorManager.EnqueueSaveNetChunkData(netChunkData);
			}
			else
			{
//				//上面已经移除掉了
//				//先移除
//				PlayerRemoveChunkPackage removeChunkPackage = PackageFactory.GetPackage(PackageType.PlayerRemoveChunk) as PlayerRemoveChunkPackage;
//				removeChunkPackage.pos = chunk.worldPos;
//				SendPackage(removeChunkPackage);
				//先需要从服务器上拿最新的entity数据,更新后再保存
				//再保存
				PlayerSaveChunkOnServerPackage package = PackageFactory.GetPackage(PackageType.PlayerSaveChunkOnServer)
					as PlayerSaveChunkOnServerPackage;
				package.pos = chunk.worldPos;
				package.chunkByteData = netChunkData.data.data;
				package.compressType = (byte)netChunkData.data.compressType;
				package.roleId = netChunkData.roleId;
				SendPackage(package);
			}
		}
	}

	private void OnChunkDataSave(object[] param)
	{
		NetRemovedChunk savedChunk = param[0] as NetRemovedChunk;
		if(savedChunk.needSave)
		{
			Chunk chunk = savedChunk.chunk;
			for (int i = 0; i < savedChunk.changedEntityInfos.Count; i++) {
				EntityData entityData = new EntityData();
				ClientEntityInfo info = savedChunk.changedEntityInfos[i];
				entityData.id = info.entityId;
				entityData.type = info.type;
				entityData.pos = info.position;
				entityData.exData = info.extData;
				chunk.AddEntityData(entityData);
			}
			NetChunkData netChunkData = WorldPersistanceManager.Instance.GetNetChunkData(chunk,roleId);
			//如果当前客户端是主机客户端，直接保存文件到当前本地，否则，将数据发送给服务器，保存在主机
			if(isMainClient)
			{
				World.world.WorldGenerator.DataProcessorManager.EnqueueSaveNetChunkData(netChunkData);
			}
			else
			{
				PlayerSaveChunkOnServerPackage package = PackageFactory.GetPackage(PackageType.PlayerSaveChunkOnServer)
					as PlayerSaveChunkOnServerPackage;
				package.pos = chunk.worldPos;
				package.chunkByteData = netChunkData.data.data;
				package.compressType = (byte)netChunkData.data.compressType;
				package.roleId = netChunkData.roleId;
				SendPackage(package);
			}
		}
	}

	private void InitServerMainPlayerInfo()
	{
		//初始化主玩家的一些信息
		MainClientInitPackage package = PackageFactory.GetPackage(PackageType.MainClientInit) as MainClientInitPackage;
		GameObject mainPlayer = HasActionObjectManager.Instance.playerManager.getMyPlayer();
		package.roleId = roleId;
		package.position = mainPlayer.transform.position;
		package.aoId = mainPlayer.GetComponent<PlayerAttributes>().aoId;
		package.playerId = mainPlayer.GetComponent<PlayerAttributes>().playerId;
		package.worldConfigName = WorldConfig.Instance.name;
		package.seed = WorldConfig.Instance.seed;
		package.time = DayNightTime.Instance.TotalTime;
		SendPackage(package);

		//初始化当前玩家内存中的chunk信息到服务器
		foreach (var key in World.world.chunks) {

			WorldPos chunkPos = key.Key;
			int sign = key.Value.GetSign();
			PlayerAddChunkPackage addChunkPackage = PackageFactory.GetPackage(PackageType.PlayerAddChunk) as PlayerAddChunkPackage;
			addChunkPackage.pos = chunkPos;
			addChunkPackage.sign = sign;
			SendPackage(addChunkPackage);
		}

		//初始化主玩家的entity信息到服务器上(暂时只同步怪物)
		MainClientEntityInitPackage entityInitPackage = PackageFactory.GetPackage(PackageType.MainClientEntityInit) as MainClientEntityInitPackage;
		List<ClientEntityInfo> list = new List<ClientEntityInfo>();
		List<GameObject> listMonster = HasActionObjectManager.Instance.monsterManager.listObj();
		for (int i = 0; i < listMonster.Count; i++) {
			GOMonsterController controller = listMonster[i].GetComponent<GOMonsterController>();
			ClientEntityInfo info = new ClientEntityInfo();
			EntityData entityData = controller.GetEntityData();
			info.roleId = roleId;
			info.aoId = controller.monsterAttribute.aoId;
			info.entityId = entityData.id;
			info.type = EntityType.MONSTER;
			info.position = entityData.pos;
			info.extData = entityData.exData;
			list.Add(info);
		}
		entityInitPackage.entityInfos = list;
		SendPackage(entityInitPackage);
	}

	private void OnServerStop(object[] param)
	{
		RemoveMainClientEvents();
		GameObject mainPlayer = HasActionObjectManager.Instance.playerManager.getMyPlayer();
		if(mainPlayer != null)
		{
			int aoId = mainPlayer.GetComponent<PlayerAttributes>().aoId;
			HasActionObjectManager.Instance.playerManager.RemoveObjBeside(aoId);
		}
		DisConnection();
	}

	public void Connect(string ip, int port)
	{
		Debug.Log("正在连接服务器:" + ip + ":" + port + " ....");
		this.ip = ip;
		this.port = port;
		TcpConnection conn = new TcpConnection (ip, port, false);
		conn.Connect ();

		serverConnectionWorker = new ServerConnectionWorker(conn);
        serverConnectionWorker.Start();
        
		isConnection = true;
        StartCoroutine(HandleReadPackage());
		//连接成功后缓存当前人物的roleId
		roleId = GetRoleId();
		LinkStatePackage package = PackageFactory.GetPackage(PackageType.LinkState) as LinkStatePackage;
		SendPackage(package);
		AddClientEvents();
	}

	public void JoinGame(int playerId)
	{
		EventManager.RegisterEvent(EventMacro.ON_JOIN_SCENE_SUCCESS,OnJoinSceneSuccess);
		Debug.Log("正在进入游戏...");
		JoinGamePackage package = PackageFactory.GetPackage(PackageType.JoinGame) as JoinGamePackage;
		package.roleId = roleId;
		package.playerId = playerId;
		SendPackage(package);
	}

	private void OnJoinSceneSuccess(object[] param)
	{
		EventManager.UnRegisterEvent(EventMacro.ON_JOIN_SCENE_SUCCESS,OnJoinSceneSuccess);
		//下面是局域网联机的时候才有作用
		LoadScenePlayers();
		LoadSceneEntities();
		//其他客户端同步开始
		StartSync();
	}

	public void DelayRefreshEntity(int aoId)
	{
		if(!isConnection)return;
		DelayRefreshEntityPackage package = PackageFactory.GetPackage(PackageType.DelayRefreshEntity)
			as DelayRefreshEntityPackage;
		package.aoId = aoId;
		package.needRefresh = false;
		SendPackage(package);
	}

	private void StartSync()
	{
		//开始位置同步
		StartCoroutine(SyncPosition());
		StartCoroutine(SyncAction());
		//如果是主机，那么主机将一直同步服务器上面的时间
		if(isMainClient)
		{
			StartCoroutine(SyncGameTime());
		}
	}

	private IEnumerator SyncPosition()
	{
		while(true)
		{
			SyncPlayerPosition();
			SyncMonsterPosition();
			yield return new WaitForSeconds(0.3f);
		}
	}

	private void SyncPlayerPosition()
	{
		PositionPackage package = PackageFactory.GetPackage(PackageType.Position) as PositionPackage;
		GameObject mainPlayer = HasActionObjectManager.Instance.playerManager.getMyPlayer();
		package.position = mainPlayer.transform.position;
		package.aoId = mainPlayer.GetComponent<GOPlayerController>().playerAttribute.aoId;
		SendPackage(package);
	}

	private void SyncMonsterPosition()
	{
		List<GameObject> list = HasActionObjectManager.Instance.monsterManager.listObj();
		for (int i = 0; i < list.Count; i++) {
			MonsterSyncPositionPackage package = PackageFactory.GetPackage(PackageType.MonsterSyncPosition)
				as MonsterSyncPositionPackage;
			package.position = list[i].transform.position;
			package.aoId = list[i].GetComponent<GameObjectController>().baseAttribute.aoId;
			SendPackage(package);
		}
	}
	
	private IEnumerator SyncAction()
	{
		while(true)
		{
			SyncPlayerAction();
			SyncMonsterAction();
			yield return null;
		}
	}

	private void SyncPlayerAction()
	{
		ActionPackage package = PackageFactory.GetPackage(PackageType.Action) as ActionPackage;
		GameObject mainPlayer = HasActionObjectManager.Instance.playerManager.getMyPlayer();
		GOPlayerController controller = mainPlayer.GetComponent<GOPlayerController>();
		package.aoId = controller.playerAttribute.aoId;
		package.direction = controller.playerInputState.moveDirection;
		package.isJump = controller.playerInputState.jump;
		package.actionId = controller.goActionController.curAction.actionData.id;
		package.yRotate = mainPlayer.transform.localRotation.eulerAngles.y;
		SendPackage(package);
	}

	private void SyncMonsterAction()
	{
		List<GameObject> list = HasActionObjectManager.Instance.monsterManager.listObj();
		for (int i = 0; i < list.Count; i++) {
			MonsterSyncActionPakage package = PackageFactory.GetPackage(PackageType.MonsterSyncAction)
				as MonsterSyncActionPakage;
			GameObjectController controller = list[i].GetComponent<GameObjectController>();
			package.aoId = controller.baseAttribute.aoId;
			package.direction = controller.gameObjectInputState.moveDirection;
			package.isJump = controller.gameObjectInputState.jump;
			package.actionId = controller.goActionController.curAction.actionData.id;
			package.yRotate = list[i].transform.localRotation.eulerAngles.y;
			SendPackage(package);
		}
	}
	
	private IEnumerator SyncGameTime()
	{
		while(true)
		{
			GameTimePackage package = PackageFactory.GetPackage(PackageType.GameTime) as GameTimePackage;
			package.time = DayNightTime.Instance.TotalTime;
			SendPackage(package);
			yield return new WaitForSeconds(5f);
		}
	}

	public void LoadScenePlayers()
	{
		if(!isMainClient)
		{
			SendPackage(PackageFactory.GetPackage(PackageType.LoadScenePlayer) as LoadScenePlayerPackage);
		}
	}

	public void LoadSceneEntities()
	{
		if(!isMainClient)
		{
			LoadSceneEntityPackage package = PackageFactory.GetPackage(PackageType.LoadSceneEntity)
				as LoadSceneEntityPackage;
			package.list = new List<ClientEntityInfo>();
			SendPackage(package);
		}
	}

	public void DisConnection()
	{
		RemoveClientEvents();
		isConnection = false; 
		serverConnectionWorker.Stop();
		serverConnectionWorker = null;
	}

	public void SendPackage(NetPackage package)
	{
		if (serverConnectionWorker == null || !isConnection)
			return;
		serverConnectionWorker.SendPackage(package);
	}

	IEnumerator HandleReadPackage()
	{
        //无影响
		while (isConnection) {
			if(serverConnectionWorker.HasDisConnection())
			{
				DisConnection();
				CloseApplication();
				yield break;
			}
			lock(serverConnectionWorker.readPackageQueue)
			{
				while (serverConnectionWorker.readPackageQueue.Count > 0)
				{
					NetPackage package = serverConnectionWorker.readPackageQueue.Dequeue();
					((TcpPackage)package).ClientDo(serverConnectionWorker);
				}
			}
			yield return null;
		}
	}

	void CloseApplication()
	{
		Debug.Log("服务器已断开连接!");
		Application.Quit();
	}

//	void OnDestroy()
//	{
//		if(isConnection)
//		{
//			RemoveMainClientEvents();
//			RemoveClientEvents();
//			this.serverConnectionWorker.Stop ();
//			this.serverConnectionWorker.Dispose();
//		}
//	}

	public void Dispose()
	{
		if(isConnection)
		{
			RemoveMainClientEvents();
			RemoveClientEvents();
			this.serverConnectionWorker.Stop ();
			this.serverConnectionWorker.Dispose();
		}
	}
}
