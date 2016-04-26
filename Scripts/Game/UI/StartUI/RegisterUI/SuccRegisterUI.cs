using System;
using UnityEngine;
using UnityEngine.UI;
namespace MTB
{
	public class SuccRegisterUI : UISubPanel
	{
		private Button _joinGameBtn;

		public override void InitView ()
		{
			base.InitView ();
			_joinGameBtn = this.transform.Find("JoinGameBtn").GetComponent<Button>();
		}

		protected override void InitEvents ()
		{
			base.InitEvents ();
			_joinGameBtn.onClick.AddListener(delegate() {
				UIManager.Instance.showUI<SelectWorldUI>(UITypes.SELECTWORLD);
				UIManager.Instance.closeUI(mainPanel.uiType);
			});
		}

		protected override void removeEvents ()
		{
			base.removeEvents ();
			_joinGameBtn.onClick.RemoveAllListeners();
		}
	}
}

