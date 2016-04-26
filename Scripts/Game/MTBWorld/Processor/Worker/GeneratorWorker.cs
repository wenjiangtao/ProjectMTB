using System;
using System.Collections.Generic;
using Priority_Queue;
namespace MTB
{
    public class GeneratorWorker : ProcessWorker
    {
        #region implemented abstract members of ProcessWorker

        protected override void Process()
        {
            try
            {
                SingleGeneratorWorker();
                NetGeneratorWorker();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
            }
        }

        #endregion

        private void SingleGeneratorWorker()
        {
            //单机使用
            Chunk chunk = DeGeneratorQueue();
            if (chunk != null)
            {
				WorldPos pos = chunk.worldPos;
				if(World.world.GetChunk(pos.x,pos.y,pos.z) == null)return;
                if (!chunk.isTerrainDataPrepared)
                {
                    _terrainControlGenerator.Generate(chunk);
                    if (WorldConfig.Instance.CaveEnable)
                    {
                        _cavesGenerator.generate(chunk);
                    }
                    chunk.isTerrainDataPrepared = true;
                    chunk.isUpdate = true;
                }

                CheckAroundChunkAndGeneratorPopulation(chunk, false);
                RemoveSingleGenerateChunk(chunk);
            }

            RefreshChunkArea refreshChunkArea = DeRefreshQueue();
            if (refreshChunkArea != null)
            {
                for (int i = 0; i < refreshChunkArea.chunkList.Count; i++)
                {

                    if (refreshChunkArea.chunkList[i].refreshList.Count > 0)
                    {
                        for (int j = 0; j < refreshChunkArea.chunkList[i].refreshList.Count; j++)
                        {
                            UpdateBlock updateBlock = refreshChunkArea.chunkList[i].refreshList[j];
                            refreshChunkArea.chunkList[i].chunk.UpdateChangedBlock(updateBlock);
                        }
                    }
                }
                ReculateChunkAreaLightAndAround(refreshChunkArea, true);
            }
        }

        private void NetGeneratorWorker()
        {
            NetPriorityChunk netPriorityChunk = DeNetGeneratorQueue();
            if (netPriorityChunk != null)
            {
                Chunk chunk = netPriorityChunk.netChunk.chunk;
                if (!netPriorityChunk.netChunk.chunkData.isExist)
                {
					WorldPos pos = chunk.worldPos;
					if(World.world.GetChunk(pos.x,pos.y,pos.z) == null)return;
                    if (!chunk.isTerrainDataPrepared)
                    {
                        _terrainControlGenerator.Generate(chunk);
                        if (WorldConfig.Instance.CaveEnable)
                        {
                            _cavesGenerator.generate(chunk);
                        }
                        chunk.isTerrainDataPrepared = true;
                        chunk.isUpdate = true;
                        //这里是地形数据在本机上产生时，需要发送到服务器上
                        EventManager.SendEvent(EventMacro.CHUNK_DATA_GENERATE_FINISH, chunk);
                    }
                }
                //如果服务器上已经有最新的数据，拿到最新的数据更改
                if (netPriorityChunk.netChunk.chunkData.hasChangeData)
                {
                    ClientChangedChunkData changedData = netPriorityChunk.netChunk.chunkData.changedData;
                    chunk.UpdateSign(changedData.sign);
                    if (changedData.list.Count > 0)
                    {
                        for (int i = 0; i < changedData.list.Count; i++)
                        {
                            ClientChangedBlock changedBlock = changedData.list[i];
                            chunk.UpdateClientChangedBlock(changedBlock);
                        }
                        ReculateChunkLightAndAround(chunk);
                    }
                }
                CheckAroundChunkAndGeneratorPopulation(chunk, true);
            }

            //////////////////////////////////////////////////// 
            //收到服务器同步的大块更改数据
            refreshNetArea();
        }


