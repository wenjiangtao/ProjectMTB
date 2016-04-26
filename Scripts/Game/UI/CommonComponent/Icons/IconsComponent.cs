using UnityEngine;
using System;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
namespace MTB
{
    public class IconsComponent : MonoBehaviour, IDragHandler, IUIComponent
    {
        protected UITypes _uiType;
        private string _iconResPath = "UI/Icon/ItemIcons/";
        protected string _iconPrefabPath = "UI/Common/BagButton";
        private int[] _materials;
        private GameObject[] _buttons;
        private UIIcon[] _iconbases;
        private GameObject _prefab;
        private int _iconSumPerPage;
        private GameObject _selectSprite;
		private Transform _bagBtnGrid;
        private int _curTabIndex = 0;
        private const int OFFSET_X = 0;
        private const int OFFSET_Y = 0;
//		private int[] tabIndexToItemType = new int[]{Item.GetItemType((int)ItemType.Block,(int)ItemBlockType.terrain)
//			,Item.GetItemType((int)ItemType.Block,(int)ItemBlockType.plant)
//			,Item.GetItemType((int)ItemType.CollectTool,(int)ItemCollectToolType.defalut)
//			,int.MaxValue,int.MaxValue};
		private int[] tabIndexToItemType = new int[]{0,int.MaxValue,int.MaxValue,int.MaxValue,int.MaxValue};

        public void OnDrag(PointerEventData eventData)
        {
            if (_selectSprite == null)
                return;
            _selectSprite.SetActive(false);
        }

        /***
         * @params:
         *  iconsumperpage
         *  iconname
         *  materialdic
         *  iconrespath* 
         *  iconprefabpath*
         * **/
        public void initComponents(params object[] paras)
        {
			//暂时用同一个size
			_iconSumPerPage = BackpackItemManager.Instance.GetSize(tabIndexToItemType[0]);
            string iconName = Convert.ToString(paras[0]);
            _uiType = (UITypes)paras[1];
            _iconResPath = paras[2] == null ? _iconResPath : Convert.ToString(paras[2]);
            _materials = new int[_iconSumPerPage];
            _iconbases = new UIIcon[_iconSumPerPage];
            _buttons = new GameObject[_iconSumPerPage];

			_bagBtnGrid = this.transform.Find("Grid");
            for (int i = 0; i < _iconSumPerPage; i++)
            {
                addItemIcon(i);
            }
            if (paras[3] != null)
            {
                _selectSprite = GameObject.Find(Convert.ToString(paras[3]));
                _selectSprite.SetActive(false);
            }
			GridLayoutGroup glg = _bagBtnGrid.GetComponent<GridLayoutGroup>();
			float height = (glg.cellSize.y + glg.spacing.y) * (_iconSumPerPage / 6);
			RectTransform rtf = _bagBtnGrid.GetComponent<RectTransform>();
			rtf.sizeDelta = new Vector2(rtf.sizeDelta.x,height);
            EventManager.RegisterEvent(UIEventMacro.CLICK_TAB, onClickTab);
            UIEventManager.RegisterEvent(UIEventManager.ET_UI_CLICK, _uiType.ToString(), onSelect);
            showIcon(0);
        }

        public void addItemIcon(int id)
        {
            _prefab = _prefab == null ? Resources.Load(_iconPrefabPath) as GameObject : _prefab;
            GameObject bagButton = GameObject.Instantiate(_prefab) as GameObject;
			_buttons[id] = bagButton;
//			bagButton.transform.parent = _bagBtnGrid;
			RectTransform rectTrans = bagButton.GetComponent<RectTransform>();
			rectTrans.SetParent(_bagBtnGrid);
			rectTrans.localScale = Vector3.one;
			rectTrans.localPosition = new Vector3(rectTrans.localPosition.x,rectTrans.localPosition.y,0);
//			rectTrans.localPosition;
//            RectTransform rectTrans = bagButton.GetComponent<RectTransform>();
//            Vector3 localPosition = rectTrans.anchoredPosition3D;
//            rectTrans.SetParent(_buttons[id].transform);
//            bagButton.transform.parent = _buttons[id].transform;
//            rectTrans.localScale = Vector3.one;
//            //rectTrans.localScale = Vector3.one * (_buttons[id].GetComponent<RectTransform>().rect.width / rectTrans.rect.width);
//            localPosition.x += OFFSET_X;
//            localPosition.y += OFFSET_Y;
//            rectTrans.anchoredPosition3D = localPosition;
			UIIcon iconBase = bagButton.transform.Find("Icon").gameObject.AddComponent<UIIcon>();
            iconBase.Init(id.ToString(), _uiType, _iconResPath);
//            iconBase.setEnable(false);
            _iconbases[id] = iconBase;
        }

        public void showIcon(int tabindex)
        {
			int itemKey = tabIndexToItemType[tabindex];
			for (int j = 0; j < _iconSumPerPage; j++) {
				int itemId = BackpackItemManager.Instance.GetId(itemKey,j);
				if(itemId <= 0)
					UIEventManager.SendEvent(UIEventManager.ET_UI_UPDATE, j.ToString(), _uiType, "Image", "null", "", itemId);
				else
					UIEventManager.SendEvent(UIEventManager.ET_UI_UPDATE, j.ToString(), _uiType, "Image", "", IconResManager.getIconNameByMId(itemId), itemId);
			}

//            string[] sp = { "," };
//            string[] ms = _materialsDic[tabindex].ToString().Split(sp, _iconSumPerPage, System.StringSplitOptions.RemoveEmptyEntries);
//            for (int j = 0; j < ms.Length; j++)
//            {
//                _materials[j] = Int32.Parse(ms[j]);
//                updateItemIcon(j, IconResManager.getIconNameByMId(Int32.Parse(ms[j])));
//            }
//            cleanIcons(ms.Length);
        }

//        private void cleanIcons(int index)
//        {
//            for (; index < _iconSumPerPage; index++)
//            {
//                removeItemIcon(index);
//            }
//        }

//        public void updateItemIcon(int id, params object[] paras)
//        {
//            _iconbases[id].setEnable(true);
//            UIEventManager.SendEvent(UIEventManager.ET_UI_UPDATE, id.ToString(), _uiType, "Image", "", paras[0], _materials[id]);
//        }
//
//        private void removeItemIcon(int id)
//        {
//            UIEventManager.SendEvent(UIEventManager.ET_UI_UPDATE, id.ToString(), _uiType, "Image", "null", "", _materials[id]);
//        }

        protected virtual void onClickTab(params object[] paras)
        {
            if ((UITypes)paras[0] == _uiType)
            {
                _curTabIndex = Convert.ToInt32(paras[1]) - 1;
                showIcon(_curTabIndex);
            }
            _selectSprite.SetActive(false);
        }

        private void onSelect(params object[] paras)
        {
            if (_selectSprite == null)
                return;
            int selectId = Convert.ToInt32(paras[4]);
            _selectSprite.transform.position = _buttons[selectId].transform.position;
            _selectSprite.SetActive(true);
        }


        public void dispose()
        {
        }
    }
}
