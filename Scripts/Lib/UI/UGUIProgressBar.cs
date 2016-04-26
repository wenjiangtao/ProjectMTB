using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class UGUIProgressBar : MonoBehaviour, IUIElement {

	public Slider slider;
	public void SetValue(string attr, object value)
	{
		slider.value = (float)(value);
	}

	void Start () {
		if (slider == null) {
			slider = GetComponent<Slider>();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
