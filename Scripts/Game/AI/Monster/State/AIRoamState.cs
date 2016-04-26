using UnityEngine;
using System.Collections;
namespace MTB
{
    public class AIRoamState : BaseMonsterAIState
    {
        private IRoamComponent _roamComponent;

        private RoamType _roamType;

        private int _roamTime;

        public AIRoamState(IAIComponent monsterAIComponent)
            : base(monsterAIComponent)
        {
        }

        protected override void init()
        {
            base.init();
            this._roamComponent = RoamComponentFactory.creatRoamComponent(RoamComponentType.RANDOM_POINT, this._host);
        }

        public override void stateIn()
        {
            base.stateIn();

            GameObject host = this._monsterAIComponent.host();
            this._roamTime = getMonsterAIComponent().monsterAIData.roamTime;
            this._roamType = RoamType.WALK;

            this._roamComponent.setRoamCenterPoint(this._host.transform.position);
            this._roamComponent.setRoamParams(getMonsterAIComponent().monsterAIData.maxRoamDis, getMonsterAIComponent().monsterAIData.roamIdelTime, getMonsterAIComponent().monsterAIData.minRoamDis);
            this._roamComponent.setRoamType(this._roamType);
        }

        public override void stateOut()
        {
            base.stateOut();
            this._roamComponent.reset();
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
            this._roamComponent.onRoam();
            if (this._roamType != this._roamComponent.getRoamType())
            {
                this._roamType = this._roamComponent.getRoamType();
                if (this._roamType == RoamType.WALK)
                {
                    this._roamComponent.setRoamCenterPoint(this._host.transform.position);
                }
            }
            this._roamTime--;
            if (this._roamTime <= 0)
            {
                return AIStateType.FREE;
            }
            return AIStateType.NONE;
        }

        public override AIStateType getType()
        {
            return AIStateType.ROAM;
        }

        public override void dispose()
        {
            base.dispose();

            this._roamComponent.dispose();
            this._roamComponent = null;
        }
    }
}
