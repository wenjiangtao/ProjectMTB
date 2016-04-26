using System;
using UnityEngine;
using UnityEngine.UI;
namespace MTB
{
	public class ConfirmAccountPanel : UISubPanel
	{
		private Button _nextBtn;
		private Button _closeBtn;

		public override void InitView ()
		{
			base.InitView ();
			_nextBtn = this.transform.Find("NextBtn").GetComponent<Button>();
			_closeBtn = this.transform.Find("CloseBtn").GetComponent<Button>();
		}

		protected override void InitEvents ()
		{
			base.InitEvents ();
			_nextBtn.onClick.AddListener(delegate() {
				mainPanel.CloseSubPanel(this.panelId);
				mainPanel.OpenSubPanel(2);
			});
			_closeBtn.onClick.AddListener(delegate() {
				UIManager.Instance.showUI<LoginUI>(UITypes.LOGIN);
				mainPanel.Close();
			});
		}

		protected override void removeEvents ()
		{
			base.removeEvents ();
			_nextBtn.onClick.RemoveAllListeners();
			_closeBtn.onClick.RemoveAllListeners();
		}
	}
}

