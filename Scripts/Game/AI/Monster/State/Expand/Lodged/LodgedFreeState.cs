using UnityEngine;
using System.Collections;
namespace MTB
{
    public class LodgedFreeState : BaseMonsterAIState
    {
        private System.Random _random = new System.Random();

        public LodgedFreeState(IAIComponent monsterAIComponent)
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
            GameObject target = _monsterAIComponent.seachTarget();
            if (target != null && getMonsterAIComponent().monsterAIData.initiativeAttack)
            {
                this._monsterAIComponent.setTarget(target);
                return AIStateType.AIM;
            }
            if (_random.Next(100) > 50)
            {
                return AIStateType.IDEL;
            }
            return AIStateType.PREROAM;
        }

        public override AIStateType getType()
        {
            return AIStateType.FREE;
        }
    }
}