        private HeapPriorityQueue<PriorityChunk> _selfGeneratorQueue;
        private HeapPriorityQueue<NetPriorityChunk> _netGeneratorQueue;
        private Queue<RefreshChunkArea> _refreshQueue;  
        private Queue<RefreshChunkArea> _netRefreshQueue;

        private TerrainControlGenerator _terrainControlGenerator;
        private CaveGenerator _cavesGenerator;
        private PopulationControlGenerator _populationControlGenerator;
        private LightControlGenerator _lightControlGenerator;
        private object GenerateLock = new object();
        private int singleGenerateNum;
        //		private int netGenerateNum;

        public GeneratorWorker(DataProcessorManager manager)
            : base(manager)
        {
            _selfGeneratorQueue = new HeapPriorityQueue<PriorityChunk>(1000);
            _netGeneratorQueue = new HeapPriorityQueue<NetPriorityChunk>(1000);
            _refreshQueue = new Queue<RefreshChunkArea>(1000);
            _netRefreshQueue = new Queue<RefreshChunkArea>(1000);
            _terrainControlGenerator = new TerrainControlGenerator();
            _cavesGenerator = new CaveGenerator(WorldConfig.Instance.seed);
            _populationControlGenerator = new PopulationControlGenerator();
            _lightControlGenerator = new LightControlGenerator(_manager.World);
            singleGenerateNum = 0;
            //			netGenerateNum = 0;
        }

        public void AddSingleGenerateChunk()
        {
            lock (GenerateLock)
            {
                singleGenerateNum++;
            }
        }

        private void RemoveSingleGenerateChunk(Chunk chunk)
        {
            lock (GenerateLock)
            {
                singleGenerateNum--;
                if (singleGenerateNum < 0) singleGenerateNum = 0;
            }
        }

        public bool hasSingleDataGenerated()
        {
            lock (GenerateLock)
            {
                return singleGenerateNum == 0;
            }
        }
        public void EnGeneratorQueue(PriorityChunk priorityChunk)
        {
            lock (_selfGeneratorQueue)
            {
                if (_selfGeneratorQueue.Count > _selfGeneratorQueue.MaxSize) return;
                _selfGeneratorQueue.Enqueue(priorityChunk, priorityChunk.Priority);
            }
        }

        private Chunk DeGeneratorQueue()
        {
            lock (_selfGeneratorQueue)
            {
                if (_selfGeneratorQueue.Count > 0)
                {
                    PriorityChunk c = _selfGeneratorQueue.Dequeue();
                    Chunk chunk = c.chunk;
                    _manager.SetPriorityChunk(c);
                    return chunk;
                }
                return null;
            }
        }

        //刷新区块网格和光照
        public void EnRefreshQueue(RefreshChunkArea chunk)
        {
            lock (_refreshQueue)
            {
                if (_refreshQueue.Count > 1000) return;
                _refreshQueue.Enqueue(chunk);
            }
        }



        private RefreshChunkArea DeRefreshQueue()
        {
            lock (_refreshQueue)
            {
                if (_refreshQueue.Count > 0)
                {
                    RefreshChunkArea c = _refreshQueue.Dequeue();
                    return c;
                }
                return null;
            }
        }


        //刷新网络大范围区块网格和光照
        public void EnNetRefreshAreaQueue(RefreshChunkArea chunk)
        {
            lock (_netRefreshQueue)
            {
                if (_netRefreshQueue.Count > 1000) return;
                _netRefreshQueue.Enqueue(chunk);
            }
        }

        private RefreshChunkArea DeNetRefreshAreaQueue()
        {
            lock (_netRefreshQueue)
            {
                if (_netRefreshQueue.Count > 0)
                {
                    RefreshChunkArea c = _netRefreshQueue.Dequeue();
                    return c;
                }
                return null;
            }
        }


        public void EnNetGeneratorQueue(NetPriorityChunk priorityChunk)
        {
            lock (_netGeneratorQueue)
            {
                if (_netGeneratorQueue.Count > _netGeneratorQueue.MaxSize) return;
                _netGeneratorQueue.Enqueue(priorityChunk, priorityChunk.Priority);
            }
        }

