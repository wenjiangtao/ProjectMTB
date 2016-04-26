using UnityEngine;
using System.Collections;
using MTB;

public class MTBEditorModeController : Singleton<MTBEditorModeController>
{
    private IEditorController[] _controllerList;

    private MTBEditorModeType _modetype;

    private EditorSaveController _saveController;
    private EditorLoadController _loadController;

    public void initController()
    {
        _controllerList = new IEditorController[5];
        _controllerList[0] = gameObject.AddComponent<EditorEditController>();
        _controllerList[1] = gameObject.AddComponent<EditorSelectController>();
        _controllerList[2] = gameObject.AddComponent<EditorDragController>();
        _controllerList[3] = gameObject.AddComponent<EditorFillController>();
        _controllerList[4] = gameObject.AddComponent<EditorRecordController>();
        changeMode(MTBEditorModeType.EDITOR);
        _saveController = new EditorSaveController();
    }

    public void initFileLoader()
    {
        _loadController = new EditorLoadController();
    }

    public void changeMode(MTBEditorModeType type)
    {
        _modetype = type;
        for (int i = 0; i < _controllerList.Length; i++)
        {
            if (_controllerList[i].editorType() == type)
                _controllerList[i].enable();
            else
                _controllerList[i].disEnable();
        }
        if (type == MTBEditorModeType.DRAG || type == MTBEditorModeType.COPY)
        {
            EventManager.SendEvent(EventMacro.ON_CHANGE_HANDCUBE, 0, 0);
        }
    }

    public MTBEditorModeType curMode()
    {
        return _modetype;
    }

    public void saveData()
    {
        _saveController.saveData();
    }

    public void loadData()
    {
        SelectAreaParams paras = _loadController.loadData();
        if (paras == null)
            return;
        EditorSelectArea.Instance.load(paras);
        changeMode(MTBEditorModeType.DRAG);
        _controllerList[1].setState(2);
        _controllerList[2].setState(2);
    }

    public void saveBirthPos()
    {
        _saveController.saveBirthPos();
    }

    public Vector3 loadBirthPos(bool useSaveBirth)
    {
        return _loadController.loadBirthPos(useSaveBirth);
    }

    public void cancel()
    {
        for (int i = 0; i < _controllerList.Length; i++)
        {
            _controllerList[i].cancel();
        }
        changeMode(MTBEditorModeType.EDITOR);
    }

    public void dispose()
    {
        changeMode(MTBEditorModeType.EDITOR);
        for (int i = 0; i < _controllerList.Length; i++)
        {
            _controllerList[i].cancel();
            _controllerList[i] = null;
        }
    }
}

