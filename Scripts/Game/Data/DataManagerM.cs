using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace MTB
{
    public enum DataTypes
    {
        Basic = 1,
        AI = 2,
        Action = 3,
        Breeding = 4
    }

    public class DataManagerM : Singleton<DataManagerM>
    {
        private Dictionary<GameObjectTypes, IDataManager> MANAGERS = new Dictionary<GameObjectTypes, IDataManager>()
        {
           {GameObjectTypes.MONSTER,new MonsterDataManager()}
        };

        public void Init()
        {
            foreach (IDataManager manager in MANAGERS.Values)
            {
                manager.loadData();
            }
            MTBTaskDataManager.Instance.loadData();
            MTBPlantDataManager.Instance.loadData();
            MTBNPCDataManager.Instance.loadData();
        }

        public IDataManager getManager(GameObjectTypes type)
        {
            if (!MANAGERS.ContainsKey(type))
            {
                Debug.LogError("没有此种类型的data配置！:" + type);
                return null;
            }
            return MANAGERS[type];
        }

        public MonsterDataManager getMonsterDataManager()
        {
            return getManager(GameObjectTypes.MONSTER) as MonsterDataManager;
        }
    }

}
