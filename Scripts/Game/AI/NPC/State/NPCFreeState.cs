namespace MTB
{
    public class NPCFreeState : BaseNPCAIState
    {
        public NPCFreeState(IAIComponent npcAIComponent)
            : base(npcAIComponent)
        {
        }

        public override void stateIn()
        {
            base.stateIn();
            this._npcAIComponent.setTarget(null);
            this._host.GetComponent<AutoMoveController>().cancelAutoMove();
        }

        public override void stateOut()
        {
            base.stateOut();
        }

        public override AIStateType onThink()
        {
            return AIStateType.IDEL;
        }

        public override AIStateType getType()
        {
            return AIStateType.FREE;
        }
    }
}
