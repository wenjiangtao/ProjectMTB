using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UGUIImage : MonoBehaviour, IUIElement
{

    Image image;
    Vector3 localPosition = new Vector3(0, 0, 0);
	string resPath = "UI/Icon/ItemIcons/";

    void Start()
    {
        if (image == null)
        {
            image = gameObject.GetComponent<Image>();
        }
    }

    void Update()
    {
        image.transform.localPosition = localPosition;
    }

    public void SetPath(string path)
    {
        resPath = path;
    }

    public void SetValue(string attr, object value)
    {
		if (image == null)
		{
			image = gameObject.GetComponent<Image>();
		}
        if (attr == "null")
        {
//            image.enabled = false;
			image.color = new Color(image.color.r,image.color.g,image.color.b,0f);
            return;
        }
		image.enabled = true;
        string name = System.Convert.ToString(value);
        string path = resPath + name;
		Sprite s = Resources.Load<Sprite>(path);
		image.sprite = s;
    }
}
