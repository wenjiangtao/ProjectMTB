using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BagItem
{
	public int id;
	public string name;
	public int count;
	public BagItem(int id, string name, int count)
	{
		this.id = id;
		this.name = name;
		this.count = count;
	}
}

public class ModelBag {
	string name;
	List<BagItem> itemList = new List<BagItem>();
	HashSet<int> itemSet = new HashSet<int> ();
	int capacity = 6;

	public ModelBag(string name, int capacity)
	{
		this.name = name;
		this.capacity = capacity;
		EventManager.SendEvent (TestModelEventMacro.ME_ITEM_BAG_INIT, this.name);

	}

	public void AddCapacity(int amount)
	{
		SetCapacity(this.capacity + amount);
	}

	public bool ContainsItem(int id)
	{
		return itemSet.Contains (id);
	}

	public int GetItemIndex(int id)
	{
		for (int i = 0; i < itemList.Count; i++) {
			if (id == itemList[i].id)
				return i;
		}
		return -1;
	}

	public BagItem GetItemAtIndex(int idx)
	{
		if (idx < 0 || idx >= itemList.Count)
			return null;
		return itemList [idx];
	}

	public BagItem AddItem(int id, string name, int count)
	{
		if (itemSet.Contains(id))
			return null;
		BagItem item = new BagItem (id, name, count);
		itemSet.Add (id);
		itemList.Add (item);
		EventManager.SendEvent (TestModelEventMacro.ME_ADD_BAG_ITEM, item, itemList.Count - 1);
		return item;
	}

	public void DelItem(int id)
	{
		if (!itemSet.Contains (id)) {
			return;
		}
		int idx = GetItemIndex (id);
		itemList.RemoveAt (idx);
		itemSet.Remove (id);
		EventManager.SendEvent (TestModelEventMacro.ME_DEL_BAG_ITEM, id);
	}

	public void Sort()
	{
		itemList.Sort ();
		EventManager.SendEvent (TestModelEventMacro.ME_ITEM_BAG_SORT);
	}

	public void SetCapacity(int capacity)
	{
		if (capacity <= this.itemList.Count)
			return;
		this.capacity = capacity;
		EventManager.SendEvent (TestModelEventMacro.ME_ITEM_BAG_SET_CAPACITY, this.capacity);
	}

	public int Count()
	{
		return itemList.Count;
	}
	public int GetCapacity()
	{
		return this.capacity;
	}
}
