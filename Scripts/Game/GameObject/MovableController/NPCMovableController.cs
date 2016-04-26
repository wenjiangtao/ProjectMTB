using System;
namespace MTB
{
    public class NPCMovableController : MovableController
    {
        #region implemented abstract members of MovableController

        protected override void InitMovableCondition()
        {
            RegisterMovableCondition(MovableGround);
            RegisterMovableCondition(MovableWater);
            ChangeMovableCondition(MovableConditionType.Ground);
        }

        #endregion
        public MovableGround MovableGround = new MovableGround();
        public MovableWater MovableWater = new MovableWater();

        public NPCMovableController()
        {
        }
    }
}
