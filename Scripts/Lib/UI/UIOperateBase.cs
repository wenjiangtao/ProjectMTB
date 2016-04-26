using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MTB;
public class UIOperateBase : MonoBehaviour
{
    public static T New<T>(string name, params object[] paras) where T : UIOperateBase
    {
        GameObject g = GameObject.Find(name);
        T t = g.AddComponent<T>();
        t.SetParent(UIOperateRoot.uiOpRoot);
        t.name = name;
        t.Init(paras);
        t.hide();
        return t;
    }

    //用于动态加载UI
    public static T DynamicNew<T>(string name, string path, params object[] paras) where T : UIOperateBase
    {
        GameObject g = GameObject.Instantiate(Resources.Load(path) as GameObject) as GameObject;
        T t = g.AddComponent<T>();
        t.SetParent(UIOperateRoot.uiOpRoot);
        t.name = name;
        t.Init(paras);
        g.transform.localScale = Vector3.one;
        g.transform.localPosition = Vector3.zero;
        t.hide();
        return t;
    }

    public List<UIOperateBase> childrenList = new List<UIOperateBase>();
    public UIOperateBase parent = null;

    public GameObject viewGo;
    public UITypes uiType { get; set; }

    protected virtual void InitEvents()
    {
    }

    protected virtual void removeEvents()
    {
    }

    public virtual void Init(params object[] paras)
    {
        InitView();
    }

    public virtual void InitView()
    {
        viewGo = this.gameObject;
    }

    public void updateLayer(int layer)
    {
        if (gameObject.GetComponent<Canvas>() == null)
            return;
        gameObject.GetComponent<Canvas>().overrideSorting = true;
        gameObject.GetComponent<Canvas>().sortingOrder = layer;
    }

    public virtual void Open()
    {
        foreach (UIOperateBase child in childrenList)
            child.Open();
        viewGo.SetActive(true);
        InitEvents();
    }

    public virtual void Close()
    {
        foreach (UIOperateBase child in childrenList)
            child.Close();
        viewGo.SetActive(false);
        removeEvents();
    }

    public virtual void Dispose()
    {
        foreach (UIOperateBase child in childrenList)
            child.Dispose();
        this.transform.parent = null;
        this.parent = null;
        Destroy(viewGo);
        viewGo = null;
    }

    public void SetParent(UIOperateBase parent)
    {
        this.transform.parent = parent.transform;
        this.parent = parent;
        parent.AddChild(this);
        RectTransform rectTrans = GetComponent<RectTransform>();
        rectTrans.anchorMin = new Vector2(0.5f, 0.5f);
        rectTrans.anchorMax = new Vector2(0.5f, 0.5f);
    }

    protected virtual void hide()
    {
        viewGo.SetActive(false);
    }

    void AddChild(UIOperateBase child)
    {
        if (childrenList.Contains(child))
            return;
        childrenList.Add(child);
    }
}
