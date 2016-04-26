using UnityEngine;
using System.Collections;

public class DayNight : MonoBehaviour {

	public int hour;
	public int minute;
	float startTime;
	float lastMinuteStartTime;
	public Light sunLight;
	public float timeScaleFactor = 120;
	public float[] lightIntensity = {0.18f, 0.22f, 0.26f, 0.30f, 0.34f, 0.38f, 0.42f, 0.46f,
		0.50f, 0.54f, 0.66f, 0.76f, 0.86f, 1.0f,
		0.86f, 0.70f, 0.68f, 0.60f, 0.58f,
		0.5f, 0.45f, 0.40f, 0.3f, 0.2f,
	};
	Vector3 GetLightAngles(int hour)
	{
		float y = hour * 15;
		float x = 0f;
		if (hour >= 6 && hour <= 12) {
			x = (hour - 5) * (12);
		} else if (hour >= 12 && hour <= 18) {
			x = (19 - hour) * (11);
		} else
			x = 30;
		return new Vector3 (x, y, 0);
	}
	//public Vector3 
	// Use this for initialization
	void Start () {
		hour = Random.Range (0, 24);
		minute = Random.Range (0, 60);
		ResetTime (hour, minute);
	}

	void ResetTime(int hour, int minute)
	{
		this.hour = hour;
		this.minute = minute;
		this.startTime = Time.realtimeSinceStartup;
		this.lastMinuteStartTime = Time.realtimeSinceStartup;
	}

	void OnMinuteIncrease(int delta)
	{

		this.minute += delta;
		if (this.minute >= 60) {
			this.minute %= 60;
			this.hour += 1;
			this.hour %= 24;
		}

		UpdateDayNightLight ();
	}

	void UpdateDayNightLight()
	{
		float currHourIntensity = lightIntensity [this.hour];
		float nextHourIntensity = lightIntensity [(this.hour + 1) % 24];
		float rate = ((float)this.minute / 60f);
		float intensity = Mathf.Lerp (currHourIntensity, nextHourIntensity, rate);
		Vector3 currHourEulerAngles = GetLightAngles (this.hour);
		Quaternion currHourRotation = Quaternion.Euler (currHourEulerAngles);
		Vector3 nextHourEulerAngles = GetLightAngles (this.hour + 1);
		Quaternion nextHourRotation = Quaternion.Euler (nextHourEulerAngles);
		Quaternion lightRotation = Quaternion.Lerp (currHourRotation, nextHourRotation, rate); 
		RenderSettings.ambientIntensity = intensity / 2f;
		sunLight.intensity = intensity / 2f;
		sunLight.transform.rotation = lightRotation;
	}
	// Update is called once per frame
	void Update () {
		float now = Time.realtimeSinceStartup;
		float delta = (now - this.lastMinuteStartTime) * timeScaleFactor;
		if (delta >= 60)
		{
			this.lastMinuteStartTime = Time.realtimeSinceStartup;
			OnMinuteIncrease((int)(delta / 60));
		}
	}
}
