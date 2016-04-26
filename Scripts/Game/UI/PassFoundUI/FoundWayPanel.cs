using System;
using UnityEngine;
using UnityEngine.UI;
namespace MTB
{
	public class FoundWayPanel : UISubPanel
	{
		private Button _phoneCheckBtn;
		private Button _emailCheckBtn;
		private Button _papersCheckBtn;
		private Button _closeBtn;
		private Button _backBtn;

		public override void InitView ()
		{
			base.InitView ();
			_phoneCheckBtn = this.transform.Find("PhoneCheckBtn").GetComponent<Button>();
			_emailCheckBtn = this.transform.Find("EmailCheckBtn").GetComponent<Button>();
			_papersCheckBtn = this.transform.Find("PapersCheckBtn").GetComponent<Button>();
			_closeBtn = this.transform.Find("CloseBtn").GetComponent<Button>();
			_backBtn = this.transform.Find("BackBtn").GetComponent<Button>();
		}

		protected override void InitEvents ()
		{
			base.InitEvents ();
			_phoneCheckBtn.onClick.AddListener(delegate() {
				mainPanel.CloseSubPanel(this.panelId);
				mainPanel.OpenSubPanel(3);
			});
			_emailCheckBtn.onClick.AddListener(delegate() {
				mainPanel.CloseSubPanel(this.panelId);
				mainPanel.OpenSubPanel(4);
			});
			_papersCheckBtn.onClick.AddListener(delegate() {
				mainPanel.CloseSubPanel(this.panelId);
				mainPanel.OpenSubPanel(5);
			});
			_closeBtn.onClick.AddListener(delegate() {
				UIManager.Instance.showUI<LoginUI>(UITypes.LOGIN);
				mainPanel.Close();
			});
			_backBtn.onClick.AddListener(delegate() {
				mainPanel.CloseSubPanel(this.panelId);
				mainPanel.OpenSubPanel(1);
			});
		}

		protected override void removeEvents ()
		{
			base.removeEvents ();
			_phoneCheckBtn.onClick.RemoveAllListeners();
			_emailCheckBtn.onClick.RemoveAllListeners();
			_papersCheckBtn.onClick.RemoveAllListeners();
			_closeBtn.onClick.RemoveAllListeners();
			_backBtn.onClick.RemoveAllListeners();
		}
	}
}

