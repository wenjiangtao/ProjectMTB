using UnityEngine;
using System.Collections;
namespace MTB
{
    public class MonsterAIComponent : BaseMonsterAIComponent
    {
        public MonsterAIComponent()
            : base()
        {
        }

        protected override void initMonsterAIState()
        {
            registerMonsterAIState(new AIFreeState(this));
            registerMonsterAIState(new AIRoamState(this));
            registerMonsterAIState(new AIAimState(this));
            registerMonsterAIState(new AIChaseState(this));
            registerMonsterAIState(new AIPreAttack(this));
            registerMonsterAIState(new AIAttackState(this));
            registerMonsterAIState(new AIRunAwayState(this));
            registerMonsterAIState(new AIIdleState(this));
            registerMonsterAIState(new AIAttractedState(this));
            registerMonsterAIState(new AIBreedState(this));
            runAIState(AIStateType.FREE);
        }
    }
}