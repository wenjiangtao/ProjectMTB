/*****
 * 
 * 管理各种manager
 * 
 * ****/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace MTB
{
    public class HasActionObjectManager : Singleton<HasActionObjectManager>
    {
        private Dictionary<HasActionObjectManagerTypes, IHasActionObjectManager> _managerList = new Dictionary<HasActionObjectManagerTypes, IHasActionObjectManager>(new HasActionObjectManagerTypesComparer());
        private PlantManager _plantManager;


        void Awake()
        {
            Init();
        }

        public void Init()
        {
            gameObject.name = "ObjectManager";
            _managerList.Add(HasActionObjectManagerTypes.PLAYER, new PlayerManager(gameObject) as IHasActionObjectManager);
            _managerList.Add(HasActionObjectManagerTypes.MONSTER, new MonsterManager(gameObject) as IHasActionObjectManager);
            _managerList.Add(HasActionObjectManagerTypes.NPC, new NPCManager(gameObject) as IHasActionObjectManager);
        }

        public IHasActionObjectManager getManager(HasActionObjectManagerTypes type)
        {
            return _managerList[type];
        }

        public MonsterManager monsterManager { get { return _managerList[HasActionObjectManagerTypes.MONSTER] as MonsterManager; } }

        public PlayerManager playerManager { get { return _managerList[HasActionObjectManagerTypes.PLAYER] as PlayerManager; } }

        public PlantManager plantManager { get { if (_plantManager == null)_plantManager = new PlantManager(); return _plantManager; } }

        public NPCManager npcManager { get { return _managerList[HasActionObjectManagerTypes.NPC] as NPCManager; } }

//		public List<EntityData> GetEntityDatasInChunk(WorldPos pos)
//		{
//			List<EntityData> list = new List<EntityData>();
//			List<GameObject> listMonster = monsterManager.listObjInChunk(pos);
//			List<GameObject> listNPC = npcManager.listObjInChunk(pos);
//			List<GrowDecoration> listGrowDecoration = Instance.plantManager.listDecorationInChunk(pos);
//			if(listMonster != null)
//			{
//				for (int i = 0; i < listMonster.Count; i++) {
//					EntityData entityData = listMonster[i].GetComponent<GOMonsterController>().GetEntityData();
//					list.Add(entityData);
//				}
//			}
//			if(listNPC != null)
//			{
//				for (int i = 0; i < listNPC.Count; i++) {
//					EntityData entityData = listNPC[i].GetComponent<GONPCController>().GetEntityData();
//					list.Add(entityData);
//				}
//			}
//			if(listGrowDecoration != null)
//			{
//				for (int i = 0; i < listGrowDecoration.Count; i++) {
//					EntityData entityData = listGrowDecoration[i].GetEntityData();
//					list.Add(entityData);
//				}
//			}
//			return list;
//		}

        public GameObject[] getEnemyList(int groupId)
        {
            return null;
        }

        void Update()
        {
            plantManager.updateState(Time.deltaTime);
        }
    }

    public class HasActionObjectManagerTypesComparer : IEqualityComparer<HasActionObjectManagerTypes>
    {
        #region IEqualityComparer implementation
        bool IEqualityComparer<HasActionObjectManagerTypes>.Equals(HasActionObjectManagerTypes a, HasActionObjectManagerTypes b)
        {
            return a == b;
        }
        int IEqualityComparer<HasActionObjectManagerTypes>.GetHashCode(HasActionObjectManagerTypes obj)
        {
            return (int)obj;
        }
        #endregion
    }
}
