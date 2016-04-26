using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace MTB
{
    public class PlantManager
    {
        private Dictionary<int, GrowDecoration> _plantMap;
		private Dictionary<WorldPos,List<GrowDecoration>> _mapInChunk;
        private IMTBRandom _random;
        private float _updateTime = 0;
        private List<int> _plantMapBuff;

        public PlantManager()
        {
            _random = new MTBRandom();
            _plantMap = new Dictionary<int, GrowDecoration>(new PlantComparer());
			_mapInChunk = new Dictionary<WorldPos, List<GrowDecoration>>(new WorldPosComparer());
        }

        ////把植物种植到场景中
        public void buildPlant(Vector3 position, DecorationType type = DecorationType.FaceTree, int aoId = 0,float growTime = 0)
        {
            MTBPlantData data = MTBPlantDataManager.Instance.getData((int)type);
            GrowDecorationParam paras = new GrowDecorationParam(data);
            if (aoId == 0)
                paras.aoId = AoIdManager.instance.getAoId();
            else
                paras.aoId = aoId;
            paras.growedTime = 0;
            paras.pos = position;
            paras.random = _random;
            int x = ((int)position.x);
            int z = ((int)position.z);
            int localx = x % Chunk.chunkWidth;
            int localz = z % Chunk.chunkDepth;
            localx = localx < 0 ? localx + Chunk.chunkWidth : localx;
            localz = localz < 0 ? localz + Chunk.chunkDepth : localz;
            Chunk chunk = World.world.GetChunk(x, (int)position.y, z);
            for (int y = (int)position.y + 5; y >= (int)position.y - 5; y--)
            {
                BlockType curType = chunk.GetBlock(localx, y, localz).BlockType;
                if (curType != BlockType.Air && curType != BlockType.StillWater)
                {
                    paras.loacalPos = new Vector3(localx, y + 1, localz);
                    GrowDecoration decoration = new GrowDecoration(paras);
                    World.world.CheckAndRecalculateMesh(x, y, z, World.world.GetBlock(x, y, z));
                    _plantMap.Add(paras.aoId, decoration);
					AddToChunkMap(decoration);
                    return;
                }
            }
        }

		public List<GrowDecoration> listDecorationInChunk(WorldPos chunkPos)
		{
			List<GrowDecoration> list;
			_mapInChunk.TryGetValue(chunkPos,out list);
			return list;
		}

		public void RemoveDecorationInChunk(WorldPos chunkPos)
		{
			List<GrowDecoration> list;
			_mapInChunk.TryGetValue(chunkPos,out list);
			if(list != null)
			{
				for (int i = 0; i < list.Count; i++) {
					removePlant(list[i].plantAoId);
				}
			}
		}

		private void AddToChunkMap(GrowDecoration decoration)
		{
			List<GrowDecoration> list;
			_mapInChunk.TryGetValue(decoration.ChunkPos,out list);
			if(list == null)
			{
				list = new List<GrowDecoration>();
				_mapInChunk.Add(decoration.ChunkPos,list);
			}
			list.Add(decoration);
		}

		private void RemoveFromChunkMap(GrowDecoration decoration)
		{
			List<GrowDecoration> list;
			_mapInChunk.TryGetValue(decoration.ChunkPos,out list);
			if(list != null)
			{
				list.Remove(decoration);
				if(list.Count <= 0)_mapInChunk.Remove(decoration.ChunkPos);
			}
		}

        public int checkIsPlantSeedling(int materialId)
        {
            if (PlantConfig.SeedlingList.ContainsKey(materialId))
                return (int)PlantConfig.SeedlingList[materialId];
            return -1;
        }

        public void updateState(float deltatime)
        {
            if (DayNightTime.Instance.TimeSlot != DayTimeSlot.Day)
                return;
            if (_updateTime >= 1)
            {
                _updateTime = 0;
                if (_plantMap.Count <= 0)
                    return;
                _plantMapBuff = new List<int>(_plantMap.Keys);
                foreach (int key in _plantMapBuff)
                {
                    _plantMap[key].updateState();
                }
            }
            _updateTime += deltatime;
        }

        public void endGrow(int plantAoId)
        {
            if (_plantMap.ContainsKey(plantAoId))
				RemoveFromChunkMap(_plantMap[plantAoId]);
                _plantMap.Remove(plantAoId);
        }

        public void pauseGrow(int plantAoId)
        {
            if (_plantMap.ContainsKey(plantAoId))
            {
                _plantMap[plantAoId].pauseGrow();
            }
        }

        public void resumeGrow(int plantAoId)
        {
            if (_plantMap.ContainsKey(plantAoId))
                _plantMap[plantAoId].resumeGrow();
        }

        /**
         * 从场景中移除植物,可在chunk中保存数据
         * **/
        public void removePlant(int plantAoId)
        {
            if (_plantMap.ContainsKey(plantAoId))
            {
				RemoveFromChunkMap(_plantMap[plantAoId]);
                _plantMap[plantAoId].remove();
                _plantMap.Remove(plantAoId);
            }
        }
    }

    public class PlantComparer : IEqualityComparer<int>
    {
        #region IEqualityComparer implementation
        bool IEqualityComparer<int>.Equals(int a, int b)
        {
            return a == b;
        }
        int IEqualityComparer<int>.GetHashCode(int obj)
        {
            return (int)obj;
        }
        #endregion
    }
}
