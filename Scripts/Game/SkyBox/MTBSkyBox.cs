using UnityEngine;
using System.Collections;

namespace MTB
{
	public class MTBSkyBox : Singleton<MTBSkyBox> {

		public SkyBoxPrefabs skyBoxTransforms;

		public SkyBoxMaterials skyBoxMaterials;

		public SkyBoxParams skyBoxParams;

		private float _curSunLightScale;
		public float CurSunLightScale{get{return _curSunLightScale;}}
		private Material _curSkyBoxMaterial;

		void Awake()
		{
			MTBSkyBox.Instance = this;
		}

		void Start () {
			skyBoxTransforms.skyBoxTransform = this.transform;
			_curSkyBoxMaterial = skyBoxMaterials.skyBoxDayToNightFall;
		}

		void Update () {
			UpdateLightAndSun();
			UpdateCloud();
			UpdateFog();
		}

		public float GetLightIntensity(int sunLightLevel,int blockLightLevel)
		{
			float sunLight = LightConst.lightColor[sunLightLevel];
			float blockLight = LightConst.lightColor[blockLightLevel];
			float lightIntensity = (blockLight + sunLight * _curSunLightScale);
			lightIntensity = lightIntensity > 1 ? 1 : lightIntensity;
			return lightIntensity;
		}

		private void UpdateLightAndSun()
		{
			DayTimeSlot slot = DayNightTime.Instance.TimeSlot;
			
			SkyLightScaleParam skyLight;
			SphereMoveParam sunNormalMove;
			SphereMoveParam sunSpecialMove;
			
			switch(slot)
			{
			case DayTimeSlot.Day:
				skyLight = skyBoxParams.skyParam.dayLight;
				sunNormalMove = skyBoxParams.sunNormalParam.dayAngle;
				sunSpecialMove = skyBoxParams.sunSpecialParam.dayAngle;
				break;
			case DayTimeSlot.Evening:
				skyLight = skyBoxParams.skyParam.eveningLight;
				sunNormalMove = skyBoxParams.sunNormalParam.eveningAngle;
				sunSpecialMove = skyBoxParams.sunSpecialParam.eveningAngle;
				break;
			case DayTimeSlot.Night:
				skyLight = skyBoxParams.skyParam.nightLight;
				sunNormalMove = skyBoxParams.sunNormalParam.nightAngle;
				sunSpecialMove = skyBoxParams.sunSpecialParam.nightAngle;
				break;
			default:
				skyLight = skyBoxParams.skyParam.morningLight;
				sunNormalMove =skyBoxParams.sunNormalParam.morningAngle;
				sunSpecialMove = skyBoxParams.sunSpecialParam.morningAngle;
				break;
			}
			
			float lerp = DayNightTime.Instance.SlotElapsedTime / DayNightTime.Instance.SlotTime;
			
			//更新一般天空亮度（昼夜变化的时候）
			_curSunLightScale = Mathf.Lerp(skyLight.lightScaleFrom,skyLight.lightScaleTo,lerp);

			//根据昼夜变化的亮度来改变贴图
			float dayNightFactor;
			if(_curSunLightScale > skyBoxParams.skyParam.dayPicLightScaleMax)
			{
				dayNightFactor = 0;
				_curSkyBoxMaterial = skyBoxMaterials.skyBoxDayToNightFall;
			}
			else if(_curSunLightScale < skyBoxParams.skyParam.nightPicScaleMin)
			{
				dayNightFactor = 1;
				_curSkyBoxMaterial = skyBoxMaterials.skyBoxNightFallToNight;
			}
			else
			{
				if(_curSunLightScale > skyBoxParams.skyParam.nightFallScaleMiddle)
				{
					dayNightFactor = (skyBoxParams.skyParam.dayPicLightScaleMax - _curSunLightScale) / (skyBoxParams.skyParam.dayPicLightScaleMax - skyBoxParams.skyParam.nightFallScaleMiddle);
					_curSkyBoxMaterial = skyBoxMaterials.skyBoxDayToNightFall;
				}
				else
				{
					dayNightFactor = (skyBoxParams.skyParam.nightFallScaleMiddle - _curSunLightScale) / (skyBoxParams.skyParam.nightFallScaleMiddle - skyBoxParams.skyParam.nightPicScaleMin);
					_curSkyBoxMaterial = skyBoxMaterials.skyBoxNightFallToNight;
				}
//				dayNightFactor = (skyBoxParams.skyParam.dayPicLightScaleMax - _curSunLightScale) / (skyBoxParams.skyParam.dayPicLightScaleMax - skyBoxParams.skyParam.nightPicScaleMin);
			}
			RenderSettings.skybox = _curSkyBoxMaterial;
			_curSkyBoxMaterial.SetFloat("_DayNightFactor",dayNightFactor);

			//云对天空亮度的影响
			_curSunLightScale -= skyBoxParams.cloudParam.lightIntensityDecrease * skyBoxParams.cloudParam.lightIntensityFactor;
			if(_curSunLightScale < 0)_curSunLightScale = 0;
			//真正更新天空亮度
			Shader.SetGlobalFloat("MTB_SunLightScale",_curSunLightScale);

			//显示与隐藏太阳
			bool isSpecialTime = DayNightTime.Instance.IsSpecialTime;
			bool isOddDay = DayNightTime.Instance.Days % 2 == 1;
			bool sunNormalShow = (skyBoxParams.sunNormalParam.oddDayShow == isOddDay && !isSpecialTime) ||
				(isSpecialTime);
			skyBoxTransforms.sunNormalTransform.gameObject.SetActive(sunNormalShow);
			
			bool sunSpecialShow = (skyBoxParams.sunSpecialParam.oddDayShow == isOddDay && !isSpecialTime) ||
				(isSpecialTime);
			skyBoxTransforms.sunSpecialTransform.gameObject.SetActive(sunSpecialShow);
			
			//更新一般太阳位置
			float sunXAngleDeg = Mathf.Lerp(sunNormalMove.angleFrom.x,sunNormalMove.angleTo.x,lerp);
			float sunYAngleDeg = Mathf.Lerp(sunNormalMove.angleFrom.y,sunNormalMove.angleTo.y,lerp);
			UpdateSunNormalPosition(skyBoxParams.sunNormalParam.sunRadius,sunYAngleDeg,sunXAngleDeg);
			
			//更新特殊太阳位置
			float sunSpecialXAngleDeg = Mathf.Lerp(sunSpecialMove.angleFrom.x,sunSpecialMove.angleTo.x,lerp);
			float sunSpecialYAngleDeg = Mathf.Lerp(sunSpecialMove.angleFrom.y,sunSpecialMove.angleTo.y,lerp);
			UpdateSunSpecialPosition(skyBoxParams.sunSpecialParam.sunRadius,sunSpecialYAngleDeg,sunSpecialXAngleDeg);
		}

