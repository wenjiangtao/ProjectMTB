using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MTB;
public class MainUIButton : MonoBehaviour
{
    public int _id;

    private GameObject _prefab;
    private GameObject _selectImage;
    private const int OFFSET_X = 0;
    private const int OFFSET_Y = 0;
    private MainUIIcon _iconBase;

    public void initViews(int id)
    {
        _id = id;
        _selectImage = GameObject.Find("Image" + _id);
        setSelected(false);
        _prefab = _prefab == null ? Resources.Load(BagConfig.ICON_PREFAB_PATH) as GameObject : _prefab;
        GameObject icon = GameObject.Instantiate(_prefab) as GameObject;
        RectTransform rectTrans = icon.GetComponent<RectTransform>();
        Vector3 localPosition = rectTrans.anchoredPosition3D;
        rectTrans.SetParent(transform);
        rectTrans.localScale = Vector3.one;
        localPosition.x += OFFSET_X;
        localPosition.y += OFFSET_Y;
        rectTrans.anchoredPosition3D = localPosition;
        icon.AddComponent<MainUIIcon>();
        _iconBase = icon.GetComponent<MainUIIcon>();
        _iconBase.Init(_id.ToString(), UITypes.MAIN_UI);
        _iconBase.setEnable(false);
        addEventListener();
    }

    private void addEventListener()
    {
		EventManager.RegisterEvent(EventMacro.ON_CHANGE_HANDCUBE, onChangeHandeCube);
		gameObject.GetComponent<Button>().onClick.AddListener(delegate() {
//			if(_iconBase.materialId == 0)return;
			EventManager.SendEvent(EventMacro.ON_CHANGE_HANDCUBE, _iconBase.materialId,_id);
		});
       
    }

    private void onChangeHandeCube(params object[] paras)
    {
        if (_id == int.Parse(paras[1].ToString()))
        {
            setSelected(true);
        }
		else
		{
			setSelected(false);
		}
    }

    private void setSelected(bool b)
    {
        _selectImage.SetActive(b);
    }

    public void setEnable(bool b)
    {
        _iconBase.setEnable(b);
    }
}
