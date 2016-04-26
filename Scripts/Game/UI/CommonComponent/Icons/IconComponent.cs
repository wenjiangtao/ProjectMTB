/****
 * 用于单个图标显示的管理
 * ****/
using UnityEngine;
using System;
namespace MTB
{
    public class IconComponent : MonoBehaviour, IUIComponent
    {
        protected UITypes _uiType;
        private string _iconResPath = "UI/Icon/ItemIcons/";
        protected string _iconPrefabPath = "UI/Common/Icon";
        protected int _iconId;
        protected UIIcon _iconBase;
        private GameObject _prefab;
        private GameObject _iconContainer;

        public void initComponents(params object[] paras)
        {
            string containerName = Convert.ToString(paras[0]);
            _uiType = (UITypes)paras[1];
            //_iconResPath = paras[2] == null ? _iconResPath : Convert.ToString(paras[2]);
            //_iconPrefabPath = paras[3] == null ? _iconPrefabPath : Convert.ToString(paras[3]);
            _iconContainer = GameObject.Find(containerName);

            _prefab = _prefab == null ? Resources.Load(_iconPrefabPath) as GameObject : _prefab;
            GameObject icon = GameObject.Instantiate(_prefab) as GameObject;
            RectTransform rectTrans = icon.GetComponent<RectTransform>();
            Vector3 localPosition = rectTrans.anchoredPosition3D;
            rectTrans.SetParent(_iconContainer.transform);
            icon.transform.parent = _iconContainer.transform;
            rectTrans.localScale = Vector3.one;
            rectTrans.anchoredPosition3D = localPosition;
            icon.AddComponent<UIIcon>();
            _iconBase = icon.GetComponent<UIIcon>();
            _iconBase.Init("0", _uiType, _iconResPath);
        }

        public void setIconId(int IconId)
        {
            string resName = IconResManager.getIconNameByMId(IconId);
            UIEventManager.SendEvent(UIEventManager.ET_UI_UPDATE, "0", _uiType, "Image", "", resName, IconId);
        }

        public void removeIcon()
        {
            UIEventManager.SendEvent(UIEventManager.ET_UI_UPDATE, "0", _uiType, "Image", "null", "", 0);
        }

        public void dispose()
        {
        }
    }
}
