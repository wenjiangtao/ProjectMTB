using UnityEngine;
using System.Collections;
namespace MTB
{
    public class RandomPointRoamComponent : BaseRoamComponent
    {
        public RandomPointRoamComponent(GameObject host)
            : base(host)
        { }

        protected override void onStartWalk()
        {
            base.onStartWalk();
            Vector3 despoint = AIComponentHelper.getRandomPoint(this._centerPoint, this._roamRadius,this._minRoamDis);
            startWalk(despoint);
        }

        protected override void onStartIdle()
        {
            base.onStartIdle();
        }
    }

}