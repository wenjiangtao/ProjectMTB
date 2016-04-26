using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MTB;
public abstract class UIItemBase : MonoBehaviour
{
    //每个UIItem 是一个抽象单位比如一条排行榜记录
    //里面会包含多个Element，比如一个头像,军工数量,VIP等级，星数
    //UIElement只包含动态会变化的元素，不包含静态
    protected Dictionary<string, IUIElement> elementMap = new Dictionary<string, IUIElement>();
    public string id;
    public UITypes UItype;
    protected Transform mTrans;
    protected GameObject mGo;
    public virtual void Init(string id, params object[] paras)
    {
        mTrans = transform;
        mGo = gameObject;
        this.id = id;
        UItype = (UITypes)paras[0];
        Register();
        InitElements();
    }

    public virtual void InitElements()
    {
    }

    public virtual void Register()
    {
        UIEventManager.RegisterEvent(UIEventManager.ET_UI_UPDATE, this.id, OnUpdate);
    }

    public virtual void UnRegister()
    {
        UIEventManager.UnRegisterEvent(UIEventManager.ET_UI_UPDATE, this.id, OnUpdate);
    }

    public virtual void AddElement(string name, IUIElement element)
    {
        elementMap[name] = element;
    }

    public virtual void OnUpdate(string name, string attr, object value)
    {
        if (!elementMap.ContainsKey(name))
        {
            return;
        }
        IUIElement element = elementMap[name];
        element.SetValue(attr, value);
    }

    public virtual void OnUpdate(params object[] paras)
    {
        if (UItype != (UITypes)paras[0])
            return;
        string name = System.Convert.ToString(paras[1]);
        string attr = System.Convert.ToString(paras[2]);
        object value = paras[3];
        OnUpdate(name, attr, value);
    }

    public virtual void OnDestroy()
    {
        UnRegister();
    }

    public virtual void dispose()
    {
        UnRegister();
    }
}
