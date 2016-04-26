using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
namespace MTB
{
    public class UIIcon : UIItemBase
    {
        private Button _button;
        private bool _selectMark = false;
        private Transform _imageTrans;
        private int _materialId;
        private string _resPath;
        private UITypes _uiType;

        public override void Init(string id, params object[] paras)
        {
            _resPath = Convert.ToString(paras[1]);
            _uiType = (UITypes)paras[0];
            base.Init(id, paras);
        }

        public override void InitElements()
        {
            _imageTrans = mTrans.FindChild("Image");
            _imageTrans.GetComponent<UGUIImage>().SetPath(_resPath);
            AddElement("Image", _imageTrans.GetComponent<UGUIImage>());
            _button = gameObject.GetComponent<Button>();
            _button.onClick.AddListener(delegate()
            {
                UIEventManager.SendEvent(UIEventManager.ET_UI_CLICK, _uiType.ToString(), "Image", "", IconResManager.getIconNameByMId(_materialId), _materialId, id);
            });
        }

        public override void OnUpdate(params object[] paras)
        {
            if (UItype != (UITypes)paras[0])
                return;
            base.OnUpdate(paras);
            _materialId = (int)paras[4];
//            if (System.Convert.ToString(paras[2]) == "null")
//                _button.enabled = false;
//            else
//                _button.enabled = true;
        }

        public void setEnable(bool b)
        {
            gameObject.SetActive(b);
        }
    }
}
