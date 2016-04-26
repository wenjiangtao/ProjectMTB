namespace MTB
{
    public class NPCIdleState : BaseNPCAIState
    {
        private int _idelTime;

        public NPCIdleState(IAIComponent npcAIComponent)
            : base(npcAIComponent)
        {
        }

        public override void stateIn()
        {
            base.stateIn();
            this._idelTime = 100;
        }

        public override void stateOut()
        {
            base.stateOut();
        }

        public override AIStateType onThink()
        {
            this._idelTime--;
            if (this._idelTime <= 0)
            {
                return AIStateType.FREE;
            }
            return AIStateType.NONE;
        }

        public override AIStateType getType()
        {
            return AIStateType.IDEL;
        }
    }
}
