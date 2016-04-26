namespace MTB
{
    public class AIIdleState : BaseMonsterAIState
    {
        private int _idelTime;

        public AIIdleState(IAIComponent monsterAIComponent)
            : base(monsterAIComponent)
        {
        }

        public override void stateIn()
        {
            base.stateIn();
            this._idelTime = getMonsterAIComponent().monsterAIData.idelTime;
        }

        public override void stateOut()
        {
            base.stateOut();
        }

        public override AIStateType onThink()
        {
            if (getMonsterAIComponent().hasState(AIStateType.DROWNING) && isInWater())
            {
                return AIStateType.DROWNING;
            }
            _checkTargetType = checkTarget();
            if (_checkTargetType != AIStateType.NONE)
            {
                return _checkTargetType;
            }
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
