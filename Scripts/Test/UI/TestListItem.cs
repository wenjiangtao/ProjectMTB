using UnityEngine;
using System.Collections;

public class TestListItem : UIItemBase
{
    public override void InitElements()
    {
        Transform nameTrans = mTrans.FindChild("Name");
        AddElement("Name", nameTrans.GetComponent<UGUIText>());
    }

    public override void OnUpdate(string name, string attr, object value)
    {
        base.OnUpdate(name, attr, value);
    }

    public void OnDestroy()
    {
        base.OnDestroy();
    }
}
