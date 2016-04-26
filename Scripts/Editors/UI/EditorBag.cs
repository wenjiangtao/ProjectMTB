/***
 * 物品面板
 * ***/
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using MTB;

public class EditorBag : MonoBehaviour
{
    private UITypes uiType = UITypes.MAIN_BAG;
    private int _id;
    private Dictionary<int, string> _materialsDic = new Dictionary<int, string>()
    {
         {0,"1,2,3,12,14,18,38,40,41,44,45,50,100"},
         {1,"42,43,46,48,49"},
         {2,""},
         {3,"1001,1002,1003,1004,1005,1006,1007,1008,1009,1010,1011,1012"}
    };

    private IBagComponent[] _components;

    void Start()
    {
        _id = 0;
        gameObject.AddComponent<BagIconsComponent>();
        gameObject.AddComponent<BagTabsComponent>();
        _components = new IBagComponent[2];
        _components[0] = gameObject.GetComponent<BagIconsComponent>();
        _components[1] = gameObject.GetComponent<BagTabsComponent>();
        foreach (IBagComponent comp in _components)
        {
            comp.initComponents(_materialsDic, uiType);
        }

        GameObject.Find("Bagclose").GetComponent<Button>().onClick.AddListener(delegate()
        {
            gameObject.SetActive(false);
        });
    }

    public void open()
    {
        gameObject.SetActive(true);
    }
}

