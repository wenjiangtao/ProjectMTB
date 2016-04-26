using System;
using UnityEngine;
public class TimeDebug
{
	private static DateTime start = DateTime.Now;
	private static string _name;
	public static double totalMilliseconds;
	public TimeDebug ()
	{
	}

	public static void Start(string name = "default")
	{
		_name = name;
		start = DateTime.Now;
		totalMilliseconds = 0;
	}

	public static void End()
	{
		TimeSpan ts = new TimeSpan(DateTime.Now.Ticks).Subtract(new TimeSpan(start.Ticks));
		start = DateTime.Now;
		totalMilliseconds = ts.TotalMilliseconds;
		Debug.Log("[ " + _name + " ]totalMilliseconds:"+ts.TotalMilliseconds);
	}
}

