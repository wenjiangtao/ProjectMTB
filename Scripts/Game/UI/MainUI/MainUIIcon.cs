using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using MTB;
public class MainUIIcon : UIItemBase
{
	public int materialId{get;private set;}
//    private Button _button;

    public override void InitElements()
    {
        Transform imageTrans = mTrans.FindChild("Image");
        imageTrans.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        AddElement("Image", imageTrans.GetComponent<UGUIImage>());
		materialId = 0;
//        _button = gameObject.GetComponent<Button>();
//        _button.onClick.AddListener(delegate()
//        {
//            EventManager.SendEvent(EventMacro.ON_CHANGE_HANDCUBE, _materialId, this.id);
//        });
    }

    public override void OnUpdate(params object[] paras)
    {
        if (UItype != (UITypes)paras[0])
            return;
        base.OnUpdate(paras);
        materialId = (int)paras[4];
    }

    public void setEnable(bool b)
    {
        gameObject.SetActive(b);
    }
}
