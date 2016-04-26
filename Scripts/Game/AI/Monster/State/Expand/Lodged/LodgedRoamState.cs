/***
 * 遁地状态
 * 
 * ***/
using UnityEngine;
using System.Collections;
namespace MTB
{
    public class LodgedRoamState : BaseMonsterAIState
    {
        private IRoamComponent _roamComponent;

        private RoamType _roamType;

        private int _roamTime;

        private int _state;

        public LodgedRoamState(IAIComponent monsterAIComponent)
            : base(monsterAIComponent)
        {
        }

        protected override void init()
        {
            base.init();
            this._roamComponent = RoamComponentFactory.creatRoamComponent(RoamComponentType.RANDOM_LODGED, this._host);
            this._roamComponent.setMoveType(MoveType.LODGED);
        }

        public override void stateIn()
        {
            base.stateIn();
            this._roamComponent.setMoveType(MoveType.LODGED);
            _state = 1;
            GameObject host = getMonsterAIComponent().host();
            this._roamTime = getMonsterAIComponent().monsterAIData.roamTime;
            this._roamType = RoamType.LODGED;

            this._roamComponent.setRoamCenterPoint(this._host.transform.position);
            this._roamComponent.setRoamParams(getMonsterAIComponent().monsterAIData.maxRoamDis, getMonsterAIComponent().monsterAIData.roamIdelTime, getMonsterAIComponent().monsterAIData.minRoamDis);
        }

        public override void stateOut()
        {
            base.stateOut();
            if (this._roamComponent != null)
            {
                this._roamComponent.reset();
            }
        }

        public override AIStateType onThink()
        {
            if (_state == 1)
            {
                if (_roamTime <= 0)
                {
                    _state = 2;
                }
                _roamTime--;
            }
            if (_state == 2)
            {
                this._roamComponent.setRoamType(RoamType.LODGED);
                return AIStateType.AFTERROAM;
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
