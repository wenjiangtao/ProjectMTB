using UnityEngine;
using System.Collections;
namespace MTB
{
    public class AIAimState : BaseMonsterAIState
    {
        public AIAimState(IAIComponent monsterAIComponent)
            : base(monsterAIComponent)
        {
        }

        public override AIStateType onThink()
        {
            if (getMonsterAIComponent().hasState(AIStateType.DROWNING) && isInWater())
            {
                return AIStateType.DROWNING;
            }
            if (this._monsterAIComponent.getTarget() == null)
            {
                Debug.LogError("AIM状态下没有目标!");
                return AIStateType.FREE;
            }

            Vector3 targetPoint = this._monsterAIComponent.getTarget().transform.position;
            Vector3 hostPoint = this._host.transform.position;

            if (Vector3.Distance(targetPoint, hostPoint) > getMonsterAIComponent().monsterAIData.sightDis)
            {
                return AIStateType.FREE;
            }
            return AIStateType.CHASE;
        }

        public override AIStateType getType()
        {
            return AIStateType.AIM;
        }
    }
}
