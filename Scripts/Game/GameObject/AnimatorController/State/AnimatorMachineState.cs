using System;
using UnityEngine;
namespace MTB
{
	public class AnimatorMachineState : StateMachineBehaviour
	{
		public int layerIndex;
		public override void OnStateMachineEnter (Animator animator, int stateMachinePathHash)
		{
			animator.SendMessage("OnAnimActionStateEnter",new StateInfo(stateMachinePathHash,layerIndex),SendMessageOptions.DontRequireReceiver);
			base.OnStateMachineEnter (animator, stateMachinePathHash);
		}
		
//		public override void OnStateMachineExit (Animator animator, int stateMachinePathHash)
//		{
//			animator.SendMessage("OnAnimActionStateExit",new StateInfo(stateMachinePathHash,layerIndex),SendMessageOptions.DontRequireReceiver);
//			base.OnStateMachineExit (animator, stateMachinePathHash);
//		}
	}
}

