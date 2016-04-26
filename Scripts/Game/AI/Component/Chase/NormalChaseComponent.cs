using UnityEngine;
using System.Collections;
namespace MTB
{
    public class NormalChaseComponent : BaseChaseComponent
    {
        public NormalChaseComponent(GameObject host)
            : base(host)
        {
        }

        public override bool onChasing()
        {
            if (this._target == null) {
                Debug.LogError("无法找到目标，清先设置要追踪的目标");
                return true;
            }
            return _isMoveEnd;
        }
    }
}
