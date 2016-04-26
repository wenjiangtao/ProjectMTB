/******************
 * 逃跑状态
 * ****************/
using UnityEngine;
using System.Collections;
namespace MTB
{
    class AIRunAwayState : BaseMonsterAIState
    {
        private IChaseComponent _runAwayComponent;
        private static int CHASE_MIN = 30;
        private static int CHASE_MAX = 40;

        public AIRunAwayState(IAIComponent monsterAIComponent)
            : base(monsterAIComponent)
        {
        }

        protected override void init()
        {
            base.init();
            _runAwayComponent = ChaseComponentFactory.createCharseComponent(ChaseComponentType.RUNAWAY, this._host);
        }

        public override void stateIn()
        {
            base.stateIn();
            this._runAwayComponent.setChaseParams(CHASE_MIN, CHASE_MAX, CHASE_MAX);
            this._runAwayComponent.setMoveType(MoveType.WALK);
            this._runAwayComponent.updateTarget(this._monsterAIComponent.getTarget());
        }

        public override void stateOut()
        {
            base.stateOut();
            this._runAwayComponent.reset();
        }

        public override AIStateType onThink()
        {
            if (getMonsterAIComponent().hasState(AIStateType.DROWNING) && isInWater())
            {
                return AIStateType.DROWNING;
            }
            if (this._runAwayComponent.onChasing())
            {
                return AIStateType.FREE;
            }
            if (this._runAwayComponent.onLose())
            {
                return AIStateType.FREE;
            }
            return AIStateType.NONE;
        }

        public override AIStateType getType()
        {
            return AIStateType.RUNAWAY;
        }

        public override void dispose()
        {
            base.dispose();
            this._runAwayComponent.dispose();
            this._runAwayComponent = null;
        }
    }
}
