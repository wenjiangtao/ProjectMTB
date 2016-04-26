using UnityEngine;
using System.Collections;
namespace MTB
{
    public class MTBTouch : MonoBehaviour
    {
        #region Delegate
        public delegate void TouchCancelHandler(MTBGesture gesture);
        public delegate void TouchStartHandler(MTBGesture gesture);
        public delegate void TouchDownHandler(MTBGesture gesture);
        public delegate void TouchUpHandler(MTBGesture gesture);
        public delegate void SimpleTapHandler(MTBGesture gesture);
        public delegate void DoubleTapHandler(MTBGesture gesture);
        public delegate void LongTapStartHandler(MTBGesture gesture);
        public delegate void LongTapHandler(MTBGesture gesture);
        public delegate void LongTapEndHandler(MTBGesture gesture);
        public delegate void DragStartHandler(MTBGesture gesture);
        public delegate void DragHandler(MTBGesture gesture);
        public delegate void DragEndHandler(MTBGesture gesture);
        public delegate void SwipeStartHandler(MTBGesture gesture);
        public delegate void SwipeHandler(MTBGesture gesture);
        public delegate void SwipeEndHandler(MTBGesture gesture);
        #endregion

        #region Events
        /// <summary>
        /// Occurs when The system cancelled tracking for the touch, as when (for example) the user puts the device to her face.
        /// </summary>
        public event TouchCancelHandler On_Cancel;
        /// <summary>
        /// Occurs when a finger touched the screen.
        /// </summary>
        public event TouchStartHandler On_TouchStart;
        /// <summary>
        /// Occurs as the touch is active.
        /// </summary>
        public event TouchDownHandler On_TouchDown;
        /// <summary>
        /// Occurs when a finger was lifted from the screen.
        /// </summary>
        public event TouchUpHandler On_TouchUp;
        /// <summary>
        /// Occurs when a finger was lifted from the screen, and the time elapsed since the beginning of the touch is less than the time required for the detection of a long tap.
        /// </summary>
        public event SimpleTapHandler On_SimpleTap;
        /// <summary>
        /// Occurs when the number of taps is egal to 2 in a short time.
        /// </summary>
        public event DoubleTapHandler On_DoubleTap;
        /// <summary>
        /// Occurs when a finger is touching the screen,  but hasn't moved  since the time required for the detection of a long tap.
        /// </summary>
        public event LongTapStartHandler On_LongTapStart;
        /// <summary>
        /// Occurs as the touch is active after a LongTapStart
        /// </summary>
        public event LongTapHandler On_LongTap;
        /// <summary>
        /// Occurs when a finger was lifted from the screen, and the time elapsed since the beginning of the touch is more than the time required for the detection of a long tap.
        /// </summary>
        public event LongTapEndHandler On_LongTapEnd;
        /// <summary>
        /// Occurs when a drag start. A drag is a swipe on a pickable object
        /// </summary>
        public static event DragStartHandler On_DragStart;
        /// <summary>
        /// Occurs as the drag is active.
        /// </summary>
        public event DragHandler On_Drag;
        /// <summary>
        /// Occurs when a finger that raise the drag event , is lifted from the screen.
        /// </summary>/
        public event DragEndHandler On_DragEnd;
        /// <summary>
        /// Occurs when swipe start.
        /// </summary>
        public event SwipeStartHandler On_SwipeStart;
        /// <summary>
        /// Occurs as the  swipe is active.
        /// </summary>
        public event SwipeHandler On_Swipe;
        /// <summary>
        /// Occurs when a finger that raise the swipe event , is lifted from the screen.
        /// </summary>
        public event SwipeEndHandler On_SwipeEnd;

        #endregion


        public void Start()
        {
            EasyTouch.On_Cancel += HandleOn_Cancel;
            EasyTouch.On_TouchStart += HandleOn_TouchStart;
            EasyTouch.On_TouchDown += HandleOn_TouchDown;
            EasyTouch.On_TouchUp += HandleOn_TouchUp;
            EasyTouch.On_SimpleTap += HandleOn_SimpleTap;
            EasyTouch.On_DoubleTap += HandleOn_DoubleTap;
            EasyTouch.On_LongTapStart += HandleOn_LongTapStart;
            EasyTouch.On_LongTap += HandleOn_LongTap;
            EasyTouch.On_LongTapEnd += HandleOn_LongTapEnd;
            EasyTouch.On_DragStart += HandleOn_DragStart;
            EasyTouch.On_Drag += HandleOn_Drag;
            EasyTouch.On_DragEnd += HandleOn_DragEnd;
            EasyTouch.On_SwipeStart += HandleOn_SwipeStart;
            EasyTouch.On_Swipe += HandleOn_Swipe;
            EasyTouch.On_SwipeEnd += HandleOn_SwipeEnd;
        }

