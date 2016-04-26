using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
namespace MTB
{
    class MTBInputField : MonoBehaviour
    {
        private InputField _inputfield;
        private string _inputString;

        void Awake()
        {
            _inputfield = GetComponent<InputField>();
            UIKeyboard.onInput += onInput;
            _inputString = "";
        }

        void OnDestroy()
        {
            UIKeyboard.onInput -= onInput;
        }

        void onInput(string input)
        {
            if (!_inputfield.isFocused)
                return;
            if (input == null || input == " ")
                return;
            if (input == "\b")
            {
                onBackSpace();
                return;
            }
            if (_inputString.Length < 10)
            {
                _inputString += input;
            }
            _inputfield.text = _inputString;
        }

        void onBackSpace()
        {
            if (!_inputfield.isFocused)
            {
                return;
            }
            _inputString = _inputString.Substring(0, _inputString.Length - 1);
            _inputfield.text = _inputString;
        }

        public string getString()
        {
            return _inputfield.text;
        }

    }
}
