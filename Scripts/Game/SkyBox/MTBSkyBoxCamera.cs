using System;
using UnityEngine;
namespace MTB
{
	public class MTBSkyBoxCamera : MonoBehaviour
	{
		private Transform cameraTransform;
		private Vector3 pos;
		void Awake()
		{
			cameraTransform = GetComponent<Camera>().transform;
			pos = new Vector3(0,128,0);
		}
		protected void OnPreCull()
		{
			pos.x = cameraTransform.position.x;
			pos.z = cameraTransform.position.z;
			MTBSkyBox.Instance.transform.position = pos;
//			MTBSkyBox.Instance.skyBoxTransforms.cameraTransform.rotation = GetComponent<Camera>().transform.rotation;
		}
	}
}

