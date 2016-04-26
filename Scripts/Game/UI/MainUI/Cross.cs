using UnityEngine;
using System.Collections;

public class Cross : UIOperateBase
{
    public override void Init(params object[] paras)
    {
        base.Init(paras);
    }

    public override void InitView()
    {
        base.InitView();
        viewGo.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        viewGo.transform.localScale = new Vector3(1.0F, 1.0F, 1.0F);
    }

}
