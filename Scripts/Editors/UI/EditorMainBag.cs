/***
 * 腰带面板
 * ***/

using UnityEngine;
using System;
using UnityEngine.UI;
using MTB;
public class EditorMainBag : MonoBehaviour
{
    private int[] SUM_BUTTONS = { 5, 6 };
    private int _curSceneType = 1;
    private int[] _materials;
    private GameObject[] _buttons;

    private GameObject _bagBtn;

    private EditorBag bag;

    private UITypes uiType = UITypes.MAIN_UI;

    private int _itemNum;
    private int _curSelectId;

    void Start()
    {
        initComponents();
        InitEvents();
    }

    private void initComponents()
    {
        _curSelectId = 0;
        _itemNum = 0;
        _buttons = new GameObject[SUM_BUTTONS[_curSceneType]];
        _materials = new int[SUM_BUTTONS[_curSceneType]];
        for (int i = 0; i < SUM_BUTTONS[_curSceneType]; i++)
        {
            _buttons[i] = GameObject.Find("Button_" + i);
            addItemIcon(i);
        }
        _bagBtn = GameObject.Find("ButtonBag");
    }

    public void addItemIcon(int id)
    {
        _buttons[id].AddComponent<MainUIButton>();
        _buttons[id].GetComponent<MainUIButton>().initViews(id);
    }

    private void InitEvents()
    {
        UIEventManager.RegisterEvent(UIEventManager.ET_UI_CLICK, UITypes.MAIN_BAG.ToString(), onSelectFromBag);
        EventManager.RegisterEvent(EventMacro.ON_CHANGE_HANDCUBE, onChangeHandeCube);

        _bagBtn.GetComponent<Button>().onClick.AddListener(delegate()
        {
            openBag();
        });

    }

    public void openBag()
    {
        if (bag != null)
        {
            bag.open();
        }
        else
        {
            bag = (GameObject.Instantiate(Resources.Load("UI/EditorUI/EditorBag")) as GameObject).GetComponent<EditorBag>();
        }
    }

    /***
* 现规则为物品格满了以后从背包选取物品会替换当前手持物品
* **/
    private void onSelectFromBag(params object[] paras)
    {
        int index = Array.IndexOf(_materials, (int)paras[3]);
        int id = _itemNum > SUM_BUTTONS[_curSceneType] - 1 ? _curSelectId : _itemNum;
        if (index == -1)
        {
            _materials[id] = (int)paras[3];
            updateItemIcon(id, paras[2]);
            EventManager.SendEvent(EventMacro.ON_CHANGE_HANDCUBE, (int)paras[3], id);
            _itemNum = _itemNum > SUM_BUTTONS[_curSceneType] - 1 ? SUM_BUTTONS[_curSceneType] - 1 : _itemNum;
            _itemNum++;
        }
        else
        {
            updateItemIcon(index, paras[2]);
        }
    }

    public void updateItemIcon(int id, params object[] paras)
    {
        _buttons[id].GetComponent<MainUIButton>().setEnable(true);
        UIEventManager.SendEvent(UIEventManager.ET_UI_UPDATE, id.ToString(), uiType, "Image", "", paras[0], _materials[id]);
    }

    public void removeItemIcon(int id)
    {
        _buttons[id].GetComponent<MainUIButton>().setEnable(true);
    }

    private void onChangeHandeCube(params object[] paras)
    {
        _curSelectId = int.Parse(paras[1].ToString());
    }
}

