using System;
using UnityEngine;
namespace MTB
{
	public class UISubPanel : UIOperateBase
	{
		protected UIMainPanel mainPanel;
		protected int panelId;

		public static T New<T>(int id,string name,UIMainPanel panel, params object[] paras) where T : UISubPanel
		{
			GameObject g = panel.transform.Find(name).gameObject;
			T t = g.AddComponent<T>();
			t.SetId(id);
			t.SetMainPanel(panel);
			t.Init(paras);
			t.hide();
			return t;
		}

		//因为这两个方法会改变初始位置，所以重写了一下
		public override void Open ()
		{
			foreach (UIOperateBase child in childrenList)
				child.Open();
			viewGo.SetActive(true);
			InitEvents();
		}

		public override void InitView ()
		{
			viewGo = this.gameObject;
		}
		
		public void SetMainPanel(UIMainPanel panel)
		{
			mainPanel = panel;
		}

		public void SetId(int id)
		{
			this.panelId = id;
		}
	}
}

