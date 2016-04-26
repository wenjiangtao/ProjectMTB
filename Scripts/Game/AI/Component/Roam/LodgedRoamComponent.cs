using UnityEngine;
using System.Collections;
namespace MTB
{
    public class LodgedRoamComponent : BaseRoamComponent
    {
        public LodgedRoamComponent(GameObject host)
            : base(host)
        {
        }

        protected override void onSpecialRoam()
        {
            Vector3 despoint = AIComponentHelper.getRandomPoint(this._centerPoint, this._roamRadius,this._minRoamDis);
            despoint.Set(despoint.x, despoint.y + 1F, despoint.z);
            startWalk(despoint);
        }

        protected override void startWalk(Vector3 desPoint)
        {
            if (desPoint != null && _moveType == MoveType.LODGED)
            {
                //这边直接瞬移到某个点
                _host.GetComponent<AutoMoveController>().LodgedMove(desPoint);
            }
            else
            {

            }
        }
    }
}
