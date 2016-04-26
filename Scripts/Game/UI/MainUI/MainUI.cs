using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using MTB;
public class MainUI : UIOperateBase
{
    private int[] SUM_BUTTONS = { 5, 6 };
    private int _curSceneType = 1;
    private int[] _materials;
    private GameObject[] _buttons;
    private GameObject _systemBtn;
    private GOPlayerController _playerController;

    private GameObject _bagBtn;
    private GameObject _newsBtn;
    private GameObject _btnJump;

    private int _itemNum;
    private int _curSelectId;

    // Use this for initialization
    public override void Init(params object[] paras)
    {
        uiType = UITypes.MAIN_UI;
        initComponents();
        base.Init(paras);
    }

    public override void Open()
    {
        base.Open();
        if (_playerController == null)
        {
            _playerController = HasActionObjectManager.Instance.playerManager.getMyPlayer().GetComponent<GOPlayerController>();
        }
        SetAllActive(true);
    }

    public override void InitView()
    {
        viewGo = this.gameObject;
    }

    public override void Dispose()
    {
        _playerController = null;
        _buttons = null;
        base.Dispose();
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
        _systemBtn = GameObject.Find("system");
        _bagBtn = GameObject.Find("ButtonBag");
        _btnJump = GameObject.Find("ButtonJump");
        _newsBtn = GameObject.Find("news");
    }

    public void addItemIcon(int id)
    {
        _buttons[id].AddComponent<MainUIButton>();
        _buttons[id].GetComponent<MainUIButton>().initViews(id);
    }

    protected override void InitEvents()
    {
        UIEventManager.RegisterEvent(UIEventManager.ET_UI_CLICK, UITypes.MAIN_BAG.ToString(), onSelectFromBag);
        EventManager.RegisterEvent(EventMacro.ON_CHANGE_HANDCUBE, onChangeHandeCube);
        EventManager.RegisterEvent(EventMacro.CLOSE_UI, (object[] paras) =>
         {
             if ((UITypes)paras[0] == UITypes.CROSS)
             {
                 return;
             }
             if ((UITypes)paras[0] == UITypes.MAIN_BAG)
             {
                 SetNormalBtnsActive(true);
             }
             else if ((UITypes)paras[0] != UITypes.MAIN_UI)
             {
                 SetAllActive(true);
             }
         });

        EventManager.RegisterEvent(EventMacro.SHOW_UI, (object[] paras) =>
        {
            if ((UITypes)paras[0] == UITypes.CROSS)
            {
                return;
            }
            if ((UITypes)paras[0] == UITypes.MAIN_BAG)
            {
                SetNormalBtnsActive(false);
            }
            else if ((UITypes)paras[0] != UITypes.MAIN_UI && (UITypes)paras[0] != UITypes.TASK)
            {
                SetAllActive(false);
            }
        });
        _bagBtn.GetComponent<Button>().onClick.AddListener(delegate()
        {
            UIManager.Instance.showUI<MaterialBag>(UITypes.MAIN_BAG);
        });
        _systemBtn.GetComponent<Button>().onClick.AddListener(delegate()
        {
            OnClickSystemBtn();
        });
        EventTriggerListener.Get(_btnJump).onDown += On_JumpClickDown;
        EventTriggerListener.Get(_btnJump).onUp += On_JumpClickUp;
    }

    protected override void removeEvents()
    {
        EventTriggerListener.Get(_btnJump).onDown -= On_JumpClickDown;
        EventTriggerListener.Get(_btnJump).onUp -= On_JumpClickUp;
    }

    private void On_JumpClickDown(GameObject gameObject)
    {
        _playerController.Jump();
    }

    private void On_JumpClickUp(GameObject gameObject)
    {
        _playerController.StopJump();
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

    private void OnClickSystemBtn()
    {
        UIManager.Instance.showUI<SetUpUI>(UITypes.SETUP);
        //EventManager.SendEvent(EventMacro.ON_CLICK_SWITCH_VIEW);
    }

    private void OnClickCross()
    {
        EventManager.SendEvent(EventMacro.ON_CLICK_SWITCH_PROPMODEL);
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

    private void SetNormalBtnsActive(bool b)
    {
        _systemBtn.SetActive(b);

        _btnJump.SetActive(b);
        _newsBtn.SetActive(b);
        MTBUserInput.Instance.SetJoyStickActive(b);
    }

    private void SetAllActive(bool b)
    {
        foreach (GameObject btn in _buttons)
        {
            btn.SetActive(b);
        }
        _bagBtn.SetActive(b);
        SetNormalBtnsActive(b);
    }
}
