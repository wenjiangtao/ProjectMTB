using System;
using UnityEngine;
namespace MTB
{
	public abstract class MTBCamera : MonoBehaviour
	{
		public float viewSensitivity = 20f;
		
		public float viewXRotateMax = 4f;
		public float viewYRotateMax = 5f;

		public float MaxLookUpAngle = 80f;
		public float MaxLookDownAngle = 80f;

		public Camera followCamera;

		public bool needDamping = true;
		public float damping = 5.0f;
		public float dampingMinAngle = 1f;

		public WorldPos Pos{get{return _pos;}}
		private WorldPos _pos;

		private float curXAngle;

		protected Transform mainPlayer;

		void Start()
		{
			Vector3 angles = transform.eulerAngles;
			curXAngle = angles.x;
		}

		private static float receivedXAngle = 4f;
		public virtual void Rotate(float x,float y)
		{
//			//绕y轴旋转的角度
			float yAngle = x * Time.fixedDeltaTime * viewSensitivity;
			yAngle = GetViewRotateValue(yAngle, viewYRotateMax);
			Quaternion yQuaternion = Quaternion.AngleAxis(yAngle, Vector3.up);
			Quaternion rotateY = mainPlayer.localRotation * yQuaternion;
			if (needDamping && Mathf.Abs(yAngle) > dampingMinAngle)
			{
				mainPlayer.localRotation = Quaternion.Lerp(mainPlayer.localRotation, rotateY, Time.fixedDeltaTime * damping);
			}
			else
			{
				mainPlayer.localRotation = rotateY;
			}

			//绕x轴旋转的角度
			float xAngle = y * Time.fixedDeltaTime * viewSensitivity;
			xAngle = GetViewRotateValue(xAngle, viewXRotateMax);
			if(xAngle < -receivedXAngle)xAngle = -receivedXAngle;
			else if(xAngle > receivedXAngle) xAngle = receivedXAngle;
			curXAngle -= xAngle;
			curXAngle = ClampAngle(curXAngle,-MaxLookDownAngle,MaxLookUpAngle);
			Quaternion rotateX = Quaternion.Euler(curXAngle,0,0);
			if(needDamping && Math.Abs(xAngle) > dampingMinAngle)
			{
				transform.localRotation = Quaternion.Lerp(transform.localRotation, rotateX,Time.fixedDeltaTime * damping);
			}
			else
			{
				transform.localRotation = rotateX;
			}
		}

		protected virtual void Update()
		{
			_pos = Terrain.GetWorldPos(followCamera.transform.position + followCamera.transform.forward * (followCamera.nearClipPlane + 0.001f));
		}

		private float ClampAngle(float angle, float min, float max)  
		{  
			return Mathf.Clamp(angle, min, max);  
		}  

		public void SetMainPlayer(Transform mainPlayer)
		{
			this.mainPlayer = mainPlayer;
		}

		private float GetViewRotateValue(float input, float max)
		{
			if (input < -max) input = -max;
			else if (input > max) input = max;
			return input;
		}

		public abstract MTBCameraType CameraType{get;}

		public Camera FollowCamera{get{return followCamera;}}
	}
}

