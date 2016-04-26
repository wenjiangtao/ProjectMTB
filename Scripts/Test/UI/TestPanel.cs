using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/***
 *     UI面板测试
 *     使用  
 *     UIOperateBase.New<TestPanel>("TestPanel", "UI/Test/TestPanel").Open();   
 *     打开面板
 * ***/

public class TestPanel : UIOperateBase
{
    Hashtable btntables;
    TestList testlist;
    InputField textinput;
    int id;

    public override void Init(params object[] paras)
    {
        base.Init(paras);
        InitView();
        InitEvents();
        id = 1;
    }

    protected override void InitEvents()
    {
        base.InitEvents();
        (btntables["CloseBtn"] as Button).onClick.AddListener(Close);
        (btntables["AddItem"] as Button).onClick.AddListener(addItem);
        (btntables["SetText"] as Button).onClick.AddListener(updateText);
    }

    public override void InitView()
    {
        base.InitView();
        btntables = new Hashtable();
        foreach (Button btn in FindObjectsOfType<Button>())
        {
            btntables.Add(btn.name, btn);
        }
        textinput = FindObjectOfType<InputField>();
        if (textinput == null)
        {
            Debug.Log("textinput is null");
        }
        testlist = viewGo.GetComponent<TestList>();
        if (testlist == null)
        {
            Debug.Log("testlistisnull");
        }
    }

    public override void Close()
    {
        base.Close();
        Dispose();
    }

    private void addItem()
    {
        testlist.AddItem("" + id);
        id++;
    }

    private void updateText()
    {
        UIEventManager.SendEvent(UIEventManager.ET_UI_UPDATE, "" + (id - 1), "Name", "text", "" + textinput.text);
    }
}
