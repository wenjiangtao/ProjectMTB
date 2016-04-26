using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
namespace MTB
{
    public class Chunk : IDataSerialize, IDisposable
    {
        #region IDataSerialize implementation

        public void Serialize(Stream stream)
        {
            for (int i = 0; i < sections.Length; i++)
            {
                sections[i].Serialize(stream);
            }
            for (int i = 0; i < biomeIdMap.Length; i++)
            {
                Serialization.WriteIntToStream(stream, biomeIdMap[i]);
            }
            for (int i = 0; i < heightMap.Length; i++)
            {
                Serialization.WriteIntToStream(stream, heightMap[i]);
            }
            Serialization.WriteBoolToStream(stream, isTerrainDataPrepared);
            Serialization.WriteBoolToStream(stream, isPopulationDataPrepared);
            Serialization.WriteBoolToStream(stream, isLightDataPreparedAndUpdate);
            Serialization.WriteBoolToStream(stream, isLightDataPrepared);

            Serialization.WriteBoolToStream(stream, haveWater);
            
			Serialization.WriteIntToStream(stream, entities.Count);
			while(entities.Count > 0)
			{
				EntityData entityData = entities.Dequeue();
				entityData.Serialize(stream);
			}
        }

        public void Deserialize(Stream stream)
        {
            for (int i = 0; i < sections.Length; i++)
            {
                sections[i].Deserialize(stream);
            }
            for (int i = 0; i < biomeIdMap.Length; i++)
            {
                biomeIdMap[i] = Serialization.ReadIntFromStream(stream);
            }
            for (int i = 0; i < heightMap.Length; i++)
            {
                heightMap[i] = Serialization.ReadIntFromStream(stream);
            }
            isTerrainDataPrepared = Serialization.ReadBoolFromStream(stream);
            isPopulationDataPrepared = Serialization.ReadBoolFromStream(stream);
            isLightDataPreparedAndUpdate = Serialization.ReadBoolFromStream(stream);
            isLightDataPrepared = Serialization.ReadBoolFromStream(stream);

            haveWater = Serialization.ReadBoolFromStream(stream);

            int entitiesLen = Serialization.ReadIntFromStream(stream);
            for (int i = 0; i < entitiesLen; i++)
            {
                EntityData entidata = new EntityData();
                entidata.Deserialize(stream);
                entities.Enqueue(entidata);
            }
        }

        #endregion

        #region IDisposable implementation

        public void Dispose()
        {
            world = null;
            hasRefreshEntities = false;
            isTerrainDataPrepared = false;
            isPopulationDataPrepared = false;
            isLightDataPrepared = false;
            isGenerated = false;
            isUpdate = false;
            if (chunkObj != null)
            {
                chunkObj.Dispose();
                chunkObj = null;
            }
            for (int i = 0; i < sectionNums; i++)
            {
                sections[i].Dispose();
            }
        }

        #endregion

        private Section[] sections;

        private int[] biomeIdMap;
        private int[] heightMap;

		//虽然实体数据实在线程中产生的，但有个先后顺序，产生当前块，在实例化当前块的实体，所以不需要锁 
		private Queue<EntityData> entities;

        public const int chunkWidth = 16;
        public const int chunkDepth = 16;
        public const int chunkHeight = 256;
        public const int sectionNums = 16;

        public bool isTerrainDataPrepared = false;

        public bool isPopulationDataPrepared = false;
        public bool isLightDataPreparedAndUpdate = false;

        public bool isLightDataPrepared = false;

        public bool isGenerated = false;
        public bool isUpdate = false;

        public bool haveWater = false;

        public bool hasRefreshEntities = false;

        public ChunkObj chunkObj;

        public World world;

        public long chunkSeed { get; private set; }

        private WorldPos _worldPos;
        public WorldPos worldPos
        {
            get
            {
                return _worldPos;
            }
            set
            {
                _worldPos = value;
                chunkSeed = initChunkSeed();
            }
        }

        public Section[] Sections { get { return sections; } }

        public Chunk()
        {
            sections = new Section[sectionNums];
            for (int i = 0; i < sectionNums; i++)
            {
                sections[i] = new Section();
                sections[i].chunk = this;
                sections[i].chunkOffsetY = i * Section.sectionHeight;
            }
            biomeIdMap = new int[chunkWidth * chunkDepth];
            heightMap = new int[chunkWidth * chunkDepth];
            entities = new Queue<EntityData>();
        }

