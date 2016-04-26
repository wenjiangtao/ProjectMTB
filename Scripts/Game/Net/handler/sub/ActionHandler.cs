using UnityEngine;
using System.Collections;

namespace MTB
{
    public class ActionHandler : BaseHandler
    {
        public static event DelegateDef.VoidDelegate On_Shake;
        public static event DelegateDef.VoidDelegate On_Hold;
        public static event DelegateDef.VoidDelegate On_Lift;

        public override void Handler(NetPackage package)
        {
            base.Handler(package);
        }
    }
}
