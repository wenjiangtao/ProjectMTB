using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class UGUIText : MonoBehaviour, IUIElement{
	public Text text;
	// Use this for initialization
	void Start () {
		if (text == null)
			text = GetComponent<Text> ();
	}
	public void SetValue(string attr, object value)
	{
        if (text == null) {
            text = GetComponent<Text>();
        }
		if (attr.Equals ("text"))
			text.text = System.Convert.ToString (value);
	}
}
