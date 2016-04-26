using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
namespace MTB
{
    public class MonsterManager : BaseHasActionObjectManager
    {
        private bool _isStopAI = false;
        private int _defaultAITargetFoId = -1;
        private int _defaultAITargetGroupId = -1;

        private Dictionary<int, GameObject> _resObjMap;

		private Dictionary<WorldPos,MonsterInfo> _delayRefreshMonster;

        public MonsterManager(GameObject parent)
            : base(parent)
        {
            _resObjMap = new Dictionary<int, GameObject>(new MonsterComparer());
			_delayRefreshMonster = new Dictionary<WorldPos, MonsterInfo>(new WorldPosComparer());
			EventManager.RegisterEvent(EventMacro.CHUNK_GENERATE_FINISH,OnChunkGeneratedFinish);
        }

		private void OnChunkGeneratedFinish(object[] param)
		{
			Chunk chunk = param[0] as Chunk;
			MonsterInfo info;
			_delayRefreshMonster.TryGetValue(chunk.worldPos,out info);
			if(info != null)
			{
				_delayRefreshMonster.Remove(chunk.worldPos);
				NetManager.Instance.client.DelayRefreshEntity(info.aoId);
			}
		}

		public void ChangeObjNet(int aoId,bool isNetObj)
		{
			GameObject obj = getObjById(aoId);
			if(obj == null)
			{
				foreach (var item in _delayRefreshMonster) {
					item.Value.isNetObj = isNetObj;
				}
			}
			else
			{
				obj.GetComponent<GOMonsterController>().ChangeNetObj(isNetObj);
			}
		}

		public override void dispose ()
		{
			EventManager.UnRegisterEvent(EventMacro.CHUNK_GENERATE_FINISH,OnChunkGeneratedFinish);
		}

		public GameObject InitMonster(MonsterInfo info)
		{
			if(info.isNetObj)
			{
				WorldPos pos = Terrain.GetChunkPos(info.position);
				Chunk chunk = World.world.GetChunk(pos.x,pos.y,pos.z);
				if(chunk == null || !chunk.isGenerated)
				{
					if(!_delayRefreshMonster.ContainsKey(pos))
					{
						_delayRefreshMonster.Add(pos,info);
					}
					return null;
				}
			}
			return InitMonsterSelf(info);
		}

		private GameObject InitMonsterSelf(MonsterInfo info)
		{
			MonsterBasicData monsterdata = DataManagerM.Instance.getManager(GameObjectTypes.MONSTER).getData(info.monsterId, DataTypes.Basic) as MonsterBasicData;
			string resPath = monsterdata.respath;
			if (resPath == null)
			{
				throw new Exception("当前playertype有误或者没有配置对应资源");
			}
			GameObject prefab;
			_resObjMap.TryGetValue(info.monsterId, out prefab);
			if (prefab == null)
			{
				prefab = Resources.Load(resPath) as GameObject;
				_resObjMap.Add(info.monsterId, prefab);
			}
			
			GameObject monster = GameObject.Instantiate(prefab);
			monster.transform.position = info.position;
			if (monster.GetComponent<AutoMoveController>() == null)
			{
				monster.AddComponent<AutoMoveController>();
			}
			if (monster.GetComponent<AIContainer>() == null)
			{
				monster.AddComponent<AIContainer>();
			}
			addObj(monster,info);
			return monster;
		}

        public void stopAllMonsterAI()
        {
            foreach (GameObject monster in _listObj.ToArray())
            {
                if (monster.GetComponent<AIContainer>().AIComponent != null)
                    monster.GetComponent<AIContainer>().AIComponent.pause();
            }
        }

        public void startAllMonsterAI()
        {
            foreach (GameObject monster in _listObj.ToArray())
            {
                if (monster.GetComponent<AIContainer>().AIComponent != null)
                    monster.GetComponent<AIContainer>().AIComponent.run();
            }
        }

        public GameObject[] getListMonsters()
        {
            return _listObj.ToArray();
        }


        public void setDefaultAITarget(int foId, int groupId)
        {
            if (_defaultAITargetFoId != foId)
            {
                if (_defaultAITargetFoId != -1)
                {
                    updateCurrentMonstersAIEnmityByFoId(_defaultAITargetFoId, _defaultAITargetGroupId, MonsterAIEnmity.DEFAULT_ENIMITY);
                }
                _defaultAITargetFoId = foId;
                _defaultAITargetGroupId = groupId;
                updateCurrentMonstersAIEnmityByFoId(_defaultAITargetFoId, _defaultAITargetGroupId, MonsterAIEnmity.DEFAULT_TARGET_ENIMITY);
            }
        }

        private void updateCurrentMonstersAIEnmityByFoId(int foId, int groupId, MonsterAIEnmity enmity)
        {
            foreach (GameObject monster in _listObj)
            {
                if (monster.GetComponent<AIContainer>().AIComponent != null && monster.GetComponent<BaseAttributes>().groupId != groupId)
                {
                    monster.GetComponent<AIContainer>().AIComponent.updateEnmityByFoId(foId, (Int32)Enum.Parse(typeof(MonsterAIEnmity), enmity.ToString()));
                }
            }
        }
    }

    public class MonsterComparer : IEqualityComparer<int>
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
