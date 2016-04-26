using UnityEngine;
using System.Collections;
namespace MTB
{
    public class AIFreeState : BaseMonsterAIState
    {
        public AIFreeState(IAIComponent monsterAIComponent)
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
            return AIStateType.ROAM;
        }

        public override AIStateType getType()
        {
            return AIStateType.FREE;
        }
    }
}
