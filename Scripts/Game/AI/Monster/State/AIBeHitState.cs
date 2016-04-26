using UnityEngine;
using System.Collections;
namespace MTB
{
    public class AIBeHitState : BaseMonsterAIState
    {
        protected bool _beHitOver;

        public AIBeHitState(IAIComponent monsterAIComponent)
            : base(monsterAIComponent)
        {
        }

        protected override void init()
        {
        }

        public override void stateIn()
        {
            _host.GetComponent<GameObjectController>().On_BeAttackEnd += onBehitRecover;
            _beHitOver = false;
            base.stateIn();
            //DoBeHitActoin
            //todo 监听退出受击事件
        }

        public override void stateOut()
        {
			_host.GetComponent<GameObjectController>().On_BeAttackEnd -= onBehitRecover;
            base.stateOut();
            //tode 撤销退出受击监听
        }

        void onBehitRecover()
        {
            _beHitOver = true;
        }

        public override AIStateType onThink()
        {
            if (getMonsterAIComponent().hasState(AIStateType.DROWNING) && isInWater())
            {
                return AIStateType.DROWNING;
            }
            if (_beHitOver)
            {
                if (getMonsterAIComponent().monsterAIData.counterAttack)
                {
                    GameObject target = _monsterAIComponent.seachTarget();
                    if (target != null)
                    {
                        this._monsterAIComponent.setTarget(target);
                        return AIStateType.AIM;
                    }
                }
                return AIStateType.RUNAWAY;
            }
            return AIStateType.NONE;
        }

        public override AIStateType getType()
        {
            return AIStateType.BEHIT;
        }
    }
}
