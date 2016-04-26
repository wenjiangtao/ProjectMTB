using System;
using UnityEngine;
namespace MTB
{
    public class MTBUserInput : Singleton<MTBUserInput>
    {
        public delegate void PlayerMoveHandler(Vector2 direction);
        public delegate void PlayerStopMoveHandler();
        public delegate void PlayerViewChangeStartHandler(float x, float y);
        public delegate void PlayerViewChangeHandler(float posX, float posY, float delX, float delY);
        public delegate void PlayerViewChangeEndHandler(float x, float y);
        public delegate void PlayerInputActionHandler(float x, float y, InputActionType inputActionType);
        public delegate void PlayerSwitchingViewHandler();

        public static event PlayerMoveHandler On_PlayerMove;
        public static event PlayerStopMoveHandler On_PlayerStopMove;
        public static event PlayerViewChangeStartHandler On_PlayerViewChangeStart;
        public static event PlayerViewChangeHandler On_PlayerViewChange;
        public static event PlayerViewChangeEndHandler On_PlayerViewChangeEnd;
        public static event PlayerInputActionHandler On_PlayerInputAction;
        public static event DelegateDef.VoidDelegate On_PlayerJump;
        public static event DelegateDef.VoidDelegate On_PlayerJumpEnd;
        public static event DelegateDef.VoidDelegate On_PlayerTouchEnd;
        public static event PlayerSwitchingViewHandler On_playerSwitchinView;
        public static event DelegateDef.VoidDelegate On_PlayerSwitchPropModel;

        public MTBTouch Touch { get; set; }
        private MTBJoystick Joystick { get; set; }
        private MTBKeyboard Keyboard { get; set; }
        public GameObject JoyStickView { get; set; }
        void Awake()
        {
            Instance = this;
            Init();
        }

        void Init()
        {
            Touch = GetComponentInChildren<MTBTouch>();
            Joystick = GetComponentInChildren<MTBJoystick>();
            JoyStickView = GameObject.Find("EasyTouchControlsCanvas");
            JoyStickView.GetComponent<Canvas>().overrideSorting = true;
            JoyStickView.GetComponent<Canvas>().sortingOrder = -1;
#if UNITY_EDITOR
            Keyboard = gameObject.AddComponent<MTBKeyboard>();
#endif
            AddEvent();
        }

        void AddEvent()
        {
            Touch.On_SwipeStart += HandleOn_SwipeStart;
            Touch.On_Swipe += HandleOn_Swipe;
            Touch.On_SwipeEnd += HandleOn_SwipeEnd;
            Touch.On_LongTap += HandleOn_LongTap;
            Touch.On_SimpleTap += HandleOn_SimpleTap;
            Touch.On_DoubleTap += HandleOn_DoubleTap;
            Touch.On_TouchUp += HandleOn_TouchEnd;
            Joystick.On_Move += HandleOn_Move;
            Joystick.On_MoveEnd += HandleOn_MoveEnd;
            EventManager.RegisterEvent(EventMacro.ON_CLICK_JUMP_DOWN, (object[] paras) => { HandleOn_Jump(); });
            EventManager.RegisterEvent(EventMacro.ON_CLICK_JUMP_UP, (object[] paras) => { HandleOn_JumpEnd(); });
            EventManager.RegisterEvent(EventMacro.ON_CLICK_SWITCH_VIEW, (object[] paras) => { HandleOn_Switching(); });

            //EventManager.RegisterEvent(EventMacro.CLOSE_UI, (object[] paras) =>
            //{
            //    if ((UITypes)paras[0] == UITypes.MAIN_BAG)
            //    {
            //        SetJoyStickActive(true);
            //    }
            //});

            //EventManager.RegisterEvent(EventMacro.SHOW_UI, (object[] paras) =>
            //{
            //    if ((UITypes)paras[0] == UITypes.MAIN_BAG) { SetJoyStickActive(false); }
            //});

#if UNITY_EDITOR
            MTBKeyboard.On_Move += HandleOn_Move;
            MTBKeyboard.On_MoveEnd += HandleOn_MoveEnd;
            MTBKeyboard.On_Jump += HandleOn_Jump;
            MTBKeyboard.On_JumpEnd += HandleOn_JumpEnd;
            MTBKeyboard.On_Switching += HandleOn_Switching;
#endif
        }


