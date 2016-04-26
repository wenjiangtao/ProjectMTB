using System;
using System.Collections.Generic;
namespace MTB
{
	public class PackageFactory
	{
		private static Dictionary<int,Type> _map = InitMap();
		private static Dictionary<int,Type> InitMap()
		{
			Dictionary<int,Type> map =new Dictionary<int, Type>();
			map.Add((int)PackageType.RequestServerSignal,typeof(RequestServerSignalPackage));
			map.Add((int)PackageType.BroadcastServerSignal,typeof(BroadcastServerSignalPackage));
			map.Add((int)PackageType.LinkState,typeof(LinkStatePackage));
			map.Add((int)PackageType.JoinGame,typeof(JoinGamePackage));
			map.Add((int)PackageType.ClientJoinGame,typeof(ClientJoinGamePackage));
			map.Add((int)PackageType.MainClientInit,typeof(MainClientInitPackage));
			map.Add((int)PackageType.LoadScenePlayer,typeof(LoadScenePlayerPackage));
			map.Add((int)PackageType.Position,typeof(PositionPackage));
			map.Add((int)PackageType.Action,typeof(ActionPackage));
			map.Add((int)PackageType.LeaveGame,typeof(LeaveGamePackage));
			map.Add((int)PackageType.ResquestChunk,typeof(RequestChunkPackage));
			map.Add((int)PackageType.ResponseChunk,typeof(ResponseChunkPackage));
			map.Add((int)PackageType.PlayerAddChunk,typeof(PlayerAddChunkPackage));
			map.Add((int)PackageType.PlayerRemoveChunk,typeof(PlayerRemoveChunkPackage));
			map.Add((int)PackageType.ChunkBlockChanged,typeof(ChunkBlockChangedPackage));
			map.Add((int)PackageType.ChunkSignChanged,typeof(ChunkSignChangedPackage));
			map.Add((int)PackageType.BatchChunkBlockChanged,typeof(ChunkPopulationGeneratePackage));
			map.Add((int)PackageType.PlayerSaveChunkOnServer,typeof(PlayerSaveChunkOnServerPackage));
			map.Add((int)PackageType.PlayerJoinView,typeof(PlayerJoinViewPackage));
			map.Add((int)PackageType.PlayerLeaveView,typeof(PlayerLeaveViewPackage));
			map.Add((int)PackageType.GameTime,typeof(GameTimePackage));
            map.Add((int)PackageType.ChunkAreaChanged, typeof(ChunkAreaChangedPackage));
			map.Add((int)PackageType.MainClientEntityInit, typeof(MainClientEntityInitPackage));
			map.Add((int)PackageType.EntityJoinView, typeof(EntityJoinViewPackage));
			map.Add((int)PackageType.EntityLeaveView,typeof(EntityLeaveViewPackage));
			map.Add((int)PackageType.EntityNetObjChanged,typeof(EntityNetObjChangedPackage));
			map.Add((int)PackageType.RequestRefreshChunkEntities,typeof(RequestRefreshChunkEntitiesPackage));
			map.Add((int)PackageType.RequestRemoveChunkExtData,typeof(RequestRemoveChunkExtDataPackage));
			map.Add((int)PackageType.ResponseRemoveChunkExtData,typeof(ResponseRemoveChunkExtDataPackage));
			map.Add((int)PackageType.MonsterSyncAction,typeof(MonsterSyncActionPakage));
			map.Add((int)PackageType.MonsterSyncPosition,typeof(MonsterSyncPositionPackage));
			map.Add((int)PackageType.LoadSceneEntity,typeof(LoadSceneEntityPackage));
			map.Add((int)PackageType.DelayRefreshEntity,typeof(DelayRefreshEntityPackage));
			return map;
		}

		public static NetPackage GetPackage(int id)
		{
			Type type;
			_map.TryGetValue(id,out type);
			if(type == null)throw new Exception("没有配置id为:" + id +"的网络包!");
			return Activator.CreateInstance(type,new object[]{id}) as NetPackage;
		}

		public static NetPackage GetPackage(PackageType type)
		{
			if(type == PackageType.Action)return new ActionPackage((int)type);
			if(type == PackageType.Position)return new PositionPackage((int)type);
			return GetPackage((int)type);
		}
	}
}