        public void InitData()
        {
            isTerrainDataPrepared = false;
            isPopulationDataPrepared = false;
            isLightDataPrepared = false;
            isGenerated = false;
            isUpdate = false;
            hasRefreshEntities = false;
            for (int i = 0; i < sectionNums; i++)
            {
                sections[i].InitData();
            }
            Array.Clear(biomeIdMap, 0, biomeIdMap.Length);
            Array.Clear(heightMap, 0, heightMap.Length);
            entities.Clear();
        }

        public void ClearLight()
        {
            for (int i = 0; i < sectionNums; i++)
            {
                sections[i].ClearLight();
            }
            Array.Clear(heightMap, 0, heightMap.Length);
        }

        protected long initChunkSeed()
        {
            long seed = WorldConfig.Instance.seed;
            seed *= (seed * 6364136223846793005L + 1442695040888963407L);
            seed += _worldPos.x;
            seed *= (seed * 6364136223846793005L + 1442695040888963407L);
            seed += _worldPos.z;
            seed *= (seed * 6364136223846793005L + 1442695040888963407L);
            seed += _worldPos.x;
            seed *= (seed * 6364136223846793005L + 1442695040888963407L);
            seed += _worldPos.z;
            return seed;
        }

        public int GetSign()
        {
            int sign = 0;
            sign |= (isPopulationDataPrepared ? 1 : 0);
			sign |= (hasRefreshEntities ? 2 : 0);
            return sign;
        }

        public void UpdateSign(int sign)
        {
            isPopulationDataPrepared = ((sign & 1) == 1);
			hasRefreshEntities = ((sign & 2) != 0);
			if(hasRefreshEntities)entities.Clear();
        }

        public void AddEntityData(EntityData data)
        {
			entities.Enqueue(data);
        }

        public int RefreshEntity()
        {
			int num = 0;
            if (!hasRefreshEntities)
            {
				num = entities.Count;
				while(entities.Count > 0)
				{
					EntityData entityData = entities.Dequeue();
					if (entityData.type == EntityType.MONSTER)
					{
						MonsterInfo monsterInfo = new MonsterInfo();
						monsterInfo.aoId = AoIdManager.instance.getAoId();
						monsterInfo.monsterId = entityData.id;
						monsterInfo.position = entityData.pos;
						GameObject monster = HasActionObjectManager.Instance.monsterManager.InitMonster(monsterInfo);
					}
					else if (entityData.type == EntityType.PLANT)
					{
						HasActionObjectManager.Instance.plantManager.buildPlant(entityData.pos, (DecorationType)entityData.id, AoIdManager.instance.getAoId());
					}
					else if (entityData.type == EntityType.NPC)
					{
						NPCInfo info = new NPCInfo();
						info.position = entityData.pos;
						info.NPCId = entityData.id;
						info.aoId = AoIdManager.instance.getAoId();
						info.taskId = entityData.exData[0];
						info.stepId = entityData.exData[1];
						GameObject npc = HasActionObjectManager.Instance.npcManager.InitNPC(info);
					}
				}
                hasRefreshEntities = true;
				isUpdate = true;
            }
			return num;
        }

		public List<EntityData> ClearEntityAndSetRefresh()
		{
			List<EntityData> list = new List<EntityData>();
			if (!hasRefreshEntities)
			{
				while(entities.Count > 0)
				{
					EntityData entityData = entities.Dequeue();
					list.Add(entityData);
				}
				hasRefreshEntities = true;
				isUpdate = true;
			}
			return list;
		}

        public int RemoveEntity()
        {
			if(ResetEntity() > 0)isUpdate = true;
			if (hasRefreshEntities)
			{
				HasActionObjectManager.Instance.monsterManager.RemoveObjInChunk(worldPos);
				HasActionObjectManager.Instance.npcManager.RemoveObjInChunk(worldPos);
				HasActionObjectManager.Instance.plantManager.RemoveDecorationInChunk(worldPos);
				hasRefreshEntities = false;
			}
			return entities.Count;
        }

		public int ResetEntity()
		{
			List<GameObject> listMonster = HasActionObjectManager.Instance.monsterManager.listObjInChunk(worldPos);
			List<GameObject> listNPC = HasActionObjectManager.Instance.npcManager.listObjInChunk(worldPos);
			List<GrowDecoration> listGrowDecoration = HasActionObjectManager.Instance.plantManager.listDecorationInChunk(worldPos);
			if (hasRefreshEntities)
			{
				entities.Clear();
			}
			int num = entities.Count;
			if(listMonster != null)
			{
				for (int i = 0; i < listMonster.Count; i++) {
					EntityData entityData = listMonster[i].GetComponent<GOMonsterController>().GetEntityData();
					entities.Enqueue(entityData);
				}
			}
			if(listNPC != null)
			{
				for (int i = 0; i < listNPC.Count; i++) {
					EntityData entityData = listNPC[i].GetComponent<GONPCController>().GetEntityData();
					entities.Enqueue(entityData);
				}
			}
			if(listGrowDecoration != null)
			{
				for (int i = 0; i < listGrowDecoration.Count; i++) {
					EntityData entityData = listGrowDecoration[i].GetEntityData();
					entities.Enqueue(entityData);
				}
			}
			return entities.Count - num;
		}