        public void OnDestroy()
        {
            EasyTouch.On_Cancel -= HandleOn_Cancel;
            EasyTouch.On_TouchStart -= HandleOn_TouchStart;
            EasyTouch.On_TouchDown -= HandleOn_TouchDown;
            EasyTouch.On_TouchUp -= HandleOn_TouchUp;
            EasyTouch.On_SimpleTap -= HandleOn_SimpleTap;
            EasyTouch.On_DoubleTap -= HandleOn_DoubleTap;
            EasyTouch.On_LongTapStart -= HandleOn_LongTapStart;
            EasyTouch.On_LongTap -= HandleOn_LongTap;
            EasyTouch.On_LongTapEnd -= HandleOn_LongTapEnd;
            EasyTouch.On_DragStart -= HandleOn_DragStart;
            EasyTouch.On_Drag -= HandleOn_Drag;
            EasyTouch.On_DragEnd -= HandleOn_DragEnd;
            EasyTouch.On_SwipeStart -= HandleOn_SwipeStart;
            EasyTouch.On_Swipe -= HandleOn_Swipe;
            EasyTouch.On_SwipeEnd -= HandleOn_SwipeEnd;
        }

        public void setEnabled(bool b)
        {
            EasyTouch.SetEnabled(b);
        }

        void HandleOn_SwipeEnd(Gesture gesture)
        {
            if (On_SwipeEnd != null)
            {
                On_SwipeEnd(GetMTBGesture(gesture));
            }
        }

        void HandleOn_Swipe(Gesture gesture)
        {
            if (On_Swipe != null)
            {
                On_Swipe(GetMTBGesture(gesture));
            }
        }

        void HandleOn_SwipeStart(Gesture gesture)
        {
            if (On_SwipeStart != null)
            {
                On_SwipeStart(GetMTBGesture(gesture));
            }
        }

        void HandleOn_DragEnd(Gesture gesture)
        {
            if (On_DragEnd != null)
            {
                On_DragEnd(GetMTBGesture(gesture));
            }
        }

        void HandleOn_Drag(Gesture gesture)
        {
            if (On_Drag != null)
            {
                On_Drag(GetMTBGesture(gesture));
            }
        }

        void HandleOn_DragStart(Gesture gesture)
        {
            if (On_DragStart != null)
            {
                On_DragStart(GetMTBGesture(gesture));
            }
        }

        void HandleOn_LongTapEnd(Gesture gesture)
        {
            if (On_LongTapEnd != null)
            {
                On_LongTapEnd(GetMTBGesture(gesture));
            }
        }

        void HandleOn_LongTap(Gesture gesture)
        {
            if (On_LongTap != null)
            {
                On_LongTap(GetMTBGesture(gesture));
            }
        }

        void HandleOn_LongTapStart(Gesture gesture)
        {
            if (On_LongTapStart != null)
            {
                On_LongTapStart(GetMTBGesture(gesture));
            }
        }

        void HandleOn_DoubleTap(Gesture gesture)
        {
            if (On_DoubleTap != null)
            {
                On_DoubleTap(GetMTBGesture(gesture));
            }
        }

        void HandleOn_SimpleTap(Gesture gesture)
        {
            if (On_SimpleTap != null)
            {
                On_SimpleTap(GetMTBGesture(gesture));
            }
        }

        void HandleOn_TouchUp(Gesture gesture)
        {
            if (On_TouchUp != null)
            {
                On_TouchUp(GetMTBGesture(gesture));
            }
        }

        void HandleOn_TouchDown(Gesture gesture)
        {
            if (On_TouchDown != null)
            {
                On_TouchDown(GetMTBGesture(gesture));
            }
        }

        void HandleOn_TouchStart(Gesture gesture)
        {
            if (On_TouchStart != null)
            {
                On_TouchStart(GetMTBGesture(gesture));
            }
        }


        void HandleOn_Cancel(Gesture gesture)
        {
            if (On_Cancel != null)
            {
                On_Cancel(GetMTBGesture(gesture));
            }
        }

        private MTBGesture GetMTBGesture(Gesture gesture)
        {
            MTBGesture mg = new MTBGesture();
            mg.fingerIndex = gesture.fingerIndex;
            mg.touchCount = gesture.touchCount;
            mg.startPosition = gesture.startPosition;
            mg.position = gesture.position;
            mg.deltaPosition = gesture.deltaPosition;
            mg.actionTime = gesture.actionTime;
            mg.deltaTime = gesture.deltaTime;
            return mg;
        }

    }
}