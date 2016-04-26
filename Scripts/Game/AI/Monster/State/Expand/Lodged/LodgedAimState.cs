using UnityEngine;
using System.Collections;
namespace MTB
{
    public class LodgedAimState : BaseMonsterAIState
    {
        private GameObject _lookObj;

        public LodgedAimState(IAIComponent monsterAIComponent)
            : base(monsterAIComponent)
        {
        }

        public override AIStateType onThink()
        {
            if (this._monsterAIComponent.getTarget() == null)
            {
                Debug.LogError("AIM状态下没有目标!");
                return AIStateType.FREE;
            }
            if (_lookObj == null)
            {
                _lookObj = _host.GetComponent<AutoMoveController>().getLookObj();
            }
            _lookObj.transform.position = new Vector3(this._monsterAIComponent.getTarget().transform.position.x, _host.transform.position.y, this._monsterAIComponent.getTarget().transform.position.z);
            _host.transform.LookAt(_lookObj.transform);
            if (Vector3.Distance(this._monsterAIComponent.getTarget().transform.position, _host.transform.position) > getMonsterAIComponent().monsterAIData.maxRoamDis)
            {
                return AIStateType.FREE;
            }
            return AIStateType.PREATTACK;
        }

        public override AIStateType getType()
        {
            return AIStateType.AIM;
        }
    }
}
