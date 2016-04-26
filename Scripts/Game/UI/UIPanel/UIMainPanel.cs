using System;
using System.Collections.Generic;
namespace MTB
{
	public class UIMainPanel : UISubPanel
	{
		private Dictionary<int,UISubPanel> _mapPanel;

		public override void Init (params object[] paras)
		{
			_mapPanel = new Dictionary<int, UISubPanel>();
			base.Init (paras);
		}

		public override void InitView ()
		{
			base.InitView ();
			InitSubPanel();
		}

		protected virtual void InitSubPanel()
		{

		}

		public virtual void Refresh()
		{
		}

		protected void RegisterSubPanel<T>(int id,string name) where T : UISubPanel
		{
			UISubPanel panel = UISubPanel.New<T>(id,name,this) as UISubPanel;
			_mapPanel.Add(id,panel);
		}

		public virtual void OpenSubPanel(int id)
		{
			UISubPanel panel;
			_mapPanel.TryGetValue(id,out panel);
			if(panel != null)
			{
				panel.Open();
			}
		}

		public void CloseSubPanel(int id)
		{
			UISubPanel panel;
			_mapPanel.TryGetValue(id,out panel);
			if(panel != null)
			{
				panel.Close();
			}
		}

		public void CloseAllSubPanel()
		{
			foreach (var item in _mapPanel) {
				item.Value.Close();
			}
		}

		public override void Close ()
		{
			base.Close ();
			foreach (var item in _mapPanel) {
				item.Value.Close();
			}
		}

		public override void Dispose ()
		{
			foreach (var item in _mapPanel) {
				item.Value.Dispose();
			}
		}
	}
}

