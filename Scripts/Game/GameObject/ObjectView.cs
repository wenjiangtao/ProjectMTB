using System;
using UnityEngine;
namespace MTB
{
	public class ObjectView : MonoBehaviour
	{
		public AnimatorController animatorController;
		public GameObject avatar;
		public HandMountPointController handMountPointController;
		private Renderer render;
		private float _curLightIntensity;
		private float _prevLightIntensity;
		void Awake()
		{
			handMountPointController = GetComponent<HandMountPointController>();
			if(avatar != null)
			{
				render = avatar.GetComponent<Renderer>();
			}
		}

		public void ShowAvatar()
		{
			if(avatar != null)
			{
				avatar.SetActive(true);
			}
		}
		
		public void HideAvatar()
		{
			
			if(avatar != null)
			{
				avatar.SetActive(false);
			}
		}

		public void Show()
		{
			this.gameObject.SetActive(true);
		}

		public void Hide()
		{
			this.gameObject.SetActive(false);
		}

		public void InitLight(int sunLightLevel,int blockLightLevel)
		{
			_curLightIntensity = MTBSkyBox.Instance.GetLightIntensity(sunLightLevel,blockLightLevel);
			_prevLightIntensity = _curLightIntensity;
			UpdateLightIntensity(_curLightIntensity);
		}

		bool canUpdateLight = false;
		float startTime = 0;
		public void UpdateLight(int sunLightLevel,int blockLightLevel)
		{
			float lightIntensity = MTBSkyBox.Instance.GetLightIntensity(sunLightLevel,blockLightLevel);
			if(Mathf.Abs(_curLightIntensity - lightIntensity) > 0.001f)
			{
				if(!canUpdateLight)
				{
					canUpdateLight = true;
					startTime = Time.time;
					_prevLightIntensity = _curLightIntensity;
				}
				_curLightIntensity = lightIntensity;
			}
			if(canUpdateLight)
			{
				float lerpTime = (Time.time - startTime);
				float realLightIntensity = Mathf.Lerp(_prevLightIntensity,_curLightIntensity,lerpTime > 1 ? 1 : lerpTime);
				UpdateLightIntensity(realLightIntensity);
				if(lerpTime > 1)
				{
					canUpdateLight = false;
				}
			}else
			{
				UpdateLightIntensity(_curLightIntensity);
			}
		}
		
		public void UpdateLightIntensity(float lightIntensity)
		{
			if(render != null)
			{
				render.material.SetFloat("_lightIntensity",lightIntensity);
			}
			if(handMountPointController != null)
			{
				GameObject handObject = handMountPointController.CurHandObj;
				if(handObject != null)
				{
					handObject.GetComponent<MeshRenderer>().material.SetFloat("_lightIntensity",lightIntensity);
				}
			}
		}
	}
}

