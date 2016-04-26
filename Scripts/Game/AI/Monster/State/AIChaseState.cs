/******************
 * 追踪状态
 * ****************/
using UnityEngine;
using System.Collections;
namespace MTB
{
    public class AIChaseState : BaseMonsterAIState
    {
        private static int CHASE_MIN = 1;
        private static int CHASE_MAX = 2;
        private IChaseComponent _chaseComponent;

        public AIChaseState(IAIComponent monsterAIComponent)
            : base(monsterAIComponent)
        {
        }

        protected override void init()
        {
            base.init();
            _chaseComponent = ChaseComponentFactory.createCharseComponent(ChaseComponentType.SMART_CHASE, this._host);
        }

        public override void stateIn()
        {
            base.stateIn();
            this._chaseComponent.setChaseParams(CHASE_MIN, CHASE_MAX, getMonsterAIComponent().monsterAIData.sightDis);
            this._chaseComponent.setMoveType(MoveType.WALK);
            this._chaseComponent.updateTarget(this._monsterAIComponent.getTarget());
        }

        public override void stateOut()
        {
            base.stateOut();
            this._chaseComponent.reset();
        }

        public override AIStateType onThink()
        {
            if (getMonsterAIComponent().hasState(AIStateType.DROWNING) && isInWater())
            {
                return AIStateType.DROWNING;
            }
            if (this._chaseComponent.onChasing())
            {
                return AIStateType.PREATTACK;
            }
            if (this._chaseComponent.onLose()) {
                return AIStateType.FREE;
            }
            return AIStateType.NONE;
        }

        public override AIStateType getType()
        {
            return AIStateType.CHASE;
        }

        public override void dispose()
        {
            base.dispose();
            this._chaseComponent.dispose();
            this._chaseComponent = null;
        }
    }
}
