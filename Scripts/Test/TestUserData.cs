using UnityEngine;
using System.Collections;

public class TestUserData{
	public static ModelBag itemBag;
	public static void InitItemBag(string name, int capacity)
	{
		if (itemBag != null)
			return;
		itemBag = new ModelBag (name, capacity);
	}
}
