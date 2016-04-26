using UnityEngine;
using System.Collections;
namespace MTB
{
    public class LodgedAIComponent : MonsterAIComponent
    {

        public LodgedAIComponent()
            : base()
        {
        }

        protected override void initMonsterAIState()
        {
            registerMonsterAIState(new LodgedFreeState(this));
            registerMonsterAIState(new LodgedAimState(this));
            registerMonsterAIState(new LodgedPreAttack(this));
            registerMonsterAIState(new LodgedAttackState(this));
            registerMonsterAIState(new LodgedRoamState(this));
            registerMonsterAIState(new LodgedPreRoamState(this));
            registerMonsterAIState(new LodgedAfterRoamState(this));
            registerMonsterAIState(new AIIdleState(this));
            registerMonsterAIState(new AIRunAwayState(this));
            runAIState(AIStateType.FREE);
        }
    }
}