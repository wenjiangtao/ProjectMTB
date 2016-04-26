using System;
namespace MTB
{
    public enum PackageType
    {
        RequestServerSignal = 1001,
        BroadcastServerSignal = 1002,
        LinkState = 2001,
        JoinGame = 2002,
        ClientJoinGame = 2003,
        MainClientInit = 2004,
        LoadScenePlayer = 2005,
        Position = 2006,
        Action = 2007,
        LeaveGame = 2008,
        ResquestChunk = 2009,
        ResponseChunk = 2010,
        PlayerAddChunk = 2011,
        PlayerRemoveChunk = 2012,
        ChunkBlockChanged = 2013,
        ChunkSignChanged = 2014,
        BatchChunkBlockChanged = 2016,
        PlayerSaveChunkOnServer = 2017,
        PlayerJoinView = 2018,
        PlayerLeaveView = 2019,
        GameTime = 2020,
        ChunkAreaChanged = 2021,
		MainClientEntityInit = 2022,
		EntityJoinView = 2023,
		EntityLeaveView = 2024,
		EntityNetObjChanged = 2025,
		RequestRefreshChunkEntities = 2026,
		RequestRemoveChunkExtData = 2027,
		ResponseRemoveChunkExtData = 2028,
		MonsterSyncAction = 2029,
		MonsterSyncPosition = 2030,
		LoadSceneEntity = 2031,
		DelayRefreshEntity = 2032
    }
}