        private NetPriorityChunk DeNetGeneratorQueue()
        {
            lock (_netGeneratorQueue)
            {
                if (_netGeneratorQueue.Count > 0)
                {
                    return _netGeneratorQueue.Dequeue();
                }
                return null;
            }
        }

        public void UpdateGeneratorChunkPriority(WorldPos curInChunkPos)
        {
            lock (_selfGeneratorQueue)
            {
                IEnumerator<PriorityChunk> enumerator = _selfGeneratorQueue.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    enumerator.Current.Priority = _manager.GetPriority(enumerator.Current.chunk.worldPos, curInChunkPos);
                }
                _selfGeneratorQueue.UpdateAll();
            }
        }

        public void UpdateNetGeneratorChunkPriority(WorldPos curInChunkPos)
        {
            lock (_netGeneratorQueue)
            {
                IEnumerator<NetPriorityChunk> enumerator = _netGeneratorQueue.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    enumerator.Current.Priority = _manager.GetPriority(enumerator.Current.netChunk.chunkPos, curInChunkPos);
                }
                _netGeneratorQueue.UpdateAll();
            }
        }

        private List<Chunk> reculateLightArounds = new List<Chunk>();
        private void ReculateChunkLightAndAround(Chunk chunk, bool reRender = false)
        {
            reculateLightArounds.Clear();
            List<Chunk> list = _manager.World.GetAroundChunks(chunk, reculateLightArounds);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].isPopulationDataPrepared)
                {
                    ReculateChunkLight(list[i]);
                    if (reRender)
                    {
                        _manager.EnqueueRender(list[i]);
                    }
                }
            }
            if (chunk.isPopulationDataPrepared)
            {
                ReculateChunkLight(chunk);
                if (reRender)
                {
                    _manager.EnqueueRender(chunk);
                }
            }
        }

        private List<Chunk> reculateAreaLightArounds = new List<Chunk>();
        private void ReculateChunkAreaLightAndAround(RefreshChunkArea chunkArea, bool reRender = false)
        {
            reculateLightArounds.Clear();
            reculateAreaLightArounds.Clear();
            for (int i = 0; i < chunkArea.chunkList.Count; i++)
            {
                List<Chunk> list = _manager.World.GetAroundChunks(chunkArea.chunkList[i].chunk, reculateLightArounds);
                for (int j = 0; j < list.Count; j++)
                {
                    if (!reculateAreaLightArounds.Contains(list[j]))
                    {
                        reculateAreaLightArounds.Add(list[j]);
                    }
                }
                if (!reculateAreaLightArounds.Contains(chunkArea.chunkList[i].chunk))
                {
                    reculateAreaLightArounds.Add(chunkArea.chunkList[i].chunk);
                }
            }

            for (int i = 0; i < reculateAreaLightArounds.Count; i++)
            {
                ReculateChunkLight(reculateAreaLightArounds[i]);
            }
            if (reRender)
            {
                for (int i = 0; i < reculateAreaLightArounds.Count; i++)
                {
                    _manager.EnqueueRender(reculateAreaLightArounds[i]);
                }
            }
        }

        private void ReculateChunkLight(Chunk chunk)
        {
            chunk.isLightDataPrepared = false;
            chunk.ClearLight();
            _lightControlGenerator.Generate(chunk);
            chunk.isLightDataPrepared = true;
        }

        private List<Chunk> aroundChunk1 = new List<Chunk>();
        //检测当前块周围的块是否能产生生物群落物块（被区块包围着就能产生）
        private void CheckAroundChunkAndGeneratorPopulation(Chunk chunk, bool isNet)
        {
            aroundChunk1.Clear();
            List<Chunk> list = _manager.World.GetAroundChunks(chunk, aroundChunk1);
            for (int i = 0; i < list.Count; i++)
            {
                if (IsCanGeneratorPopulation(list[i]))
                {
                    GeneratorPopulationBlock(list[i], isNet);
                }
            }
            if (IsCanGeneratorPopulation(chunk))
            {
                GeneratorPopulationBlock(chunk, isNet);
            }
        }

        //更新网络大范围更新数据
        private void refreshNetArea()
        {
            RefreshChunkArea netRefreshChunkArea = DeNetRefreshAreaQueue();
            if (netRefreshChunkArea != null)
            {
                for (int i = 0; i < netRefreshChunkArea.chunkList.Count; i++)
                {

                    if (netRefreshChunkArea.chunkList[i].refreshList.Count > 0)
                    {
                        for (int j = 0; j < netRefreshChunkArea.chunkList[i].refreshList.Count; j++)
                        {
                            UpdateBlock updateBlock = netRefreshChunkArea.chunkList[i].refreshList[j];
                            netRefreshChunkArea.chunkList[i].chunk.UpdateChangedBlock(updateBlock);
                        }
                    }
                }
                ReculateChunkAreaLightAndAround(netRefreshChunkArea, true);
            }
        }


        private void GeneratorPopulationBlock(Chunk chunk, bool isNet)
        {
            if (!chunk.isPopulationDataPrepared)
            {
                if (isNet)
                {
                    ClientBlockCollection.Instance.BeginCollection();
					ClientBlockCollection.Instance.StartInChunk (chunk.worldPos);
                    _populationControlGenerator.Generate(chunk);
                    chunk.isPopulationDataPrepared = true;
                    ClientBlockCollection.Instance.SendPackage();
                    ClientBlockCollection.Instance.EndCollection();
                }
                else
                {
                    _populationControlGenerator.Generate(chunk);
                    chunk.isPopulationDataPrepared = true;
                }
            }

            if (!chunk.isLightDataPrepared)
            {
                _lightControlGenerator.Generate(chunk);
                chunk.isLightDataPrepared = true;
            }

            CheckAroundChunkAndStartCalculateMesh(chunk);
        }

        private bool IsCanGeneratorPopulation(Chunk chunk)
        {
            if (!chunk.isTerrainDataPrepared) return false;
            List<Chunk> subList = _manager.World.GetAroundChunks(chunk);
            if (subList.Count < 8) return false;
            int j;
            for (j = 0; j < subList.Count; j++)
            {
                if (!subList[j].isTerrainDataPrepared)
                {
                    break;
                }
            }
            if (j < subList.Count) return false;
            else return true;
        }

        private List<Chunk> aroundChunk2 = new List<Chunk>();
        private void CheckAroundChunkAndStartCalculateMesh(Chunk chunk)
        {
            aroundChunk2.Clear();
            List<Chunk> list = _manager.World.GetAroundChunks(chunk, aroundChunk2);
            for (int i = 0; i < list.Count; i++)
            {
                if (IsCanRender(list[i]))
                {
                    CalculateLightAndRenderMesh(list[i]);
                }
            }
            if (IsCanRender(chunk))
            {
                CalculateLightAndRenderMesh(chunk);
            }
        }

        private void CalculateLightAndRenderMesh(Chunk chunk)
        {
            if (chunk.isLightDataPreparedAndUpdate)
            {
                _lightControlGenerator.SpreadLight(chunk);
                chunk.isLightDataPreparedAndUpdate = false;
            }

            _manager.EnqueueRender(chunk);
        }
        private List<Chunk> aroundChunk3 = new List<Chunk>();
        private bool IsCanRender(Chunk chunk)
        {
            if (!chunk.isPopulationDataPrepared) return false;
            aroundChunk3.Clear();
            List<Chunk> subList = _manager.World.GetAroundChunks(chunk, aroundChunk3);
            if (subList.Count < 8) return false;
            bool isCanRender = true;
            for (int j = 0; j < subList.Count; j++)
            {
                if (!subList[j].isPopulationDataPrepared)
                {
                    isCanRender = false;
                    break;
                }
            }
            return isCanRender;
        }

    }
}

