using UnityEngine;
using MTB;

public class EditorSelectController : MonoBehaviour, IEditorController
{
    private bool enableMark;
    private bool selectMark;
    private SelectAreaState selectStep;
    private GameObject _selectCube;
    private float rayDis;
    private SelectAreaParams _selectblockParams;

    public MTBEditorModeType editorType()
    {
        return MTBEditorModeType.SELECT;
    }

    void Awake()
    {
        selectStep = SelectAreaState.SelectArea;
        rayDis = 10f;
        enableMark = false;
        selectMark = false;
        _selectCube = GameObject.Instantiate(Resources.Load("Prefabs/Props/EditorSelectCube") as GameObject) as GameObject;
        _selectCube.name = "selectCube";
        _selectCube.SetActive(false);
        EditorSelectArea.Instance.Init();
    }

    void Update()
    {
        if (enableMark)
        {
            rayDis += Input.mouseScrollDelta.y;
            rayDis = rayDis <= 2 ? 2 : rayDis;
            checkSelectArea();
            checkChangeVolume(Time.deltaTime);
        }
    }

    void OnGUI()
    {
        if (enableMark)
        {
            int w = Screen.width, h = Screen.height;
            if (GUI.Button(new Rect(w - 100, h / 2 + 50, 100, 40), "大范围"))
            {
                selectStep = SelectAreaState.SelectBigFixedArea;
            }
            if (selectStep == SelectAreaState.SelectBigFixedAreaEnd)
            {
                if (GUI.Button(new Rect(w - 100, h / 2 + 90, 100, 40), "取消大范围"))
                {
                    _selectblockParams = null;
                    selectStep = SelectAreaState.SelectArea;
                    EditorSelectArea.Instance.cancel();
                }
            }

            if (selectStep == SelectAreaState.SelectAreaEnd || selectStep == SelectAreaState.SelectBigFixedAreaEnd)
            {

                if (GUI.Button(new Rect(w - 50, h / 2, 50, 40), "保存"))
                {
                    _selectblockParams = EditorSelectArea.Instance.save();
                    MTBEditorModeController.Instance.changeMode(MTBEditorModeType.EDITOR);
                }
                if (GUI.Button(new Rect(w - 50, h / 2 - 50, 50, 40), "取消"))
                {
                    _selectblockParams = null;
                    selectStep = SelectAreaState.SelectArea;
                    EditorSelectArea.Instance.cancel();
                }
                if (GUI.Button(new Rect(w - 50, h / 2 - 100, 50, 40), "旋转"))
                {
                    EditorSelectArea.Instance.changeAreaProportion();
                }
            }
        }
    }

    private void checkSelectArea()
    {
        if (selectStep == SelectAreaState.SelectArea)
        {
            _selectCube.SetActive(true);
            _selectCube.transform.position = EditorScreenRay.Instance.getMouseRayPositionByDistance(rayDis);

            if (Input.GetMouseButton(1))
            {
                //_selectArea.updateArea(EditorScreenRay.Instance.getMouseWorldPosition());
                EditorSelectArea.Instance.updateArea(EditorScreenRay.Instance.getMouseRayPositionByDistance(rayDis));
                selectMark = true;
            }
            if (selectMark && Input.GetMouseButtonUp(1))
            {
                EditorSelectArea.Instance.endDrag();
                selectStep = SelectAreaState.SelectAreaEnd;
                selectMark = false;
                _selectCube.SetActive(false);
            }
        }
        if (selectStep == SelectAreaState.SelectBigFixedArea)
        {
            EditorSelectArea.Instance.selectBigArea();
            EditorSelectArea.Instance.updateDragMoveArea();
            if (Input.GetMouseButtonUp(1))
            {
                EditorSelectArea.Instance.endDrag();
                selectStep = SelectAreaState.SelectBigFixedAreaEnd;
            }
        }
        else if (selectStep == SelectAreaState.SelectBigFixedAreaEnd)
        {
            if (Input.GetMouseButtonUp(1))
            {
                selectStep = SelectAreaState.SelectBigFixedArea;
            }
        }
    }

    private void checkChangeVolume(float deltaTime)
    {
        if (selectStep == SelectAreaState.SelectAreaEnd && Input.GetMouseButton(1))
            if (EditorSelectArea.Instance.setAreaDragType(EditorScreenRay.Instance.getMouseWorldPositionByName("SelectArea")))
                selectStep = SelectAreaState.SelectVolume;
        if (selectStep == SelectAreaState.SelectVolume)
            if (Input.GetMouseButton(1))
                EditorSelectArea.Instance.updateDragVolume(deltaTime);
            else
            {
                EditorSelectArea.Instance.endDrag();
                selectStep = SelectAreaState.SelectAreaEnd;
            }
    }


    public void setState(int state)
    {
        selectStep = (SelectAreaState)state;
    }


    public void enable()
    {
        MTBUserInput.Instance.Touch.setEnabled(true);
        enableMark = true;
    }

    public void disEnable()
    {
        enableMark = false;
        _selectCube.SetActive(false);
    }

    public void cancel()
    {
        selectStep = SelectAreaState.SelectArea;
        EditorSelectArea.Instance.cancel();
    }
}

public enum SelectAreaState
{
    SelectArea = 1,
    SelectAreaEnd = 2,
    SelectVolume = 3,
    SelectBigFixedArea = 4,
    SelectBigFixedAreaEnd = 5
}
