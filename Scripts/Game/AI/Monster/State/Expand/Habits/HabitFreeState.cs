using UnityEngine;
using System.Collections;
namespace MTB
{
    public class HabitFreeState : BaseMonsterAIState
    {
        private int _habitRate = 50;
        protected System.Random _random = new System.Random(1);
        public HabitFreeState(IAIComponent monsterAIComponent)
            : base(monsterAIComponent)
        {
        }

        public override void stateIn()
        {
            base.stateIn();
            this._monsterAIComponent.setTarget(null);
            this._host.GetComponent<AutoMoveController>().cancelAutoMove();
        }

        public override void stateOut()
        {
            base.stateOut();
        }

        public override AIStateType onThink()
        {
            if (getMonsterAIComponent().hasState(AIStateType.DROWNING) && isInWater())
            {
                return AIStateType.DROWNING;
            }
            GameObject target = _monsterAIComponent.seachTarget();
            if (target != null && getMonsterAIComponent().monsterAIData.initiativeAttack)
            {
                this._monsterAIComponent.setTarget(target);
                return AIStateType.AIM;
            }
            if (_random.Next(100) > _habitRate)
            {
                return AIStateType.ROAM;
            }
            else
            {
                return AIStateType.HABIT;
            }
        }

        public override AIStateType getType()
        {
            return AIStateType.FREE;
        }
    }
}
