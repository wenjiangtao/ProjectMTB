using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
namespace MTB
{
    public class UITab : UIItemBase
    {
        protected Button _button;
        protected UITypes _type;
        protected GameObject _selectedUI;
        protected string _selectMcName;

        public override void Init(string id, params object[] paras)
        {
            _type = (UITypes)paras[0];
            _selectMcName = Convert.ToString(paras[1]);
            base.Init(id, paras);
        }

        public override void InitElements()
        {
            _selectedUI = GameObject.Find(_selectMcName + id);
            _button = gameObject.GetComponent<Button>();
            _button.interactable = true;
            _button.onClick.AddListener(delegate()
            {
                EventManager.SendEvent(UIEventMacro.CLICK_TAB, _type, id);
            });
            setSelect(false);
        }

        public override void Register()
        {
            EventManager.RegisterEvent(UIEventMacro.CLICK_TAB, onClickTab);
        }

        public override void UnRegister()
        {
            EventManager.UnRegisterEvent(UIEventMacro.CLICK_TAB, onClickTab);
        }

        protected virtual void onClickTab(params object[] paras)
        {
            if ((UITypes)paras[0] == _type && paras[1].ToString() == this.id)
            {
                setSelect(true);
            }
            else
            {
                setSelect(false);
            }
        }

        public void setEnable(bool b)
        {
            gameObject.SetActive(b);
        }

        public void setSelect(bool b)
        {
            if (_selectedUI != null)
            {
                _selectedUI.SetActive(b);
            }
        }
    }

}
