using UnityEngine;
using System.Collections;

public class TestList : MonoBehaviour
{

    public void AddItem(string id)
    {
        GameObject prefab = Resources.Load("UI/Test/TestListItem") as GameObject;
        GameObject go = GameObject.Instantiate(prefab) as GameObject;
        //go.transform.parent = gameObject.transform;
        RectTransform rectTrans = go.GetComponent<RectTransform>();

        Vector3 localPosition = rectTrans.anchoredPosition3D;

        rectTrans.SetParent(gameObject.transform);
        rectTrans.localScale = Vector3.one;
        rectTrans.anchoredPosition3D = localPosition;
        TestListItem listItem = go.GetComponent<TestListItem>();
        listItem.Init(id);
        Vector3 position = rectTrans.localPosition;
        position.y = (int.Parse(id) - 1) * -60;
        position.x = 100;
        rectTrans.localPosition = position;
    }
}
