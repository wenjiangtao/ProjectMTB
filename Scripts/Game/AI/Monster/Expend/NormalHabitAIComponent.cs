using UnityEngine;
using System.Collections;
namespace MTB
{
    public class NormalHabitAIComponent : MonsterAIComponent
    {
        public NormalHabitAIComponent()
            : base()
        {
        }

        protected override void initMonsterAIState()
        {
            registerMonsterAIState(new HabitFreeState(this));
            registerMonsterAIState(new AIRoamState(this));
            registerMonsterAIState(new AIAimState(this));
            registerMonsterAIState(new AIChaseState(this));
            registerMonsterAIState(new AIPreAttack(this));
            registerMonsterAIState(new AIAttackState(this));
            registerMonsterAIState(new AIHabitState(this));
            registerMonsterAIState(new AIRunAwayState(this));
            registerMonsterAIState(new AIIdleState(this));
            registerMonsterAIState(new AIAttractedState(this));
            registerMonsterAIState(new AIBreedState(this));
            runAIState(AIStateType.FREE);
        }
    }
}