		private Vector2 cloudSpeed = new Vector2();
		private void UpdateCloud()
		{
			cloudSpeed += skyBoxParams.cloudParam.cloudMoveSpeed * Time.deltaTime;
			skyBoxMaterials.cloud.SetColor("_CloudColor",skyBoxParams.cloudParam.cloudColor);
			skyBoxMaterials.cloud.SetFloat("_CloudDensity",skyBoxParams.cloudParam.cloudDensity);
			skyBoxMaterials.cloud.SetFloat("_CloudSharpness",skyBoxParams.cloudParam.cloudSharpness);
			skyBoxMaterials.cloud.SetFloat("_CloudScale",skyBoxParams.cloudParam.cloudScale);
			skyBoxMaterials.cloud.SetVector("_CloudUV",cloudSpeed);
		}

		private void UpdateFog()
		{
			bool needWaterFog = false;
			if(CameraManager.Init)
			{
				if((CameraManager.Instance.InBlock.BlockType == BlockType.StillWater 
				    || CameraManager.Instance.InBlock.BlockType == BlockType.FlowingWater))
				{
					needWaterFog = true;
				}
			}
			if(needWaterFog)
			{
				_curSkyBoxMaterial.SetFloat("_WaterUseFog",1f);
				skyBoxMaterials.cloud.SetFloat("_WaterUseFog",1f);
				skyBoxMaterials.sunNormal.SetFloat("_WaterUseFog",1f);
				skyBoxMaterials.sunSpecial.SetFloat("_WaterUseFog",1f);
				RenderSettings.fogColor = Color.Lerp(skyBoxParams.waterFogParam.nightColor,skyBoxParams.waterFogParam.dayColor,_curSunLightScale * _curSunLightScale);
				RenderSettings.fogStartDistance = skyBoxParams.waterFogParam.fogStart;
				RenderSettings.fogEndDistance = skyBoxParams.waterFogParam.fogEnd;
			}
			else
			{
				_curSkyBoxMaterial.SetFloat("_WaterUseFog",0f);
				skyBoxMaterials.cloud.SetFloat("_WaterUseFog",0f);
				skyBoxMaterials.sunNormal.SetFloat("_WaterUseFog",0f);
				skyBoxMaterials.sunSpecial.SetFloat("_WaterUseFog",0f);
				RenderSettings.fogColor = Color.Lerp(skyBoxParams.fogParam.nightColor,skyBoxParams.fogParam.dayColor,_curSunLightScale * _curSunLightScale);
				RenderSettings.fogStartDistance = skyBoxParams.fogParam.fogStart;
				RenderSettings.fogEndDistance = skyBoxParams.fogParam.fogEnd;
			}
		}

