using System;
using UnityEngine;
namespace MTB
{
	public class FirstPersonCamera : MTBCamera
	{
		#region implemented abstract members of MTBCamera

		public override MTBCameraType CameraType {
			get {
				return MTBCameraType.First;
			}
		}

		#endregion


	}
}

