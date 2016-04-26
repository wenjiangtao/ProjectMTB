using System;
using System.Collections.Generic;
using UnityEngine;
namespace MTB
{
	public class AnimatorController : MonoBehaviour
	{
		public delegate void OnActionStateEnterHandler(StateInfo info);
		public delegate void OnActionStateExitHandler(StateInfo info);
		public delegate void OnAnimatorEvent(UnityEngine.Object value);

		public event OnActionStateEnterHandler On_ActionStateEnter;
		public event OnActionStateExitHandler On_ActionStateExit;
		public event OnAnimatorEvent On_AnimatorEvent;

		[System.NonSerialized]
		public Animator animator;


		private Queue<string> _curKeys;
		void Awake()
		{
			animator = GetComponent<Animator>();
			_curKeys = new Queue<string>();
		}

		public void DoAction(string key)
		{
			_curKeys.Enqueue(key);
			animator.SetBool(key,true);
		}

		public void JumpAtHighestPoint()
		{
			JumpUpStateBehaviour behaviour = animator.GetBehaviour<JumpUpStateBehaviour>();
			if(behaviour == null)return;
			behaviour.GoToJumpDown();
		}

		public void JumpLandPoint(bool loop)
		{
			JumpDownStateBehaviour behaviour = animator.GetBehaviour<JumpDownStateBehaviour>();
			if(behaviour == null)return;
			behaviour.Land(loop);
		}

		public void ChangeSpeed(float speed)
		{
			animator.SetFloat("speed",speed);
		}

		public string GetLayerName(int lay)
		{
			return animator.GetLayerName(lay);
		}

		protected virtual void OnAnimActionStateEnter(StateInfo info)
		{
			while(_curKeys.Count > 0)
			{
				string key = _curKeys.Dequeue();
				animator.SetBool(key,false);
			}
			if(On_ActionStateEnter != null)
			{
				On_ActionStateEnter(info);
			}
		}
		//暂时不启用这个
//		protected virtual void OnAnimActionStateExit(StateInfo info)
//		{
//			if(On_ActionStateExit != null)
//			{
//				On_ActionStateExit(info);
//			}
//		}

		void OnAnimEvent(UnityEngine.Object value)
		{
			if(On_AnimatorEvent != null)
			{
				On_AnimatorEvent(value);
			}
		}

		public void Dispose()
		{
			while(_curKeys.Count > 0)
			{
				string key = _curKeys.Dequeue();
				animator.SetBool(key,false);
			}
		}
	}
}
