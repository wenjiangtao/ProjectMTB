using System;
using UnityEngine;
namespace MTB
{
	public class AnimatorState : StateMachineBehaviour
	{
		public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			animator.SendMessage("OnAnimActionStateEnter",new StateInfo(stateInfo.fullPathHash,layerIndex),SendMessageOptions.DontRequireReceiver);
			base.OnStateEnter (animator, stateInfo, layerIndex);
		}

//		public override void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//		{
//			animator.SendMessage("OnAnimActionStateExit",new StateInfo(stateInfo.fullPathHash,layerIndex),SendMessageOptions.DontRequireReceiver);
//			base.OnStateExit (animator, stateInfo, layerIndex);
//		}
	}
}