        public int GetBiomeId(int x, int z, bool isInRange = false)
        {
            if (!isInRange && !IsInRange(x, z))
            {
                return world.GetBiomeId(worldPos.x + x, worldPos.z + z);
            }
            return biomeIdMap[x + chunkDepth * z];
        }

        public void SetBiomeId(int x, int z, int biomeId, bool isInRange = false)
        {
            if (!isInRange && !IsInRange(x, z))
            {
                world.SetBiomeId(worldPos.x + x, worldPos.z + z, biomeId);
            }
            else
            {
                biomeIdMap[x + chunkDepth * z] = biomeId;
                isUpdate = true;
            }
        }

        public int GetHeight(int x, int z, bool isInRange = false)
        {
            if (!isInRange && !IsInRange(x, z))
            {
                return world.GetHeight(worldPos.x + x, worldPos.z + z);
            }
            return heightMap[x + chunkDepth * z];
        }

        public void SetHeight(int x, int z, int height, bool isInRange = false)
        {
            if (!isInRange && !IsInRange(x, z))
            {
                world.SetHeight(worldPos.x + x, worldPos.z + z, height);
            }
            else
            {
                heightMap[x + chunkDepth * z] = height;
                isUpdate = true;
            }
        }

        public Block GetBlock(int x, int y, int z, bool isInRange = false)
        {
            if (!isInRange && !IsInRange(x, y, z))
            {
                return world.GetBlock(worldPos.x + x, worldPos.y + y, worldPos.z + z);
            }
            Section s = GetSection(y);
            return s.GetBlock(x, y - s.chunkOffsetY, z, true);
        }

        public void SetBlock(int x, int y, int z, Block block, bool isInRange = false)
        {
            if (!isInRange && !IsInRange(x, y, z))
            {
                world.SetBlock(worldPos.x + x, worldPos.y + y, worldPos.z + z, block);
            }
            else
            {
                if (block.BlockType == BlockType.StillWater)
                {
                    haveWater = true;
                }
                Section s = GetSection(y);
                s.SetBlock(x, y - s.chunkOffsetY, z, block, true);
                if (isTerrainDataPrepared && !isPopulationDataPrepared)
                {
                    ClientBlockCollection.Instance.Collection(worldPos, x, y, z, block);
                }


                if (isLightDataPrepared && !isGenerated)
                {
                    isLightDataPreparedAndUpdate = true;
                    //更新高度
                    UpdateSunLightHeight(x, y, z, block);
                }
            }
        }


        public void UpdateClientChangedBlock(ClientChangedBlock changedBlock, bool refresh = false)
        {
            WorldPos pos = ClientChangedChunk.GetChunkPos(changedBlock.index);
            //			Debug.Log("updateClientChangedBlock:" + pos.ToString());
            Section s = GetSection(pos.y);
            Block b = new Block((BlockType)changedBlock.blockType, changedBlock.extendId);
            s.SetBlock(pos.x, pos.y - s.chunkOffsetY, pos.z, b, true);
            if (refresh)
            {
                this.world.CheckAndRecalculateMesh(this, pos.x, pos.y, pos.z, b);
            }
        }

        public void UpdateChangedBlock(UpdateBlock updateBlock)
        {
            Block b = new Block(updateBlock.type, updateBlock.exid);
            this.SetBlock(updateBlock.Pos.x - this.worldPos.x, updateBlock.Pos.y, updateBlock.Pos.z - this.worldPos.z, b);
        }

