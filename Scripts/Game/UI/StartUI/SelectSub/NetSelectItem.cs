using System;
namespace MTB
{
	public class NetSelectItem : SelectItem
	{
		private string ip;
		private int port;
		public override void Init (string id, params object[] paras)
		{
			base.Init (id, paras);
			ip = (string)paras[1];
			port = (int)paras[2];
			_text.text = ip + ":" + port;
		}

		protected override void OnSelect ()
		{
			EventManager.SendEvent(UIEventMacro.CLICK_ITEM, UItype,NetType.Local, ip,port);
		}
	}
}

