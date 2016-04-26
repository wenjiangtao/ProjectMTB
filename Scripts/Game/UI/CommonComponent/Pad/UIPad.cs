/*****
 * 各个Pad需要自行继承此类
 * 实现各自pad的功能
 * *****/
using UnityEngine;
using System.Collections;
namespace MTB
{
    public class UIPad : UIItemBase
    {
        protected UITypes _type;

        public override void Init(string id, params object[] paras)
        {
            _type = (UITypes)paras[0];
            base.Init(id, paras);
        }

        public override void InitElements()
        {
            InitComponents();
            setSelect(false);
        }

        //各自再重写
        protected virtual void InitComponents()
        {
            Debug.LogError("InitComponents方法需要重写！");
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

        public void setSelect(bool b)
        {
            gameObject.SetActive(b);
        }
    }
}