        void HandleOn_SwipeStart(MTBGesture gesture)
        {
            if (On_PlayerViewChangeStart != null)
            {
                On_PlayerViewChangeStart(gesture.position.x, gesture.position.y);
            }
        }

        void HandleOn_SwipeEnd(MTBGesture gesture)
        {
            if (On_PlayerViewChangeEnd != null)
            {
                On_PlayerViewChangeEnd(gesture.position.x, gesture.position.y);
            }
        }

        void HandleOn_SimpleTap(MTBGesture gesture)
        {
            if (On_PlayerInputAction != null)
            {
                On_PlayerInputAction(gesture.position.x, gesture.position.y, InputActionType.SimpleTap);
            }
        }

        void HandleOn_DoubleTap(MTBGesture gesture)
        {
            if (On_PlayerInputAction != null)
            {
                On_PlayerInputAction(gesture.position.x, gesture.position.y, InputActionType.DoubleTap);
            }
        }

        void HandleOn_TouchEnd(MTBGesture gesture)
        {
            if (On_PlayerTouchEnd != null)
            {
                On_PlayerTouchEnd();
            }
        }

        void RemoveEvent()
        {
            Touch.On_SwipeStart -= HandleOn_SwipeStart;
            Touch.On_Swipe -= HandleOn_Swipe;
            Touch.On_SwipeEnd -= HandleOn_SwipeEnd;
            Touch.On_LongTap -= HandleOn_LongTap;
            Touch.On_SimpleTap -= HandleOn_SimpleTap;

            Touch.On_TouchUp -= HandleOn_TouchEnd;
            Touch.On_DoubleTap -= HandleOn_DoubleTap;
            Joystick.On_Move -= HandleOn_Move;
            Joystick.On_MoveEnd -= HandleOn_MoveEnd;
#if UNITY_EDITOR
            MTBKeyboard.On_Move -= HandleOn_Move;
            MTBKeyboard.On_MoveEnd -= HandleOn_MoveEnd;
            MTBKeyboard.On_Jump -= HandleOn_Jump;
            MTBKeyboard.On_JumpEnd -= HandleOn_JumpEnd;
            MTBKeyboard.On_Switching -= HandleOn_Switching;
#endif
        }

        void OnDestroy()
        {
            RemoveEvent();
        }

        void HandleOn_MoveEnd()
        {
            if (On_PlayerStopMove != null)
            {
                On_PlayerStopMove();
            }
        }

        void HandleOn_Move(Vector2 direction)
        {
            if (On_PlayerMove != null)
            {
                On_PlayerMove(direction);
            }
        }

        void HandleOn_Jump()
        {
            if (On_PlayerJump != null)
            {
                On_PlayerJump();
            }
        }

        void HandleOn_JumpEnd()
        {
            if (On_PlayerJumpEnd != null)
            {
                On_PlayerJumpEnd();
            }
        }

        void HandleOn_LongTap(MTBGesture gesture)
        {
            if (On_PlayerInputAction != null)
            {
                On_PlayerInputAction(gesture.position.x, gesture.position.y, InputActionType.LongTap);
            }
        }

        void HandleOn_Swipe(MTBGesture gesture)
        {
            if (On_PlayerViewChange != null)
            {
                On_PlayerViewChange(gesture.position.x, gesture.position.y, gesture.deltaPosition.x, gesture.deltaPosition.y);
            }
        }

        void HandleOn_Switching()
        {
            if (On_playerSwitchinView != null)
            {
                On_playerSwitchinView();
            }
        }

		private int activeSign = 0;
        public void SetJoyStickActive(bool b)
        {
			if(!b)activeSign++;
			else activeSign--;
			bool result = true;
			if(activeSign > 0)result = false;
            JoyStickView.SetActive(result);
			Joystick.setActived(result);
			Touch.setEnabled(result);
        }
    }
    public enum InputActionType
    {
        Null=0,
        SimpleTap=1,
        DoubleTap=2,
        LongTap=3
    }

	public enum InputType
	{
		Joystick = 0,
		Button = 1,
		ScreenTouch = 2
	}

	public enum InputJoystickType
	{
		Zero = 0,
		NoZero = 1
	}

	public enum InputButtonType
	{
		JumpDwon = 0,
		JumpUp = 1
	}
}

