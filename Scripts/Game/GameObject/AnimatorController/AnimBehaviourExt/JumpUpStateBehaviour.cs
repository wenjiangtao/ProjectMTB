using System;
using UnityEngine;
namespace MTB
{
	public class JumpUpStateBehaviour : StateMachineBehaviour
	{
		private Animator animator;
		public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.animator = animator;
			base.OnStateEnter (animator, stateInfo, layerIndex);
		}
		
		public override void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			animator = null;
			base.OnStateExit (animator, stateInfo, layerIndex);
		}

		public void GoToJumpDown()
		{
			if(animator != null)
			{
				animator.SetBool("JumpDown",true);
				animator.SetBool("JumpLand",false);
				animator.SetBool("JumpUp",false);
			}
		}
	}
}

