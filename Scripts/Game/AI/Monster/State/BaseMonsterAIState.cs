using UnityEngine;
using System;
using System.Collections;
namespace MTB
{
    public class BaseMonsterAIState : IMonsterAIState
    {
        protected IAIComponent _monsterAIComponent;
        protected IAIContext _aiContext;
        protected IAIStateManager _aiStateManager;
        protected AIStateType _checkTargetType;
        protected int _decoyItem;
        protected int _attactedDis;
        protected GameObject _host;

        public BaseMonsterAIState(IAIComponent monsterAIComponent)
        {
            this._monsterAIComponent = monsterAIComponent;
            this._aiContext = monsterAIComponent as IAIContext;
            this._aiStateManager = monsterAIComponent as IAIStateManager;
            this._host = monsterAIComponent.host();
            init();
        }

        protected virtual void init()
        {
            //todo 监听受击事件
            _decoyItem = DataManagerM.Instance.getMonsterDataManager().getBreedDate(_host).decoyItem;
            _attactedDis = DataManagerM.Instance.getMonsterDataManager().getBreedDate(_host).decoyDis;
            registerEvt();
        }

        public virtual AIStateType getType()
        {
            Debug.LogError("getType()：必须被重写");
            return AIStateType.IDEL;
        }

        protected virtual void registerEvt()
        {
            _host.GetComponent<GameObjectController>().On_BeAttackStart += onMonsterBeHit;
            if (_decoyItem != 0)
                _host.GetComponent<GOMonsterController>().On_BeFeed += onMonsteBeFeed;

        }

        protected virtual void unRegisterEvt()
        {
            _host.GetComponent<GameObjectController>().On_BeAttackStart -= onMonsterBeHit;
            if (_decoyItem != 0)
                _host.GetComponent<GOMonsterController>().On_BeFeed -= onMonsteBeFeed;
        }

        void onMonsterBeHit()
        {
            if (getType() == AIStateType.AIM || getType() == AIStateType.ATTACK || getType() == AIStateType.PREATTACK || getType() == AIStateType.CHASE)
            {
                return;
            }
            GameObject target = _monsterAIComponent.seachTarget();
            if (target != null)
            {
                this._monsterAIComponent.setTarget(target);
                if (getMonsterAIComponent().monsterAIData.counterAttack)
                {
                    this._aiStateManager.runAIState(AIStateType.AIM);
                }
                else
                {
                    this._aiStateManager.runAIState(AIStateType.RUNAWAY);
                }
            }
        }

        void onMonsteBeFeed()
        {
            GameObject target = _monsterAIComponent.seachTarget();
            if (target != null)
            {
                this._monsterAIComponent.setTarget(target);
                this._aiStateManager.runAIState(AIStateType.BREED);
            }
        }

        protected virtual bool isInBound(GameObject target, int minBound, int maxBound)
        {
            if (minBound < 0 && maxBound < 0)
                return true;
            float dis = Vector3.Distance(target.transform.position, _host.transform.position);
            if (dis < minBound || dis > maxBound)
                return false;

            return true;
        }

        public virtual void stateIn() { }

        public virtual void stateOut() { }

        public virtual AIStateType onThink()
        {
            return AIStateType.NONE;
        }

        //搜寻目标的时候判断手里是否拿着引诱物品以及是否可以被引诱
        protected AIStateType checkTarget()
        {
            GameObject target = _monsterAIComponent.seachTarget();
            if (target != null)
            {
                this._monsterAIComponent.setTarget(target);
                if (getMonsterAIComponent().monsterAIData.initiativeAttack)
                    return AIStateType.AIM;
                if (_decoyItem != 0
                    && target.GetComponent<PlayerAttributes>().handMaterialId == _decoyItem
                    && Vector3.Distance(target.transform.position, _host.transform.position) <= _attactedDis)
                    return AIStateType.ATTRACTED;
            }
            return AIStateType.NONE;
        }

        protected bool isInWater()
        {
            if (getMonsterAIComponent().hostController().gameObjectState.InBlock.BlockType == BlockType.StillWater || getMonsterAIComponent().hostController().gameObjectState.InBlock.BlockType == BlockType.FlowingWater ||
                getMonsterAIComponent().hostController().gameObjectState.StandBlock.BlockType == BlockType.StillWater || getMonsterAIComponent().hostController().gameObjectState.StandBlock.BlockType == BlockType.FlowingWater
                )
            {
                return true;
            }
            return false;
        }

        public IAIComponent getAIComponent()
        {
            return this._monsterAIComponent as IAIComponent;
        }

        public BaseMonsterAIComponent getMonsterAIComponent()
        {
            return _monsterAIComponent as BaseMonsterAIComponent;
        }

        public IAIContext getAIContext()
        {
            return this._aiContext;
        }

        public virtual void dispose()
        {
            unRegisterEvt();
            this._monsterAIComponent = null;
            this._aiContext = null;
            this._aiStateManager = null;
            this._host = null;
        }
    }
}
