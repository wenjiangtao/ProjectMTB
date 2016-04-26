using UnityEngine;

namespace MTB
{
    public class NPCFollowPlayerState : BaseNPCAIState
    {
        private static int FOLLOWDIS_MIN = 3;
        private static int FOLLOWDIS_MAX = 5;
        private IChaseComponent _chaseComponent;

        public NPCFollowPlayerState(IAIComponent npcAIComponent)
            : base(npcAIComponent)
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
            this._chaseComponent.setChaseParams(FOLLOWDIS_MIN, FOLLOWDIS_MAX, 100);
            this._chaseComponent.setMoveType(MoveType.WALK);
            this._chaseComponent.updateTarget(this._npcAIComponent.getTarget());
        }

        public override void stateOut()
        {
            base.stateOut();
            this._chaseComponent.reset();
        }

        public override AIStateType onThink()
        {
            if (this._chaseComponent.onChasing())
            {
                return AIStateType.PREATTACK;
            }
            if (this._chaseComponent.onLose())
            {
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
