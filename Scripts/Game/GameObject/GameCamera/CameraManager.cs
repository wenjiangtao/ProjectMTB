using System;
using UnityEngine;
using System.Collections.Generic;
namespace MTB
{
    public class CameraManager : Singleton<CameraManager>
    {
		private static bool _init = false;
		public static bool Init{get{return _init;}}
        private Transform mainPlayer;
        private MTBCamera thirdPersonCamera;
		private MTBCamera firstPersonCamera;
        private MTBCamera _curCamera;
        public MTBCamera CurCamera { get { return _curCamera; } }
		private WorldPos _curPos= new WorldPos(0,0,int.MaxValue);
		public Block _inBlock = Block.NullBlock;
		public Block InBlock{get{return _inBlock;}}

		private void Awake()
		{
			Instance = this;
			firstPersonCamera = GetComponentInChildren<FirstPersonCamera>();
			thirdPersonCamera = GetComponentInChildren<ThirdPersonCamera>();
			firstPersonCamera.gameObject.SetActive(false);
			thirdPersonCamera.gameObject.SetActive(false);
			_init = true;
		}

		private void Update()
		{
			if(!_curPos.EqualOther(CurCamera.Pos))
			{
				_curPos = CurCamera.Pos;
				_inBlock = World.world.GetBlock(_curPos.x,_curPos.y,_curPos.z);
			}
		}

        public void SetPlayer(Transform mainPlayer)
        {
            this.mainPlayer = mainPlayer;
			this.transform.parent = mainPlayer;
			this.transform.localPosition = this.transform.position;
			this.transform.localRotation = Quaternion.identity;
			firstPersonCamera.SetMainPlayer(mainPlayer);
			thirdPersonCamera.SetMainPlayer(mainPlayer);
        }

        public void UseFirstPersonCamera()
        {
            firstPersonCamera.gameObject.SetActive(true);
            thirdPersonCamera.gameObject.SetActive(false);
            firstPersonCamera.transform.localRotation = thirdPersonCamera.transform.localRotation;
			_curCamera = firstPersonCamera;
        }

        public void UseThirdPersonCamera()
        {
            thirdPersonCamera.gameObject.SetActive(true);
            firstPersonCamera.gameObject.SetActive(false);
            thirdPersonCamera.transform.localRotation = firstPersonCamera.transform.localRotation;
            _curCamera = thirdPersonCamera;
        }
    }
}

