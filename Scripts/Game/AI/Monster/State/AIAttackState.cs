using UnityEngine;
using System.Collections;
namespace MTB
{
    public class AIAttackState : BaseMonsterAIState
    {
        private int _attackTime;

        public AIAttackState(IAIComponent monsterAIComponent)
            : base(monsterAIComponent)
        {
        }

        public override void stateIn()
        {
            base.stateIn();
            _attackTime = getMonsterAIComponent().monsterAIData.attackTime;
            getMonsterAIComponent().hostController().DoAction(DataManagerM.Instance.getMonsterDataManager().getActionData(_host).attackAction);
        }

        public override AIStateType onThink()
        {
            if (getMonsterAIComponent().hasState(AIStateType.DROWNING) && isInWater())
            {
                return AIStateType.DROWNING;
            }
            _attackTime--;
            if (_attackTime <= 0)
            {
                return AIStateType.FREE;
            }
            return AIStateType.NONE;
        }

        public override AIStateType getType()
        {
            return AIStateType.ATTACK;
        }
    }
}
