using UnityEngine;

namespace MTB
{
    public class NPCRoamState : BaseNPCAIState
    {
        private IRoamComponent _roamComponent;
        private RoamType _roamType;
        private int _roamTime;

        public NPCRoamState(IAIComponent npcAIComponent)
            : base(npcAIComponent)
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

            GameObject host = this._npcAIComponent.host();
            this._roamType = RoamType.WALK;
            this._roamTime = 500;
            this._roamComponent.setRoamCenterPoint(this._host.transform.position);
            this._roamComponent.setRoamParams(20, 0, 1);
            this._roamComponent.setRoamType(this._roamType);
        }

        public override void stateOut()
        {
            base.stateOut();
            this._roamComponent.reset();
        }

        public override AIStateType onThink()
        {
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
