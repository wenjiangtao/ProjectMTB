using UnityEngine;
using System.Collections;
namespace MTB
{
    public class LodgedPreAttack : BaseMonsterAIState
    {

        private int _preAttackTime;

        public LodgedPreAttack(IAIComponent monsterAIComponent)
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
