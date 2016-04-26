using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace MTB
{

    class UIKeyboard : MonoBehaviour
    {
        private static bool _enableMark = false;

        public delegate void KeyBoardInputHandler(string key);

        public static event KeyBoardInputHandler onInput;

        public static event DelegateDef.VoidDelegate onBackSpace;

        public static void setEnable(bool b)
        {
            _enableMark = b;
        }

        void Update()
        {
            if (!_enableMark)
                return;
            if (Input.GetKeyUp(KeyCode.Backspace))
            {
                onBack();
            }
            else if (Input.anyKeyDown)
            {
                onKeyDown(Input.inputString);
            }
        }

        void onKeyDown(string key)
        {
            if (onInput != null)
            {
                onInput(key);
            }
        }

        void onBack()
        {
            if (onBackSpace != null)
            {
                onBackSpace();
            }
        }
    }
}
