using UnityEngine;
using System.Collections;

namespace MTB
{
    public class MoveHandler : BaseHandler
    {
        public static event DelegateDef.Vector2Delegate On_Move;
        public static event DelegateDef.VoidDelegate On_MoveEnd;
        private MoveCommandPackage MovePackage;

        public override void Handler(NetPackage package)
        {
            base.Handler(package);
            MovePackage = (MoveCommandPackage)package;
            if (MovePackage.dir.x == 0 && MovePackage.dir.y == 0)
            {
                if (On_MoveEnd != null)
                {
                    On_MoveEnd();
                }
            }
            else
            {
                if (On_Move != null)
                {
                    On_Move(MovePackage.dir);
                }
            }
        }
    }
}
