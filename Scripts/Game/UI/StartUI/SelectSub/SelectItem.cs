using UnityEngine;
using UnityEngine.UI;
using System.Collections;
namespace MTB
{
    public class SelectItem : UIItemBase
    {
        protected Button _selectBtn;

        protected GameObject _textField;

        protected Text _text;

        public override void Init(string id, params object[] paras)
        {
            _text = transform.Find("Text").gameObject.GetComponent<Text>();
            base.Init(id, paras);
        }

        public override void InitElements()
        {
            _selectBtn = gameObject.GetComponent<Button>();
            _selectBtn.onClick.AddListener(delegate()
            {
				OnSelect();
            });
        }

		protected virtual void OnSelect()
		{
		}

        public void setEnable(bool b)
        {
            gameObject.SetActive(b);
        }

        public override void dispose()
        {
            _selectBtn.onClick.RemoveAllListeners();
            base.dispose();
        }
    }
}
