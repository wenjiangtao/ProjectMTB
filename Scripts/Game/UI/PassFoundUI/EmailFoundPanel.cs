using System;
using UnityEngine;
using UnityEngine.UI;
namespace MTB
{ 
	public class EmailFoundPanel : UISubPanel
	{
		private Button _closeBtn;

		public override void InitView ()
		{
			base.InitView ();
			_closeBtn = this.transform.Find("CloseBtn").GetComponent<Button>();
		}

		protected override void InitEvents ()
		{
			base.InitEvents ();
			_closeBtn.onClick.AddListener(delegate() {
				UIManager.Instance.showUI<LoginUI>(UITypes.LOGIN);
				mainPanel.Close();
			});
		}
		
		protected override void removeEvents ()
		{
			base.removeEvents ();
			_closeBtn.onClick.RemoveAllListeners();
		}
	}
}

