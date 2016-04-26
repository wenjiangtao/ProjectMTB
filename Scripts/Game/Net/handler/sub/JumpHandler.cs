using UnityEngine;
using System.Collections;

namespace MTB
{
    public class JumpHandler : BaseHandler
    {
        public static event DelegateDef.VoidDelegate On_Jump;
        public static event DelegateDef.VoidDelegate On_JumpEnd;
        private JumpCommandPackage JumpPackage;

        public override void Handler(NetPackage package)
        {
            base.Handler(package);
            JumpPackage = (JumpCommandPackage)package;
            if (JumpPackage.command == 1)
            {
                if (On_Jump != null)
                {
                    On_Jump();
                }
            }
            else
            {
                if (On_JumpEnd != null)
                {
                    On_JumpEnd();
                }
            }
        }
    }
}
