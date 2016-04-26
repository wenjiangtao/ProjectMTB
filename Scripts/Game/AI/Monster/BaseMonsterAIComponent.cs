using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
namespace MTB
{
    public class BaseMonsterAIComponent : BaseAIComponent
    {
        protected bool _isInited;
        protected int _monsterId;
        private AIStateType[] _needTargetAIState = { AIStateType.AIM, AIStateType.CHASE, AIStateType.PREATTACK };

        public BaseMonsterAIComponent()
            : base()
        {
        }

        public override void attach(GameObject haveActionObj)
        {
            base.attach(haveActionObj);
            _monsterId = haveActionObj.GetComponent<BaseAttributes>().objectId;
            _searchComponent = SearchComponentFactory.createSearchComponent(SearchComponentType.Player_Search, _host);
            System.Random random = new System.Random();
            float startfaceDirx = random.Next(2) - 1;
            float startfaceDirz = random.Next(2) - 1;
            _host.GetComponent<AutoMoveController>().faceDir(new Vector2(startfaceDirx, startfaceDirz));
        }

        public override GameObject seachTarget(bool checkHeight = true)
        {
            return _searchComponent.searchTarget(monsterAIData.sightDis, 0, 2);
        }

        public override void run()
        {
            if (!this._isInited)
            {
                initMonsterAIState();
                this._isInited = true;
            }
            base.run();
        }

        public override void pause()
        {
            if (isRunning())
            {
                base.pause();
                runAIState(AIStateType.FREE);
            }
        }

        public override void runAIState(AIStateType key)
        {
            if (canRunAIState(key))
            {
                base.runAIState(key);
            }
            else
            {
                base.runAIState(AIStateType.FREE);
            }
        }

        public override void onTick()
        {
            base.onTick();
            if (_hostController.gameObjectState.InBlock.BlockType == BlockType.StillWater || _hostController.gameObjectState.InBlock.BlockType == BlockType.FlowingWater)
            {
                _hostController.Jump();
            }
        }

        protected virtual void initMonsterAIState()
        {
            Debug.LogError("子类必须重写initMonsterAIState方法！");
        }

        protected virtual void registerMonsterAIState(IMonsterAIState monsterAIState)
        {
            registerAIState(monsterAIState.getType(), monsterAIState);
        }

        protected virtual bool canRunAIState(AIStateType key)
        {
            foreach (AIStateType type in _needTargetAIState)
            {
                if (key == type)
                    return this._target != null;
            }
            return true;
        }

        protected override void initAIData()
        {
            monsterAIData = DataManagerM.Instance.getMonsterDataManager().getAIData(_host);
            if (monsterAIData == null)
            {
                Debug.LogError("找不到的AI数据!");
                return;
            }
        }

        public MonsterAIData monsterAIData
        {
            get
            {
                return _aiData as MonsterAIData;
            }
            set
            {
                _aiData = value;
            }
        }

        public bool hasState(AIStateType type)
        {
            return _stateMap.ContainsKey(type);
        }

        public GameObjectController hostController()
        {
            return _hostController;
        }
    }
}
