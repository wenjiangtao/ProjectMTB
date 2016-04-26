/**************
 * 吸引状态，可以是食物也可以是其他
 * ************/
using UnityEngine;
using System.Collections;
namespace MTB
{
    class AIAttractedState : BaseMonsterAIState
    {
        private static int CHASE_MIN = 2;
        private static int CHASE_MAX = 3;
        private IChaseComponent _chaseComponent;

        public AIAttractedState(IAIComponent monsterAIComponent)
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
                return AIStateType.DROWNING;
            if (this._chaseComponent.onChasing())
                return AIStateType.FREE;
            if (this._chaseComponent.onLose())
                return AIStateType.FREE;
            if (_monsterAIComponent.getTarget().GetComponent<PlayerAttributes>().handMaterialId != _decoyItem)
                return AIStateType.FREE;
            return AIStateType.NONE;
        }

        public override AIStateType getType()
        {
            return AIStateType.ATTRACTED;
        }

        public override void dispose()
        {
            base.dispose();
            this._chaseComponent.dispose();
            this._chaseComponent = null;
        }
    }
}
