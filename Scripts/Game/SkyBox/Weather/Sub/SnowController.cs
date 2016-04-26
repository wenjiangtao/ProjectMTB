using UnityEngine;
using System.Collections;
namespace MTB
{
    class SnowController : FallDownController
    {
        public SnowController(GameObject weatherObj)
            : base(weatherObj)
        {

        }

        protected override void initDirObj()
        {
            base.initDirObj();
            for (int i = 0; i < 9; i++)
            {
                _directObjs[i] = GameObject.Find("SnowDir" + i);
            }
            setEnable(false);
        }
    }
}
