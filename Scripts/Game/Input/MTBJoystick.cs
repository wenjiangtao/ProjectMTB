using System;
using UnityEngine;
using UnityEngine.Events;
namespace MTB
{
    public class MTBJoystick : MonoBehaviour
    {

        #region Delegate
        public delegate void OnMoveStartHandler();
        public delegate void OnMoveHandler(Vector2 direction);
        public delegate void OnMoveEndHandler();

        #endregion

        #region Events
        public event OnMoveStartHandler On_MoveStart;
        public event OnMoveHandler On_Move;
        public event OnMoveEndHandler On_MoveEnd;
        #endregion

        private UnityAction OnMoveStartAction;
        private UnityAction<Vector2> OnMoveAction;
        private UnityAction OnMoveEndAction;

        public ETCJoystick joystick;

        public void Start()
        {
            if (joystick != null)
            {
                Init_OnMoveStart();
                Init_OnMove();
                Init_OnMoveEnd();
            }
        }

        public void setActived(bool b)
        {
            joystick.activated = b;
        }

        void Init_OnMoveStart()
        {
            OnMoveStartAction = new UnityAction(HandleOnMoveStart);
            joystick.onMoveStart.AddListener(OnMoveStartAction);
        }

        void HandleOnMoveStart()
        {
            if (On_MoveStart != null)
            {
                On_MoveStart();
            }
        }

        void Init_OnMove()
        {
            OnMoveAction = new UnityAction<Vector2>(HandleOnMove);
            joystick.onMove.AddListener(OnMoveAction);
        }

        void HandleOnMove(Vector2 direction)
        {
            if (On_Move != null)
            {
                On_Move(direction);
            }
        }

        void Init_OnMoveEnd()
        {
            OnMoveEndAction = new UnityAction(HandleOnMoveEnd);
            joystick.onMoveEnd.AddListener(OnMoveEndAction);
        }

        void HandleOnMoveEnd()
        {
            if (On_MoveEnd != null)
            {
                On_MoveEnd();
            }
        }

        public void OnDestroy()
        {
            if (joystick != null)
            {
                joystick.onMoveStart.RemoveListener(OnMoveStartAction);
                joystick.onMove.RemoveListener(OnMoveAction);
                joystick.onMoveEnd.RemoveListener(OnMoveEndAction);
            }
        }
    }
}

