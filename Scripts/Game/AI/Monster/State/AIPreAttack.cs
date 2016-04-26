using UnityEngine;
using System.Collections;
namespace MTB
{
    public class AIPreAttack : BaseMonsterAIState
    {

        private int _preAttackTime;

        public AIPreAttack(IAIComponent monsterAIComponent)
            : base(monsterAIComponent)
        {
        }

        public override void stateIn()
        {
            base.stateIn();
            this._preAttackTime = getMonsterAIComponent().monsterAIData.preAttackTime;
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
            _preAttackTime--;
            if (_preAttackTime <= 0)
            {
                return AIStateType.ATTACK;
            }
            return AIStateType.NONE;
        }

        public override AIStateType getType()
        {
            return AIStateType.PREATTACK;
        }
    }
}
