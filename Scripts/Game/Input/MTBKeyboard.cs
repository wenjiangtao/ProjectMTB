using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace MTB
{
    public class MTBKeyboard : MonoBehaviour
    {
        private static bool _enableMark = true;
        public static event DelegateDef.Vector2Delegate On_Move;
        public static event DelegateDef.VoidDelegate On_MoveEnd;
        public static event DelegateDef.VoidDelegate On_Jump;
        public static event DelegateDef.VoidDelegate On_JumpEnd;
        public static event DelegateDef.VoidDelegate On_Switching;
        public static event DelegateDef.VoidDelegate On_SwitchPropModel;
        public static event DelegateDef.VoidDelegate On_Shake;
        public static event DelegateDef.VoidDelegate On_Hold;
        public static event DelegateDef.VoidDelegate On_Lift;

        public static void setEnable(bool b)
        {
            _enableMark = b;
        }

        enum KeyStatus
        {
            None,
            Up,
            Down,
        };

        HashSet<KeyCode> keyDownSet = new HashSet<KeyCode>();
        HashSet<KeyCode> lastFrameKeyDownSet = new HashSet<KeyCode>();
        KeyCode[] keyList = { KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.W, KeyCode.Space };
        KeyCode[] moveKeyList = { KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.W };
        Dictionary<KeyCode, Vector2> keyDir = new Dictionary<KeyCode, Vector2>(){
			{KeyCode.A, new Vector2 (-1, 0)},
			{KeyCode.S, new Vector2 (0, -1)},
			{KeyCode.D, new Vector2(1, 0)}, {KeyCode.W, new Vector2(0,1)}};
        int moveStartFrame = 0;
        // Update is called once per frame
        void Update()
        {
            if (!_enableMark)
                return;
            lastFrameKeyDownSet = new HashSet<KeyCode>(keyDownSet);
            //keyDownSet.Clear ();
            for (int i = 0; i < keyList.Length; i++)
            {
                if (Input.GetKeyDown(keyList[i]))
                    keyDownSet.Add(keyList[i]);
                else if (Input.GetKeyUp(keyList[i]))
                    keyDownSet.Remove(keyList[i]);
            }
            //test
            if (Input.GetKeyUp(KeyCode.C))
            {
                ViewUpdate();
            }
            if (Input.GetKeyUp(KeyCode.Q))
            {
                PropModelChange();
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                PlayerHold();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                PlayerLift();
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                PlayerShake();
            }

            MoveUpdate();
            JumpUpdate();
        }

        void MoveUpdate()
        {
            int keyDownCount = 0;
            int lastKeyDownCount = 0;
            foreach (KeyCode k in moveKeyList)
            {
                if (keyDownSet.Contains(k))
                {
                    keyDownCount += 1;
                    //break;
                }
                if (lastFrameKeyDownSet.Contains(k))
                {
                    lastKeyDownCount += 1;
                }
                if (keyDownCount != 0 && lastKeyDownCount != 0)
                    break;
            }


            if (lastKeyDownCount != 0 && keyDownCount == 0)
            {
                OnMoveEnd();
                return;
            }

            if (keyDownCount != 0)
            {
                OnMove();
                if (lastKeyDownCount == 0)
                {
                    moveStartFrame = Time.frameCount;
                }
            }
        }

        void JumpUpdate()
        {
            if (keyDownSet.Contains(KeyCode.Space))
            {
                OnJump();
                return;
            }
            if (lastFrameKeyDownSet.Contains(KeyCode.Space))
            {
                OnJumpEnd();
            }
        }

        void ViewUpdate()
        {
            if (On_Switching != null)
                On_Switching();
        }

        void PropModelChange()
        {
            if (On_SwitchPropModel != null)
                On_SwitchPropModel();
        }

        void PlayerShake()
        {
            if (On_Shake != null)
            {
                On_Shake();
            }
        }

        void PlayerHold()
        {
            if (On_Hold != null)
            {
                On_Hold();
            }
        }

        void PlayerLift()
        {
            if (On_Lift != null)
            {
                On_Lift();
            }
        }

        void OnMoveEnd()
        {
            if (On_MoveEnd != null)
            {
                On_MoveEnd();
                keyDownSet.Clear();
            }
        }

        void OnMove()
        {
            //Debug.Log ("OnMove~~~~~~~");
            if (On_Move != null)
            {
                Vector2 dir = Vector2.zero;
                foreach (KeyCode key in moveKeyList)
                {
                    if (keyDownSet.Contains(key))
                        dir += keyDir[key];
                }
                On_Move(dir);
            }
        }

        void OnJump()
        {
            if (On_Jump != null)
            {
                On_Jump();
            }
        }

        void OnJumpEnd()
        {
            if (On_JumpEnd != null)
            {
                On_JumpEnd();
            }
        }
    }
}
