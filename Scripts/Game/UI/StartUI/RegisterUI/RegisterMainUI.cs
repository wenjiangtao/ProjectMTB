using System;
using UnityEngine;
using UnityEngine.UI;
namespace MTB
{
	public class RegisterMainUI : UIMainPanel
	{
		public override void Init(params object[] paras)
		{
			uiType = UITypes.REGISTER;
			base.Init(paras);
		}

		protected override void InitSubPanel ()
		{
			RegisterSubPanel<RegisterUI>(1,"RegisterPanel");
			RegisterSubPanel<SuccRegisterUI>(2,"RegisterFinishPanel");
		}

		public override void Open ()
		{
			base.Open ();
			OpenSubPanel(1);
		}
	}
}

