using UnityEngine;
using System.Collections;
namespace MTB
{
    public class BatAIComponent : MonsterAIComponent
    {

        public BatAIComponent()
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
            registerMonsterAIState(new DrowningState(this));
            registerMonsterAIState(new AIRunAwayState(this));
            registerMonsterAIState(new AIIdleState(this));
            registerMonsterAIState(new AIAttractedState(this));
            registerMonsterAIState(new AIBreedState(this));
            runAIState(AIStateType.FREE);

        }

        public override void onTick()
        {
            base.onTick();
            //蝙蝠走的时候跳跃
            if (_hostController.movableController.GetVelocity().x != 0 || _hostController.movableController.GetVelocity().y != 0)
            {
                _hostController.Jump();
            }
        }
    }
}
