using System;
using UnityEngine;
namespace MTB
{
    public class MainPlayerController : Singleton<MainPlayerController>
    {
        public bool isCrossModel { get; set; }
        private GOPlayerController _curAttachController;
        public GOPlayerController CurAttachController { get { return _curAttachController; } }

        private ObjectView thirdObjectView;
        private ObjectView firstObjectView;

        private UserInputActionController _userInputActionController;


        void Awake()
        {
            Instance = this;
            GameObject uiobj = GameObject.Find("UI");
            firstObjectView = uiobj.transform.FindChild("Canvas").FindChild("FirstObjectView").GetComponent<ObjectView>();
            _userInputActionController = new UserInputActionController();
			AttachObject(HasActionObjectManager.Instance.playerManager.getMyPlayer().GetComponent<GOPlayerController>());

        }

        void Start()
        {
            AddListener();
        }

        private void AddListener()
        {
            MTBUserInput.On_PlayerViewChange += HandleOn_PlayerViewChange;
            MTBUserInput.On_PlayerInputAction += HandleOn_PlayerInputAction;
            MTBUserInput.On_PlayerTouchEnd += HandleOn_PlayerTouchEnd;
            MTBUserInput.On_PlayerMove += OnMove;
            MTBUserInput.On_PlayerStopMove += OnStopMove;
            MTBUserInput.On_PlayerJump += JumpStart;
            MTBUserInput.On_PlayerJumpEnd += JumpEnd;
            MTBUserInput.On_playerSwitchinView += HandleOn_PlayerSwitchView;
            EventManager.RegisterEvent(EventMacro.ON_CHANGE_HANDCUBE, OnChangeHandCube);
            EventManager.RegisterEvent(EventMacro.ON_SWITCH_PROPMODEL, OnSwitchPropmodel);
        }

        protected void RemoveEvent()
        {
            MTBUserInput.On_PlayerViewChange -= HandleOn_PlayerViewChange;
            MTBUserInput.On_PlayerInputAction -= HandleOn_PlayerInputAction;
            MTBUserInput.On_PlayerTouchEnd -= HandleOn_PlayerTouchEnd;
            MTBUserInput.On_PlayerMove -= OnMove;
            MTBUserInput.On_PlayerStopMove -= OnStopMove;
            MTBUserInput.On_PlayerJump -= JumpStart;
            MTBUserInput.On_PlayerJumpEnd -= JumpEnd;
            MTBUserInput.On_playerSwitchinView -= HandleOn_PlayerSwitchView;
            EventManager.UnRegisterEvent(EventMacro.ON_CHANGE_HANDCUBE, OnChangeHandCube);
            EventManager.UnRegisterEvent(EventMacro.ON_SWITCH_PROPMODEL, OnSwitchPropmodel);
        }

        void HandleOn_PlayerSwitchView()
        {
            if (CameraManager.Instance.CurCamera.CameraType == MTBCameraType.First)
            {
                UseThirdPersonView();
            }
            else
            {
                UseFirstPersonView();
            }
        }

        //使用第一人称摄像机
        public void UseFirstPersonView()
        {
            CameraManager.Instance.UseFirstPersonCamera();
            _curAttachController.ChangeObjectView(firstObjectView);
        }
        //使用第三人称视角
        public void UseThirdPersonView()
        {
            CameraManager.Instance.UseThirdPersonCamera();
            _curAttachController.ChangeObjectView(thirdObjectView);
        }

        private void OnChangeHandCube(object[] param)
        {
            int materialId = Int32.Parse(param[0].ToString());
            _curAttachController.ChangeHandleMaterial(materialId);
        }

        private void OnSwitchPropmodel(object[] param)
        {
            isCrossModel = (bool)param[0];
        }

        void HandleOn_PlayerInputAction(float x, float y, InputActionType inputActionType)
        {
            if (isCrossModel)
            {
                _curAttachController.playerInputState.X = Screen.width / 2;
                _curAttachController.playerInputState.Y = Screen.height / 2;
            }
            else
            {
                _curAttachController.playerInputState.X = x;
                _curAttachController.playerInputState.Y = y;
            }
            _curAttachController.playerInputState.inputActionType = inputActionType;
        }

        void HandleOn_PlayerTouchEnd()
        {
            _curAttachController.playerInputState.inputActionType = InputActionType.Null;
            BlockMaskController.Instance.StopDo();
        }

        void HandleOn_PlayerViewChange(float posX, float posY, float x, float y)
        {
            if (isCrossModel)
            {
                _curAttachController.playerInputState.X = Screen.width / 2;
                _curAttachController.playerInputState.Y = Screen.height / 2;
            }
            else
            {
                _curAttachController.playerInputState.X = posX;
                _curAttachController.playerInputState.Y = posY;
            }
            CameraManager.Instance.CurCamera.Rotate(x, y);
            BlockMaskController.Instance.Do(_curAttachController.playerInputState.X, _curAttachController.playerInputState.Y,
                                            _curAttachController.transform.position, 10);
        }

        private void JumpStart()
        {
            _curAttachController.Jump();
        }

        private void JumpEnd()
        {
            _curAttachController.StopJump();
        }

        private void OnMove(Vector2 direction)
        {
            _curAttachController.Move(new Vector3(direction.x, 0, direction.y));
        }

        private void OnStopMove()
        {
            _curAttachController.StopMove();
        }

        public void AttachObject(GOPlayerController controller)
        {
            _curAttachController = controller;
            WallOfAirManager.Instance.RegisterMovedWallOfAir(_curAttachController.transform, 256, new Vector3(Chunk.chunkWidth, 1, Chunk.chunkDepth));
            thirdObjectView = controller.objectView;
            CameraManager.Instance.SetPlayer(controller.transform);
            UseFirstPersonView();
            _userInputActionController.BinderGameObjectController(controller);

            if (MTB_Minimap.Instance != null)
                MTB_Minimap.Instance.SetTarget(CurAttachController.gameObject);
        }

        void Destroy()
        {
            RemoveEvent();
            WallOfAirManager.Instance.UnRegisterMovedWallOfAir(_curAttachController.transform);
        }
    }
}

