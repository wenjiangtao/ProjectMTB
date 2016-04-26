using UnityEngine;
using System.Collections;
namespace MTB
{
    class RunAwayComponent : BaseChaseComponent
    {
        public RunAwayComponent(GameObject host)
            : base(host)
        {
        }

        public override bool onChasing()
        {
            if (this._target == null)
            {
                Debug.LogError("无法找到目标，清先设置要追踪的目标");
                return true;
            }
            return _isMoveEnd;
        }

        protected override void startMove()
        {
            if (_desPoint == null)
                return;
            EventManager.RegisterEvent(EventMacro.MOVE_TO_DESPOINT, onMoveToTarget);
            _host.GetComponent<AutoMoveController>().startRunAway(_target.transform);
            _isMoveEnd = false;
        }
    }
}
