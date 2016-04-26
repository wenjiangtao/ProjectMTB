using System;
using UnityEngine;
namespace MTB
{
	public class JumpDownStateBehaviour : StateMachineBehaviour
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
		public void Land(bool loop)
		{
			if(animator != null)
			{
				if(loop)
				{
					animator.SetBool("JumpDown",false);
					animator.SetBool("JumpUp",true);
				}
				else
				{
					animator.SetBool("JumpDown",false);
					animator.SetBool("JumpLand",true);
				}
			}
		}
	}
}

