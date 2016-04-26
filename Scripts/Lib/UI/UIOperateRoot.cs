using UnityEngine;
using System.Collections;
using System;
using MTB;
public class UIOperateRoot : UIOperateBase
{

    public static UIOperateRoot instance;
    public static UIOperateBase uiOpRoot;
    public bool crossMark = false;

    protected override void InitEvents()
    {
        EventManager.RegisterEvent(EventMacro.ON_CLICK_SWITCH_PROPMODEL, (object[] paras) =>
        {
            if (paras[0] != null && Convert.ToInt32(paras[0]) == 1)
            {
                crossMark = true;
                UIManager.Instance.showUI<Cross>(UITypes.CROSS);
                EventManager.SendEvent(EventMacro.ON_SWITCH_PROPMODEL, crossMark);
            }
            else
            {
                crossMark = false;
                UIManager.Instance.closeUI(UITypes.CROSS);
                EventManager.SendEvent(EventMacro.ON_SWITCH_PROPMODEL, crossMark);
            }
        });
        //EventManager.RegisterEvent(TestUIEventMacro.ON_CLICK_INIT_BAG, (object[] paras) =>
        //{
        //    if (TestUserData.itemBag != null)
        //    {
        //        HintMsg.Instance.Hint("数据已初始化完毕");
        //        return;
        //    }
        //    TestUserData.InitItemBag("ItemBag", 6);
        //});

        //EventManager.RegisterEvent(TestUIEventMacro.ON_CLICK_ADD_ITEM, (object[] paras) =>
        //{
        //    if (TestUserData.itemBag == null)
        //    {
        //        HintMsg.Instance.Hint("数据未初始化好");
        //        return;
        //    }
        //    TestUserData.itemBag.AddItem(Random.Range(100000, 9999999), "" + Random.Range(100000, 999999), Random.Range(1, 100));
        //});

        //EventManager.RegisterEvent(TestUIEventMacro.ON_CLICK_ADD_PAGE, (object[] paras) =>
        //{
        //    if (TestUserData.itemBag == null)
        //    {
        //        HintMsg.Instance.Hint("数据未初始化好");
        //        return;
        //    }
        //    TestUserData.itemBag.AddCapacity(6);
        //});
    }

    // Use this for initialization
    void Start()
    {
        InitEvents();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Awake()
    {
        uiOpRoot = GameObject.Find("Canvas").AddComponent<UIOperateBase>();
    }
}
