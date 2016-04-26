using UnityEngine;
using System.Collections;
namespace MTB
{
    public class DefaultNpcAIComponent : BaseNPCAIComponent
    {
        public DefaultNpcAIComponent()
            : base()
        {
        }

        protected override void initNPCAIState()
        {
            registerNPCAIState(new NPCFreeState(this));
            //registerNPCAIState(new NPCRoamState(this));
            registerNPCAIState(new NPCIdleState(this));
            registerNPCAIState(new NPCFollowPlayerState(this));

            runAIState(AIStateType.FREE);
        }

    }
}