        private void UpdateSunLightHeight(int x, int y, int z, Block block)
        {
            int height = GetHeight(x, z);
            height = y <= height ? height : y + 1;
            bool isLight = true;
            int sunLightLevel = WorldConfig.Instance.maxLightLevel;
            int ty;
            for (ty = height - 1; ty >= 0; ty--)
            {
                Block b = GetBlock(x, ty, z, true);
                BlockAttributeCalculator calculator = BlockAttributeCalculatorFactory.GetCalculator(b.BlockType);
                int lightDamp = calculator.LightDamp(b.ExtendId);

                if (lightDamp != 0 && isLight)
                {
                    SetHeight(x, z, ty + 1);
                    isLight = false;
                }

                sunLightLevel -= lightDamp;
                if (sunLightLevel < 0) sunLightLevel = 0;
                SetSunLight(x, ty, z, sunLightLevel);
                if (sunLightLevel == 0)
                {
                    break;
                }
            }
            height = ty - 1;
            for (ty = height; ty >= 0; ty--)
            {
                if (GetSunLight(x, ty, z) > 0)
                {
                    SetSunLight(x, ty, z, 0);
                }
                else
                    break;
            }

        }

        public int GetSunLight(int x, int y, int z, bool isInRange = false)
        {
            if (!isInRange && !IsInRange(x, y, z))
            {
                return world.GetSunLight(worldPos.x + x, worldPos.y + y, worldPos.z + z);
            }
            Section s = GetSection(y);
            return s.GetSunLight(x, y - s.chunkOffsetY, z, true);
        }

        public void SetSunLight(int x, int y, int z, int value, bool isInRange = false)
        {
            if (!isInRange && !IsInRange(x, y, z))
            {
                world.SetSunLight(worldPos.x + x, worldPos.y + y, worldPos.z + z, value);
            }
            else
            {
                Section s = GetSection(y);
                s.SetSunLight(x, y - s.chunkOffsetY, z, value, true);
            }
        }

        public int GetBlockLight(int x, int y, int z, bool isInRange = false)
        {
            if (!isInRange && !IsInRange(x, y, z))
            {
                return world.GetBlockLight(worldPos.x + x, worldPos.y + y, worldPos.z + z);
            }
            Section s = GetSection(y);
            return s.GetBlockLight(x, y - s.chunkOffsetY, z, true);
        }

        public void SetBlockLight(int x, int y, int z, int value, bool isInRange = false)
        {
            if (!isInRange && !IsInRange(x, y, z))
            {
                world.SetBlockLight(worldPos.x + x, worldPos.y + y, worldPos.z + z, value);
            }
            else
            {
                Section s = GetSection(y);
                s.SetBlockLight(x, y - s.chunkOffsetY, z, value, true);
            }
        }

        private bool IsInRange(int x, int y, int z)
        {
            if (x < 0 || x >= chunkWidth || y < 0 || y >= chunkHeight || z < 0 || z >= chunkDepth)
            {
                return false;
            }
            return true;
        }

        private bool IsInRange(int x, int z)
        {
            if (x < 0 || x >= chunkWidth || z < 0 || z >= chunkDepth)
            {
                return false;
            }
            return true;
        }


        public List<Section> GetAroundSection(Section section)
        {
            List<Section> aroundSections = new List<Section>();
            int index = section.chunkOffsetY / Section.sectionHeight;
            if (index + 1 < sections.Length)
            {
                aroundSections.Add(sections[index + 1]);
            }

            Section right = GetSection(Chunk.chunkWidth, section.chunkOffsetY, 0);
            if (right != null)
            {
                aroundSections.Add(right);
            }

            Section front = GetSection(0, section.chunkOffsetY, Chunk.chunkDepth);
            if (front != null)
            {
                aroundSections.Add(front);
            }

            return aroundSections;
        }

        public List<Section> GetAroundSection(Section section, List<Section> aroundSections)
        {
            int index = section.chunkOffsetY / Section.sectionHeight;
            if (index + 1 < sections.Length)
            {
                aroundSections.Add(sections[index + 1]);
            }

            Section right = GetSection(Chunk.chunkWidth, section.chunkOffsetY, 0);
            if (right != null)
            {
                aroundSections.Add(right);
            }

            Section front = GetSection(0, section.chunkOffsetY, Chunk.chunkDepth);
            if (front != null)
            {
                aroundSections.Add(front);
            }

            return aroundSections;
        }


        public Section GetSection(int x, int y, int z)
        {
            if (!IsInRange(x, y, z))
            {
                return world.GetSection(worldPos.x + x, worldPos.y + y, worldPos.z + z);
            }
            return GetSection(y);
        }

        private Section GetSection(int y)
        {
            int index = y / Section.sectionHeight;
            return sections[index];
        }

        public override bool Equals(object obj)
        {
            if (obj is Chunk)
            {
                Chunk other = (Chunk)obj;
                return worldPos.EqualOther(other.worldPos);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return worldPos.GetHashCode();
        }
    }


}
