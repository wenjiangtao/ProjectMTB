using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class UIOpBag : UIOperateBase
{

    #region view compoent
    Text totalPage;
    Text currPage;
    GameObject prevBtnGo;
    GameObject nextBtnGo;
    #endregion

    #region logic
    int currPageIdx = 1;
    int totalPageCnt = 1;
    #endregion
    public override void Init(params object[] paras)
    {
        InitView();
        InitEvents();
    }

    protected override void InitEvents()
    {
        EventManager.RegisterEvent(TestUIEventMacro.ON_CLICK_BAG_NEXT, OnClickBagNext);
        EventManager.RegisterEvent(TestUIEventMacro.ON_CLICK_BAG_PREV, OnClickBagPrev);

        EventManager.RegisterEvent(TestModelEventMacro.ME_ITEM_BAG_SET_CAPACITY, OnCapacityChange);
        EventManager.RegisterEvent(TestModelEventMacro.ME_ADD_BAG_ITEM, OnAddItem);
    }

    void OnClickBagNext(params object[] paras)
    {
        Debug.Log("OnClickBagNext~~~~~~~~~~~~~");
    }

    void OnClickBagPrev(params object[] paras)
    {
        Debug.Log("OnClickBagPrev~~~~~~~~~~~~~");
    }

    void OnCapacityChange(params object[] paras)
    {
        SetTotalPage(System.Convert.ToInt32(paras[0]) / 6);
    }

    void OnAddItem(params object[] paras)
    {
        int idx = System.Convert.ToInt32(paras[0]);
        int id = System.Convert.ToInt32(paras[1]);
        string name = System.Convert.ToString(paras[2]);
        int count = System.Convert.ToInt32(paras[3]);
        AddItem(idx, id, name, count);
    }

    void AddItem(int idx, int id, string name, int count)
    {

    }

    void SetTotalPage(int total)
    {
        this.totalPageCnt = total;
        totalPage.text = "" + total;
    }

    void SetCurrentPage(int current)
    {
        currPageIdx = current;
        currPage.text = "" + current;
    }

    void InitViewComponent()
    {
        totalPage = viewGo.transform.FindChild("TotalPage").GetComponent<Text>();
        currPage = viewGo.transform.FindChild("CurrPage").GetComponent<Text>();
        nextBtnGo = viewGo.transform.FindChild("Next").gameObject;
        prevBtnGo = viewGo.transform.FindChild("Prev").gameObject;
    }

    public override void InitView()
    {
        base.InitView();
        RectTransform rect = viewGo.GetComponent<RectTransform>();
        rect.SetParent(GameObject.Find("/UIView").transform, false);
        InitViewComponent();

        ModelBag modelBag = TestUserData.itemBag;
        int count = modelBag.Count();
        SetCurrentPage(1);
        SetTotalPage(modelBag.GetCapacity() / 6);
        for (int i = 0; i < count; i++)
        {
            BagItem item = modelBag.GetItemAtIndex(i);
            AddItem(i, item.id, item.name, item.count);
        }

        if (currPageIdx > 1)
            prevBtnGo.SetActive(true);
        else
            prevBtnGo.SetActive(false);
        if (currPageIdx < count)
            nextBtnGo.SetActive(true);
        else
            nextBtnGo.SetActive(false);
    }
}
