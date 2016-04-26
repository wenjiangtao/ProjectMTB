using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MTB
{
    public class BaseAIComponent : IAIComponent, IAIStateManager, IAIContext
    {
        protected GameObject _host;

        protected GameObject _target;

        //protected Object _context;

        protected GameObjectController _hostController;

        protected IAIState _curAIState;

        protected ISearchComponent _searchComponent;

        protected IData _aiData;

        protected GameObjectTypes _attTargetType = GameObjectTypes.MONSTER;

        protected Dictionary<AIStateType, IAIState> _stateMap = new Dictionary<AIStateType, IAIState>();

        protected Dictionary<int, int> _mapAoId2Enmity = new Dictionary<int, int>();

        protected Dictionary<int, int> _mapGroupId2Enmity = new Dictionary<int, int>();

        protected int _maxEnmity = 0;

        protected int _searchRadius = 1;

        protected bool _isRunning = false;
        //protected GameObjectType[] _listFilterAttackTypes = [];

        public BaseAIComponent()
        {
        }

        protected void registerAIState(AIStateType key, IAIState aiState)
        {
            if (this._stateMap.ContainsKey(key))
            {
                Debug.LogError("registerAIState():已经存在对应的key:" + key);
                return;
            }
            this._stateMap.Add(key, aiState);
        }

        protected virtual void initAIData()
        {
            Debug.LogError("initAIData方法必须被重写！");
        }

        public virtual void runAIState(AIStateType key)
        {
            if (_curAIState != null)
            {
                this._curAIState.stateOut();
            }
            this._curAIState = _stateMap[key] as IAIState;
            if (this._curAIState == null)
            {
                Debug.LogError("无法找到对应的AI类型" + key);
                return;
            }
            this._curAIState.stateIn();
        }

        public IAIState getCurrentState()
        {
            return this._curAIState;
        }

        public void setTarget(GameObject value)
        {
            _target = value;
        }

        public GameObject getTarget()
        {
            return _target;
        }

        //广播场景战斗事件,例如受击攻击,暂时不管
        public void onReceiveSceneFightMsg()
        { }

        public virtual void attach(GameObject haveActionObj)
        {
            this._host = haveActionObj;
            if (_searchComponent != null)
                _searchComponent.dispose();
            _hostController = this._host.GetComponent<GameObjectController>();
            initAIData();
        }

        public bool isRunning()
        {
            return this._isRunning;
        }

        public virtual void run()
        {
            this._isRunning = true;
        }

        public virtual void pause()
        {
            this._isRunning = false;
        }

        public GameObject host()
        {
            return this._host;
        }

        public IData getAIDate()
        {
            return _aiData;
        }

        public virtual void onTick()
        {
            if (!this._isRunning)
                return;
            if (this._curAIState != null)
            {
                AIStateType nextStateKey = this._curAIState.onThink();
                if (nextStateKey != AIStateType.NONE)
                {
                    runAIState(nextStateKey);
                }
            }
        }

        public void setAttTargetType(GameObjectTypes value)
        {
            _attTargetType = value;
        }

        public int getMaxEnmity()
        {
            return _maxEnmity;
        }

        public void updateEnmityByFoId(int aoId, int enmity)
        {
            _mapAoId2Enmity.Add(aoId, enmity);
            updateMaxEnmity(_mapAoId2Enmity);
        }

        public void updateEnmityByGroupId(int groupId, int enmity)
        {
            _mapGroupId2Enmity.Add(groupId, enmity);
            updateMaxEnmity(_mapGroupId2Enmity);
        }

        private void updateMaxEnmity(Dictionary<int, int> listEnmitys)
        {
            _maxEnmity = 0;
            foreach (int value in listEnmitys.Values)
            {
                if (_maxEnmity < value)
                    _maxEnmity = value;
            }
        }

        public virtual GameObject seachTarget(bool checkHeight = true)
        {
            return _searchComponent.searchTarget(20, 0, 2);
        }

        private int getTargetEnmiity(GameObject target)
        {
            int enmity = _mapAoId2Enmity[target.GetComponent<BaseAttributes>().aoId];
            if (_mapGroupId2Enmity.ContainsKey(target.GetComponent<BaseAttributes>().groupId) && enmity < _mapGroupId2Enmity[target.GetComponent<BaseAttributes>().groupId])
                enmity = _mapGroupId2Enmity[target.GetComponent<BaseAttributes>().groupId];
            return enmity;
        }

        public GameObject[] listArrTarget()
        {
            GameObject[] listEnemys = HasActionObjectManager.Instance.getEnemyList(_host.GetComponent<BaseAttributes>().groupId);
            List<GameObject> listFilterEnemys = new List<GameObject>();
            if (listEnemys != null)
            {
                foreach (GameObject enemy in listEnemys)
                {
                    int enemyEnmity = getTargetEnmiity(enemy);
                    if (enemyEnmity == _maxEnmity)
                    {
                        listFilterEnemys.Add(enemy);
                    }
                }
            }
            return listFilterEnemys.ToArray();
        }

        public bool isAttachTargetType(int type)
        {
            return true;
        }

        public void dispose()
        {
            if (_searchComponent != null)
            {
                _searchComponent.dispose();
            }
            this._host = null;
            this._curAIState = null;
            foreach (IAIState state in _stateMap.Values)
            {
                state.dispose();
            }
            this._stateMap = null;
            this._target = null;
        }
    }
}
