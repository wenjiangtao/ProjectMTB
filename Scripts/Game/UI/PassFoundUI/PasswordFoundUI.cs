using System;
using UnityEngine;
using UnityEngine.UI;
namespace MTB
{
	public class PasswordFoundUI : UIMainPanel
	{
		public override void Init (params object[] paras)
		{
			uiType = UITypes.PASSWORDFOUND;
			base.Init (paras);
		}

		protected override void InitSubPanel ()
		{
			RegisterSubPanel<ConfirmAccountPanel>(1,"ConfirmAccountPanel");
			RegisterSubPanel<FoundWayPanel>(2,"FoundWayPanel");
			RegisterSubPanel<PhoneFoundPanel>(3,"PhoneFoundPanel");
			RegisterSubPanel<EmailFoundPanel>(4,"EmailFoundPanel");
			RegisterSubPanel<PapersFoundPanel>(5,"PapersFoundPanel");
			RegisterSubPanel<ResetPWPanel>(6,"ResetPWPanel");
			RegisterSubPanel<EmailResetPWPanel>(7,"EmailResetPWPanel");
		}

		public override void Open ()
		{
			base.Open ();
			OpenSubPanel(1);
		}

	}
}

