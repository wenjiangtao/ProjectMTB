using System;
namespace MTB
{
	public class PlayerMovableController : MovableController
	{
		#region implemented abstract members of MovableController

		protected override void InitMovableCondition ()
		{
			RegisterMovableCondition(MovableGround);
			RegisterMovableCondition(MovableWater);
			RegisterMovableCondition(MovableFree);
			ChangeMovableCondition(MovableConditionType.Ground);
		}

		#endregion

		public MovableGround MovableGround = new MovableGround();
		public MovableWater MovableWater = new MovableWater();
		public MovableFree MovableFree = new MovableFree();

		public PlayerMovableController ()
		{
		}
	}
}

