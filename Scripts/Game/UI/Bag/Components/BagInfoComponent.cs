using UnityEngine;
using System;
using System.Collections;
namespace MTB
{
    public class BagInfoComponent : MonoBehaviour, IBagComponent
    {
        private GameObject _iconContainer;
        private GameObject _prefab;
        private UIIcon _iconBase;
        private UITypes _uiType;
        private string _iconResPath = "UI/Icon/ItemIcons/";
        private string _iconPrefabPath = "UI/Common/Icon";

        public void initComponents(params object[] paras)
        {
            _uiType = (UITypes)paras[0];
            _iconContainer = GameObject.Find("BagSelectContainer");
            UIEventManager.RegisterEvent(UIEventManager.ET_UI_CLICK, _uiType.ToString(), onSelect);
            _prefab = _prefab == null ? Resources.Load(_iconPrefabPath) as GameObject : _prefab;
            GameObject icon = GameObject.Instantiate(_prefab) as GameObject;
            RectTransform rectTrans = icon.GetComponent<RectTransform>();
            Vector3 localPosition = rectTrans.anchoredPosition3D;
            rectTrans.SetParent(_iconContainer.transform);
            rectTrans.anchoredPosition3D = localPosition;
            rectTrans.localScale = Vector3.one;

            icon.AddComponent<UIIcon>();
            _iconBase = icon.GetComponent<UIIcon>();
            _iconBase.Init("0", _uiType, _iconResPath);
            _iconBase.setEnable(false);
        }

        private void onSelect(params object[] paras)
        {
            _iconBase.setEnable(true);
            int materialId = Convert.ToInt32(paras[3]);
            _iconBase.OnUpdate(_uiType, "Image", "", IconResManager.getIconNameByMId(materialId), Convert.ToInt32(paras[3]));
        }

        public void refresh()
        {
        }

        public void dispose()
        {
            UIEventManager.UnRegisterEvent(UIEventManager.ET_UI_CLICK, _uiType.ToString(), onSelect);
        }

        public void updateInfo() { }
    }

}
