using UnityEngine;
using System.Collections;
namespace MTB
{
    public class DrowningState : BaseMonsterAIState
    {

        public DrowningState(IAIComponent monsterAIComponent)
            : base(monsterAIComponent)
        {
        }

        public override AIStateType onThink()
        {
            if (!isInWater())
            {
                return AIStateType.FREE;
            }
            return AIStateType.NONE;
        }

        public override AIStateType getType()
        {
            return AIStateType.DROWNING;
        }
    }
}