		private void UpdateSunNormalPosition(float radius,float yAngleDeg,float xAngleDeg)
		{
			float yAngle = yAngleDeg * Mathf.Deg2Rad;
			float xAngle = xAngleDeg * Mathf.Deg2Rad;
			Vector3 v = AngleToWorldPos(radius,yAngle,xAngle);
			skyBoxTransforms.sunNormalTransform.position = skyBoxTransforms.skyBoxTransform.position +
				skyBoxTransforms.skyBoxTransform.rotation * v;
			skyBoxTransforms.sunNormalTransform.LookAt(skyBoxTransforms.skyBoxTransform.position);
		}

		private void UpdateSunSpecialPosition(float radius,float yAngleDeg,float xAngleDeg)
		{
			float yAngle = yAngleDeg * Mathf.Deg2Rad;
			float xAngle = xAngleDeg * Mathf.Deg2Rad;
			Vector3 v = AngleToWorldPos(radius,yAngle,xAngle);
			skyBoxTransforms.sunSpecialTransform.position = skyBoxTransforms.skyBoxTransform.position +
				skyBoxTransforms.skyBoxTransform.rotation * v;
			skyBoxTransforms.sunSpecialTransform.LookAt(skyBoxTransforms.skyBoxTransform.position);
		}

		//yAngle表示当前球面点到原点的向量与y轴的夹角，xAngle即与x轴夹角
		internal Vector3 AngleToWorldPos(float radius, float yAngle, float xAngle)
		{
			Vector3 res;
			
			float sinTheta = Mathf.Sin(yAngle);
			float cosTheta = Mathf.Cos(yAngle);
			float sinPhi   = Mathf.Sin(xAngle);
			float cosPhi   = Mathf.Cos(xAngle);
			
			res.z = radius * sinTheta * cosPhi;
			res.y = radius * cosTheta;
			res.x = radius * sinTheta * sinPhi;
			
			return res;
		}
	}
	[System.Serializable]
	public class SkyBoxPrefabs
	{
		public Transform sunNormalTransform;
		public Transform sunSpecialTransform;
		public Transform cameraTransform;
		public Transform cloudTransform;
		[System.NonSerialized]
		public Transform skyBoxTransform;
	}
	[System.Serializable]
	public class SkyBoxMaterials
	{
		public Material skyBox;
		public Material sunNormal;
		public Material sunSpecial;
		public Material cloud;
		public Material skyBoxDayToNightFall;
		public Material skyBoxNightFallToNight;
	}
	[System.Serializable]
	public class SkyBoxParams
	{
		public MTB_SkyParams skyParam;
		public MTB_SunParams sunNormalParam;
		public MTB_SunParams sunSpecialParam;
		public MTB_CloudParams cloudParam;
		public MTB_FogParams fogParam;
		public MTB_FogParams waterFogParam;
	}

	[System.Serializable]
	public class MTB_SkyParams
	{
		public SkyLightScaleParam morningLight;
		public SkyLightScaleParam dayLight;
		public SkyLightScaleParam eveningLight;
		public SkyLightScaleParam nightLight;
		[Range(0,1)]
		public float dayPicLightScaleMax = 1;
		[Range(0,1)]
		public float nightFallScaleMiddle = 0.5f;
		[Range(0,1)]
		public float nightPicScaleMin = 0;
	}
	[System.Serializable]
	public class MTB_SunParams
	{
		public float sunRadius = 200;
		public bool oddDayShow = true;
		public SphereMoveParam morningAngle;
		public SphereMoveParam dayAngle;
		public SphereMoveParam eveningAngle;
		public SphereMoveParam nightAngle;
	}

	[System.Serializable]
	public class MTB_CloudParams
	{
		public Color cloudColor;
		[Range(0,1f)]
		public float cloudDensity;
		[Range(0,1f)]
		public float cloudSharpness;
		[Range(0,15f)]
		public float cloudScale;
		[Range(0,1f)]
		public float lightIntensityDecrease;
		[Range(0,1f)]
		public float lightIntensityFactor;

		public Vector2 cloudMoveSpeed;
	}

	[System.Serializable]
	public class MTB_FogParams
	{
		public Color dayColor;
		public Color nightColor;
		public int fogStart;
		public int fogEnd;
	}

	[System.Serializable]
	public class SphereMoveParam
	{
		public Vector2 angleFrom;
		public Vector2 angleTo;
	}
	[System.Serializable]
	public class SkyLightScaleParam
	{
		[Range(0,1)]
		public float lightScaleFrom = 1;
		[Range(0,1)]
		public float lightScaleTo = 1;
	}
}